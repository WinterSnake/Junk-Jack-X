/*
	Junk Jack X: Archiver
	- Stream

	Written By: Ryan Smith
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace JJx;

public enum ArchiverType : ushort
{
	Invalid   = 0x00,
	Player    = 0x01,
	World     = 0x02,
	Adventure = 0x03,
}

public sealed class ArchiverStream : FileStream
{
	/* Constructors */
	// Reader
	private ArchiverStream(SafeFileHandle readerHandle, ArchiverType type, List<Chunk> chunks): base(readerHandle, FileAccess.Read)
	{
		this.Type = type;
		this.Chunks = chunks;
	}
	private ArchiverStream(string filePath, ArchiverType type): base(filePath, FileMode.Create, FileAccess.Write)
	{
		this.Type = type;
		this._Buffer = new MemoryStream();
	}
	/* Instance Methods */
	public override string ToString()
	{
		string[] chunks = new string[this.Chunks.Count];
		for (var i = 0; i < this.Chunks.Count; ++i)
			chunks[i] = '[' + this.Chunks[i].ToString() + ']';
		return $"Type: {this.Type} | Chunks: {String.Join(" ; ", chunks)}";
	}
	public override void Close()
	{
		if (this.CanWrite)
		{
			var workingData = new byte[SIZEOF_HEADER - 2];
			// Chunk Count | Padding
			JJx.BitConverter.Write(workingData, (uint)this.Chunks.Count, 0);
			this.Write(workingData, 0, workingData.Length);
			// Chunks
			workingData = new byte[Chunk.SIZE];
			var origin = SIZEOF_HEADER + SIZEOF_PADDING + (this.Chunks.Count * Chunk.SIZE);
			foreach (var chunk in this.Chunks)
			{
				BitConverter.Write(workingData, (ushort)chunk.Type,              0);
				BitConverter.Write(workingData, chunk.Version,                   2);
				BitConverter.Write(workingData, chunk.Compressed,                3);
				BitConverter.Write(workingData, (uint)(chunk.Position + origin), 4);
				BitConverter.Write(workingData, chunk.Size,                      8);
				this.Write(workingData, 0, workingData.Length);

			}
			// Buffer
			this._Buffer!.Position = 0;
			this._Buffer!.CopyTo(this);
		}
		base.Close();
	}
	protected override void Dispose(bool disposing)
	{
		if (disposing && this.CanWrite)
			this._Buffer!.Dispose();
		base.Dispose(disposing);
	}
	// Reading
	public bool AtChunk(ChunkType type)
	{
		if (!this.CanRead) return false;
		foreach (var chunk in this.Chunks)
		{
			if (chunk.Type == type)
				return this.Position == chunk.Position;
		}
		return false;
	}
	public void JumpToChunk(ChunkType type)
	{
		foreach (var chunk in this.Chunks)
			if (chunk.Type == type)
				this.Position = chunk.Position;
	}
	// Writing
	public Stream StartChunk(ChunkType type, byte version = 0, bool compressed = false)
	{
		this._ActiveChunk = new Chunk(type, version, compressed, (uint)this._Buffer.Position, 0);
		return this._Buffer;
	}
	public void EndChunk()
	{
		if (!this._ActiveChunk.HasValue)
			return;
		var curChunk = this._ActiveChunk.Value;
		var endChunk = new Chunk(
			curChunk.Type, curChunk.Version, curChunk.Compressed,
			curChunk.Position, (uint)(this._Buffer.Position - curChunk.Position)
		);
		this.Chunks.Add(endChunk);
		this._ActiveChunk = null;
	}
	/* Static Methods */
	// Reading
	public static async Task<ArchiverStream> Reader(string filePath)
	{
		var reader = new FileStream(filePath, FileMode.Open, FileAccess.Read);
		int bytesRead = 0;
		var workingData = new byte[SIZEOF_HEADER];
		while (bytesRead < SIZEOF_HEADER)
			bytesRead += await reader.ReadAsync(workingData, bytesRead, SIZEOF_HEADER - bytesRead);
		/// Header
		var type = ArchiverType.Invalid;
		var magic = JJx.BitConverter.GetString(workingData, 0, length: 4);
		var id = JJx.BitConverter.GetUInt16(workingData, 4);
		var chunkCount = JJx.BitConverter.GetUInt16(workingData, 6);
		// Type
		switch (id)
		{
			case 0:
			{
				type = ArchiverType.Player;
			} break;
			case 1:
			{
				type = ArchiverType.World;
			} break;
			case 2:
			{
				type = ArchiverType.Adventure;
			} break;
		}
		if (type == ArchiverType.Invalid)
			throw new ArgumentException($"'{filePath}' has invalid JJx format; Magic: {magic}, Id: {id}");
		/// Chunks
		reader.Seek(SIZEOF_PADDING, SeekOrigin.Current);
		var chunks = new List<Chunk>(chunkCount);
		for (var i = 0; i < chunkCount; ++i)
		{
			var chunk = await Chunk.FromStream(reader);
			chunks.Add(chunk);
		}
		return new ArchiverStream(reader.SafeFileHandle, type, chunks);
	}
	// Writing
	public static async Task<ArchiverStream> Writer(string filePath, ArchiverType type)
	{
		if (type == ArchiverType.Invalid)
			throw new ArgumentException("Cannot create ArchiverType.Invalid ArchiverStream");
		var writer = new ArchiverStream(filePath, type);
		var workingData = new byte[SIZEOF_HEADER - 2];
		switch (type)
		{
			case ArchiverType.Player:
			{
				JJx.BitConverter.Write(workingData, "JJXC", length: 4);
				JJx.BitConverter.Write(workingData, (ushort)0, 4);
			} break;
			case ArchiverType.World:
			{
				JJx.BitConverter.Write(workingData, (ushort)1, 4);
				goto Map;
			}
			case ArchiverType.Adventure:
			{
				JJx.BitConverter.Write(workingData, (ushort)2, 4);
				goto Map;
			}
			Map:
			{
				JJx.BitConverter.Write(workingData, "JJXM", length: 4);
			} break;
		}
		await writer.WriteAsync(workingData, 0, workingData.Length);
		return writer;
	}
	/* Properties */
	public readonly ArchiverType Type;
	private readonly List<Chunk> Chunks = new List<Chunk>();
	// Writing
	#nullable enable
	private Chunk? _ActiveChunk = null;
	private readonly MemoryStream? _Buffer = null;
	#nullable disable
	/* Class Properties */
	private const byte SIZEOF_HEADER  = 8;
	private const byte SIZEOF_PADDING = 4;
}
