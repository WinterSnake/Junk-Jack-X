/*
	Junk Jack X: Archiver
	- Stream

	Segment Breakdown:
	------------------------------------------------------------------------------------------------------------------------
	:<Archiver Header>
	Segment[0x0 : 0x3] = Magic       | Length: 2 (0x2) | Type: char[4]
	Segment[0x4 : 0x5] = File Type   | Length: 2 (0x2) | Type: enum[uint16]
	Segment[0x6 : 0x7] = Chunk Count | Length: 2 (0x2) | Type: uint16
	Segment[0x8 : 0xC] = UNKNOWN     | Length: 4 (0x4) | Type: ???
	------------------------------------------------------------------------------------------------------------------------
	Size: 12 (0xC)

	Written By: Ryan Smith
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using JJx.Utilities;

namespace JJx;

internal enum ArchiverType : byte
{
	Unknown,
	Player,
	Adventure,
	Map,
	Stat
}

#nullable enable
internal sealed class ArchiverStream : FileStream
{
	/* Constructors */
	private ArchiverStream(string path, FileMode mode, FileAccess access): base(path, mode, access) {}
	/* Instance Methods */
	// Override
	public override void Close()
	{
		if (this.CanWrite)
		{
			var workingData = new byte[SIZEOF_HEADER - 2];
			// Write Chunk Count
			var chunkCount = (ushort)this._WriteChunks!.Count;
			Utilities.ByteConverter.Write(new Span<byte>(workingData), chunkCount, 0);
			// Add UNKNOWN | Padding
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (uint)0, 2);
			this.Write(workingData, 0, workingData.Length);
			// Calculate Origin
			var origin = (uint)(SIZEOF_HEADER + 4 + (this._WriteChunks!.Count * Chunk.SIZE));
			// Write chunks
			foreach (var chunk in this._WriteChunks!)
				chunk.ToStream(this, origin);
			// Write buffer
			this._Buffer!.Position = 0;
			this._Buffer!.CopyTo(this);
			this._Buffer!.Dispose();
			this._Buffer = null;
		}
		base.Close();
	}
	public string[]? GetChunkStrings()
	{
		if (!this.CanRead || this._Chunks == null)
			return null;
		string[] chunkStrings = new string[this._Chunks.Length];
		for (var i = 0; i < this._Chunks.Length; ++i)
			chunkStrings[i] = this._Chunks[i].ToString();
		return chunkStrings;
	}
	// Reading
	public bool IsAtChunk(Chunk.Type type)
	{
		if (!this.CanRead || this._Chunks == null)
			return false;
		foreach (var chunk in this._Chunks)
			if (chunk.Id == type && this.Position == chunk.Location)
				return true;
		return false;
	}
	public bool HasChunk(Chunk.Type type)
	{
		if (!this.CanRead || this._Chunks == null)
			return false;
		foreach (var chunk in this._Chunks)
			if (chunk.Id == type)
				return true;
		return false;
	}
	public uint? GetChunkSize(Chunk.Type type)
	{
		if (!this.CanRead || this._Chunks == null)
			return null;
		foreach (var chunk in this._Chunks)
			if (chunk.Id == type)
				return chunk.Size;
		return null;
	}
	public void SeekToChunk(Chunk.Type type)
	{
		if (!this.CanRead || this._Chunks == null)
			return;
		foreach (var chunk in this._Chunks)
			if (chunk.Id == type)
				this.Position = chunk.Location;
	}
	// Writing
	public WritableChunk NewChunk(Chunk.Type type, byte version = 0, bool compressed = false) => new WritableChunk(type, version, compressed, this._Buffer, this._WriteChunks!.Add);
	/* Static Methods */
	public static async Task<ArchiverStream> Reader(string path)
	{
		var reader = new ArchiverStream(path, FileMode.Open, FileAccess.Read);
		int bytesRead = 0;
		var workingData = new byte[SIZEOF_HEADER];
		while (bytesRead < SIZEOF_HEADER)
			bytesRead += await reader.ReadAsync(workingData, bytesRead, SIZEOF_HEADER - bytesRead);
		/// Header
		var headerMagic = ByteConverter.GetString(new Span<byte>(workingData), 0, 4);
		var headerId    = ByteConverter.GetUInt16(new Span<byte>(workingData), 4);
		var chunkCount  = ByteConverter.GetUInt16(new Span<byte>(workingData), 6);
		// Type
		if (headerMagic == "JJXC" && headerId == 0)
			reader.Type = ArchiverType.Player;
		else if (headerMagic == "JJXM")
		{
			if (headerId == 1)
				reader.Type = ArchiverType.Map;
			else if (headerId == 2)
				reader.Type = ArchiverType.Adventure;
		}
		// =UNKNOWN=
		reader.Seek(4, SeekOrigin.Current);
		// Chunk
		var chunks = new Chunk[chunkCount];
		for (var i = 0; i < chunkCount; ++i)
			chunks[i] = await Chunk.FromStream(reader);
		reader._Chunks = chunks;
		return reader;
	}
	public static async Task<ArchiverStream> Writer(string path, ArchiverType type)
	{
		var writer = new ArchiverStream(path, FileMode.Create, FileAccess.Write);
		writer._Buffer = new MemoryStream(1024);
		writer._WriteChunks = new List<Chunk>();
		var workingData = new byte[SIZEOF_HEADER - 2];
		switch (type)
		{
			case ArchiverType.Player:
			{
				Utilities.ByteConverter.Write(new Span<byte>(workingData), "JJXC", length: 4);
				Utilities.ByteConverter.Write(new Span<byte>(workingData), (ushort)0, 4);
			} break;
			case ArchiverType.Map:
			{
				Utilities.ByteConverter.Write(new Span<byte>(workingData), (ushort)1, 4);
				goto World;
			}
			case ArchiverType.Adventure:
			{
				Utilities.ByteConverter.Write(new Span<byte>(workingData), (ushort)2, 4);
				goto World;
			}
			default:
			{
				throw new ArgumentException($"Unsupported writer format '{type}'");
			}
			World:
			{
				Utilities.ByteConverter.Write(new Span<byte>(workingData), "JJXM", length: 4);
			} break;
		}
		await writer.WriteAsync(workingData, 0, workingData.Length);
		return writer;
	}
	/* Properties */
	public ArchiverType Type { get; private set; } = ArchiverType.Unknown;
	// Read
	private Chunk[]? _Chunks = null;
	// Write
	private MemoryStream? _Buffer = null;
	private List<Chunk>? _WriteChunks = null;
	/* Class Properties */
	private const byte SIZEOF_HEADER = 8;
}
#nullable disable
