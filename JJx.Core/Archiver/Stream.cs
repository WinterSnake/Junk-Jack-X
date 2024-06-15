/*
	Junk Jack X: Core
	- [Archiver]Stream

	Segment Breakdown:
	----------------------------------------------------------------------------------------------------
	Segment[0x0 : 0x3] = Magic       | Length: 4 (0x4) | Type: char[4]
	Segment[0x4 : 0x5] = File Type   | Length: 2 (0x2) | Type: enum[uint16] | Parent: ArchiverStreamType
	Segment[0x6 : 0x7] = Chunk Count | Length: 2 (0x2) | Type: uint16
	Segment[0x8 : 0xB] = UNKNOWN     | Length: 4 (0x4) | Type: ???
	----------------------------------------------------------------------------------------------------
	Size: 12 (0xC)

	Written By: Ryan Smith
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace JJx;

public enum ArchiverStreamType : ushort
{
	Player    = 0x00,
	World     = 0x01,
	Adventure = 0x02,
}

public sealed class ArchiverStream : FileStream
{
	/* Constructors */
	private ArchiverStream(ArchiverStreamType type, List<ArchiverChunk> chunks, SafeFileHandle readerHandle): base(readerHandle, FileAccess.Read)
	{
		this.Type = type;
		this._Chunks = chunks;
	}
	private ArchiverStream(ArchiverStreamType type, string filePath): base(filePath, FileMode.Create, FileAccess.Write)
	{
		this.Type = type;
		this._Chunks = new List<ArchiverChunk>();
		this._Buffer = new MemoryStream();
	}
	/* Instance Methods */
	public override void Close()
	{
		if (this.CanWrite)
			this.CloseAsync().Wait();
		base.Close();
	}
	public async Task CloseAsync()
	{
		if (this._HasClosed) return;
		// Account for header offset (BUFFER + PADDING); Total chunks offset (Chunks * Chunk.SIZE); bufferedstream start position (this.Position)
		uint chunkOrigin = (uint)(SIZEOF_BUFFER + SIZEOF_PADDING + (this._Chunks.Count * ArchiverChunk.SIZE));
		#if DEBUG
			Console.WriteLine($"Chunk Origin: 0x{chunkOrigin:X4}");
		#endif
		// Chunk Count
		var buffer = new byte[ArchiverStream.SIZEOF_BUFFER - 2];
		BitConverter.LittleEndian.Write((ushort)this._Chunks.Count, buffer);
		// -UNKNOWN(4)-\\
		await this.WriteAsync(buffer, 0, buffer.Length);
		// Chunks
		foreach (var chunk in this._Chunks)
		{
			#if DEBUG
				Console.WriteLine($"Old Position: 0x{chunk.Position:X4} | New Position: 0x{chunk.Position + (chunk.Type != ArchiverChunkType.Padding ? chunkOrigin : 0):X4}");
			#endif
			if (chunk.Type != ArchiverChunkType.Padding)
				chunk.Position += chunkOrigin;
			await chunk.ToStream(this);
		}
		this._Buffer.Position = 0;
		await this._Buffer.CopyToAsync(this);
		this._HasClosed = true;
	}
	// Reading
	public bool HasChunk(ArchiverChunkType type)
	{
		foreach (var chunk in this._Chunks)
			if (chunk.Type == type)
				return true;
		return false;
	}
	public bool IsAtChunk(ArchiverChunkType type)
	{
		foreach (var chunk in this._Chunks)
			if (chunk.Type == type)
				return this.Position == chunk.Position;
		throw new ArgumentException($"Stream did not have expected {type} chunk type.");
	}
	public bool IsChunkCompressed(ArchiverChunkType type)
	{
		foreach (var chunk in this._Chunks)
			if (chunk.Type == type)
				return chunk.Compressed;
		throw new ArgumentException($"Stream did not have expected {type} chunk type.");
	}
	public uint GetChunkSize(ArchiverChunkType type)
	{
		foreach (var chunk in this._Chunks)
			if (chunk.Type == type)
				return chunk.Size;
		throw new ArgumentException($"Stream did not have expected {type} chunk type.");
	}
	public void JumpToChunk(ArchiverChunkType type)
	{
		if (!this.CanSeek) throw new ArgumentException("Expected a seekable stream");
		foreach (var chunk in this._Chunks)
		{
			if (chunk.Type == type)
			{
				#if DEBUG
					Console.WriteLine($"Stream at 0x{this.Position:X8}, jumping to chunk {type} @0x{chunk.Position:X8}");
				#endif
				this.Position = chunk.Position;
			}
		}
	}
	// Writing
	public void EndChunk()
	{
		if (this._ActiveChunk == null)
			return;
		this._ActiveChunk.Size = (uint)(this._Buffer.Position - this._ActiveChunk.Position);
		this._Chunks.Add(this._ActiveChunk);
		this._ActiveChunk = null;
	}
	public void EmptyChunk(byte version = 64)
	{
		this._Chunks.Add(new ArchiverChunk(ArchiverChunkType.Padding, version, false, 0, 0));
	}
	public Stream StartChunk(ArchiverChunkType type, byte version = 0, bool compressed = false)
	{
		if (!this.CanWrite) throw new ArgumentException("Expected a writable stream");
		this._ActiveChunk = new ArchiverChunk(type, version, compressed, (uint)this._Buffer.Position);
		return this._Buffer;
	}
	/* Static Methods */
	public static async Task<ArchiverStream> Reader(string filePath)
	{
		var bytesRead = 0;
		var buffer = new byte[SIZEOF_BUFFER];
		var reader = new FileStream(filePath, FileMode.Open, FileAccess.Read);
		while (bytesRead < buffer.Length)
			bytesRead += await reader.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
		// Header
		var magic = BitConverter.GetString(buffer, OFFSET_MAGIC, length: SIZEOF_MAGIC);
		var type = (ArchiverStreamType)BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_TYPE);
		var chunkCount = BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_CHUNKCOUNT);
		// -UNKNOWN(4)-\\
		reader.Seek(SIZEOF_PADDING, SeekOrigin.Current);
		// Chunks
		var chunks = new List<ArchiverChunk>(chunkCount);
		for (var i = 0; i < chunks.Capacity; ++i)
		{
			var chunk = await ArchiverChunk.FromStream(reader);
			#if DEBUG
				Console.WriteLine(chunk);
			#endif
			chunks.Add(chunk);
		}
		return new ArchiverStream(type, chunks, reader.SafeFileHandle);
	}
	public static async Task<ArchiverStream> Writer(string filePath, ArchiverStreamType type)
	{
		var buffer = new byte[SIZEOF_BUFFER - 2];
		var writer = new ArchiverStream(type, filePath);
		string magic;
		switch (type)
		{
			case ArchiverStreamType.Player: magic = "JJXC"; break;
			case ArchiverStreamType.Adventure:
			case ArchiverStreamType.World:
			{
				magic = "JJXM";
			} break;
			default: throw new ArgumentException($"Unknown Archiver Stream Type {type}");
		}
		BitConverter.Write(magic, buffer, OFFSET_MAGIC, length: SIZEOF_MAGIC);
		BitConverter.LittleEndian.Write((ushort)type, buffer, OFFSET_TYPE);
		await writer.WriteAsync(buffer, 0, buffer.Length);
		return writer;
	}
	/* Properties */
	public readonly ArchiverStreamType Type;
	private readonly List<ArchiverChunk> _Chunks;
	// Writing
	#nullable enable
	private ArchiverChunk? _ActiveChunk = null;
	private Stream? _Buffer = null;
	#nullable disable
	private bool _HasClosed = false;
	/* Class Properties */
	private const byte SIZEOF_BUFFER     = 8;
	private const byte OFFSET_MAGIC      = 0;
	private const byte OFFSET_TYPE       = 4;
	private const byte OFFSET_CHUNKCOUNT = 6;
	private const byte SIZEOF_MAGIC      = 4;
	private const byte SIZEOF_PADDING    = 4;
}
