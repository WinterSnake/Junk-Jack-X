/*
	Junk Jack X: Core
	- Archiver Stream+Chunk

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
		this.Chunks = chunks;
	}
	/* Instance Methods */
	// Reading
	public bool IsAtChunk(ArchiverChunkType type)
	{
		if (!this.CanRead) throw new ArgumentException("Expected a readable stream");
		foreach (var chunk in this.Chunks)
			if (chunk.Type == type)
				return this.Position == chunk.Position;
		throw new KeyNotFoundException($"Expected {type} chunk but found none.");
	}
	public bool IsChunkCompressed(ArchiverChunkType type)
	{
		if (!this.CanRead) throw new ArgumentException("Expected a readable stream");
		foreach (var chunk in this.Chunks)
			if (chunk.Type == type)
				return chunk.Compressed;
		throw new KeyNotFoundException($"Expected {type} chunk but found none.");
	}
	public void JumpToChunk(ArchiverChunkType type)
	{
		if (!this.CanRead) throw new ArgumentException("Expected a readable stream");
		foreach (var chunk in this.Chunks)
			if (chunk.Type == type)
				this.Position = chunk.Position;
	}
	public uint GetChunkPosition(ArchiverChunkType type)
	{
		if (!this.CanRead) throw new ArgumentException("Expected a readable stream");
		foreach (var chunk in this.Chunks)
			if (chunk.Type == type)
				return chunk.Position;
		throw new KeyNotFoundException($"Expected {type} chunk but found none.");
	}
	public uint GetChunkSize(ArchiverChunkType type)
	{
		if (!this.CanRead) throw new ArgumentException("Expected a readable stream");
		foreach (var chunk in this.Chunks)
			if (chunk.Type == type)
				return chunk.Size;
		throw new KeyNotFoundException($"Expected {type} chunk but found none.");
	}
	/* Static Methods */
	public static async Task<ArchiverStream> Reader(string filePath)
	{
		var bytesRead = 0;
		var buffer = new byte[ArchiverStream.SIZEOF_HEADER];
		var reader = new FileStream(filePath, FileMode.Open, FileAccess.Read);
		while (bytesRead < buffer.Length)
			bytesRead += await reader.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
		// Header
		var magic = JJx.BitConverter.GetString(buffer, ArchiverStream.OFFSET_MAGIC, length: ArchiverStream.SIZEOF_MAGIC);
		var type = (ArchiverStreamType)JJx.BitConverter.LittleEndian.GetUInt16(buffer, ArchiverStream.OFFSET_TYPE);
		var chunkCount = JJx.BitConverter.LittleEndian.GetUInt16(buffer, ArchiverStream.OFFSET_CHUNKCOUNT);
		// Chunks
		reader.Seek(SIZEOF_PADDING, SeekOrigin.Current);
		var chunks = new List<ArchiverChunk>(chunkCount);
		for (var i = 0; i < chunks.Capacity; ++i)
		{
			var chunk = await ArchiverChunk.Deserialize(reader);
			#if DEBUG
				Console.WriteLine(chunk);
			#endif
			chunks.Add(chunk);
		}
		return new ArchiverStream(type, chunks, reader.SafeFileHandle);
	}
	/* Properties */
	public readonly ArchiverStreamType Type;
	internal readonly List<ArchiverChunk> Chunks;
	/* Class Properties */
	private const byte OFFSET_MAGIC      = 0;
	private const byte OFFSET_TYPE       = 4;
	private const byte OFFSET_CHUNKCOUNT = 6;
	private const byte SIZEOF_HEADER     = 8;
	private const byte SIZEOF_MAGIC      = 4;
	private const byte SIZEOF_PADDING    = 4;
}

public enum ArchiverChunkType : ushort
{
	Padding = 0x0000,
	// World
	// Adventure
	// Player
	PlayerInfo         = 0x8000,
	PlayerInventory    = 0x8001,
	PlayerCraftbooks   = 0x8002,
	PlayerAchievements = 0x8003,
	PlayerStatus       = 0x8004,
}

internal sealed class ArchiverChunk
{
	/* Constructors */
	public ArchiverChunk(ArchiverChunkType type, byte version, bool compressed, uint position, uint size)
	{
		this.Type = type;
		this.Version = version;
		this.Compressed = compressed;
		this.Position = position;
		this.Size = size;
	}
	/* Instance Methods */
	public override string ToString() => $"Chunk{{Type: {this.Type} | Version: {this.Version} | Compressed: {this.Compressed} | Position: 0x{this.Position:X4} | Size: 0x{this.Size:X4}}}";
	/* Static Methods */
	public static async Task<ArchiverChunk> Deserialize(Stream stream)
	{
		int bytesRead = 0;
		var buffer = new byte[ArchiverChunk.SIZE];
		while (bytesRead < buffer.Length)
			bytesRead += await stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
		var type       = (ArchiverChunkType)JJx.BitConverter.LittleEndian.GetUInt16(buffer, ArchiverChunk.OFFSET_TYPE);
		var version    = buffer[ArchiverChunk.OFFSET_VERSION];
		var compressed = JJx.BitConverter.GetBool(buffer, ArchiverChunk.OFFSET_COMPRESSEDFLAG);
		var position   = JJx.BitConverter.LittleEndian.GetUInt32(buffer, ArchiverChunk.OFFSET_POSITION);
		var size       = JJx.BitConverter.LittleEndian.GetUInt32(buffer, ArchiverChunk.OFFSET_SIZE);
		return new ArchiverChunk(type, version, compressed, position, size);
	}
	/* Properties */
	public readonly ArchiverChunkType Type;
	public readonly byte Version;
	public readonly bool Compressed;
	public uint Position;
	public uint Size;
	/* Class Properties */
	private const byte OFFSET_TYPE           =  0;
	private const byte OFFSET_VERSION        =  2;
	private const byte OFFSET_COMPRESSEDFLAG =  3;
	private const byte OFFSET_POSITION       =  4;
	private const byte OFFSET_SIZE           =  8;
	private const byte SIZE                  = 12;
}
