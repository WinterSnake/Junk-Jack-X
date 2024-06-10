/*
	Junk Jack X: Core
	- Archiver Stream+Chunk

	Segment Breakdown:
	----------------------------------------------------------------------------------------------------
	:<Header>
	Segment[0x0 : 0x3] = Magic       | Length: 4 (0x4) | Type: chr[4]
	Segment[0x4 : 0x5] = File Type   | Length: 2 (0x2) | Type: enum[uint16] | Parent: ArchiverStreamType
	Segment[0x6 : 0x7] = Chunk Count | Length: 2 (0x2) | Type: uint16
	Segment[0x8 : 0xB] = UNKNOWN     | Length: 4 (0x4) | Type: ???
	----------------------------------------------------------------------------------------------------
	Size: 12 (0xC)
	----------------------------------------------------------------------------------------------------
	:<Chunk>
	Segment[0x0 : 0x1] = Type       | Length: 2 (0x2) | Type: enum[uint16] | Parent: ArchiverChunkType
	Segment[0x2]       = Version    | Length: 1 (0x1) | Type: uint8
	Segment[0x3]       = Compressed | Length: 1 (0x1) | Type: bool
	Segment[0x4 : 0x7] = Location   | Length: 4 (0x4) | Type: uint32
	Segment[0x8 : 0xC] = Size       | Length: 4 (0x4) | Type: uint32
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
		this.Chunks = chunks;
	}
	/* Instance Methods */
	public bool IsAtChunk(ArchiverChunkType type)
	{
		foreach (var chunk in this.Chunks)
			if (chunk.Type == type)
				return this.Position == chunk.Position;
		throw new ArgumentException($"Stream did not have expected {type} chunk type.");
	}
	public bool IsChunkCompressed(ArchiverChunkType type)
	{
		foreach (var chunk in this.Chunks)
			if (chunk.Type == type)
				return chunk.Compressed;
		throw new ArgumentException($"Stream did not have expected {type} chunk type.");
	}
	public void JumpToChunk(ArchiverChunkType type)
	{
		if (!this.CanSeek) throw new ArgumentException("Expected a seekable stream");
		foreach (var chunk in this.Chunks)
		{
			if (chunk.Type == type)
			{
				#if DEBUG
					Console.WriteLine($"Stream at 0x{this.Position:X4}, jumping to chunk {type} @0x{chunk.Position:X4}");
				#endif
				this.Position = chunk.Position;
			}
		}
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
			var chunk = await ArchiverChunk.FromStream(reader);
			#if DEBUG
				Console.WriteLine(chunk);
			#endif
			chunks.Add(chunk);
		}
		return new ArchiverStream(type, chunks, reader.SafeFileHandle);
	}
	/* Properties */
	public readonly ArchiverStreamType Type;
	private readonly List<ArchiverChunk> Chunks;
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
	public static async Task<ArchiverChunk> FromStream(Stream stream)
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
