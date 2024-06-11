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
		this._Chunks = chunks;
	}
	private ArchiverStream(ArchiverStreamType type, string filePath): base(filePath, FileMode.Create, FileAccess.Write)
	{
		this.Type = type;
		this._Chunks = new List<ArchiverChunk>();
		this._Buffer = new BufferedStream(this);
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
		uint chunkOrigin = (uint)(SIZEOF_BUFFER + SIZEOF_PADDING + (this._Chunks.Count * ArchiverChunk.SIZE) - this.Position);
		// Chunk Count + UNKNOWN (SIZE=4)
		var buffer = new byte[ArchiverStream.SIZEOF_BUFFER - 2];
		BitConverter.LittleEndian.Write((ushort)this._Chunks.Count, buffer);
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
		await this._Buffer.FlushAsync();
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
	public BufferedStream StartChunk(ArchiverChunkType type, byte version = 0, bool compressed = false)
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
	private BufferedStream? _Buffer = null;
	private bool _HasClosed = false;
	#nullable disable
	/* Class Properties */
	private const byte SIZEOF_BUFFER     = 8;
	private const byte OFFSET_MAGIC      = 0;
	private const byte OFFSET_TYPE       = 4;
	private const byte OFFSET_CHUNKCOUNT = 6;
	private const byte SIZEOF_MAGIC      = 4;
	private const byte SIZEOF_PADDING    = 4;
}

public enum ArchiverChunkType : ushort
{
	Padding            = 0x0000,
	// World
	WorldInfo          = 0x0001,
	WorldBlocks        = 0x0002,
	WorldFog           = 0x0003,  // WorldManager.cpp line: 487
	WorldSkyline       = 0x0004,
	WorldChests        = 0x0005,
	WorldForges        = 0x0006,
	WorldStables       = 0x0007,
	WorldLabs          = 0x0008,
	WorldSigns         = 0x0009,
	WorldShelves       = 0x000A,
	WorldPlants        = 0x000B,
	WorldFruits        = 0x000C,
	WorldPlantDecay    = 0x000D,
	WorldLocks         = 0x000E,
	WorldFluid         = 0x000F,
	WorldCircuitry     = 0x0010,
	WorldWeather       = 0x0011,
	WorldTime          = 0x0012,
	WorldMobs          = 0x0013,
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
	public ArchiverChunk(ArchiverChunkType type, byte version, bool compressed, uint position, uint size = 0)
	{
		this.Type = type;
		this.Version = version;
		this.Compressed = compressed;
		this.Position = position;
		this.Size = size;
	}
	/* Instance Methods */
	public override string ToString() => $"Chunk{{Type: {this.Type} | Version: {this.Version} | Compressed: {this.Compressed} | Position: 0x{this.Position:X8} | Size: 0x{this.Size:X4}}}";
	public async Task ToStream(Stream stream)
	{
		var buffer = new byte[SIZE];
		BitConverter.LittleEndian.Write((ushort)this.Type, buffer, OFFSET_TYPE);
		buffer[OFFSET_VERSION] = this.Version;
		BitConverter.Write(this.Compressed, buffer, OFFSET_COMPRESSEDFLAG);
		BitConverter.LittleEndian.Write(this.Position, buffer, OFFSET_POSITION);
		BitConverter.LittleEndian.Write(this.Size, buffer, OFFSET_SIZE);
		await stream.WriteAsync(buffer, 0, buffer.Length);
	}
	/* Static Methods */
	public static async Task<ArchiverChunk> FromStream(Stream stream)
	{
		int bytesRead = 0;
		var buffer = new byte[SIZE];
		while (bytesRead < buffer.Length)
			bytesRead += await stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
		var type       = (ArchiverChunkType)BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_TYPE);
		var version    = buffer[OFFSET_VERSION];
		var compressed = BitConverter.GetBool(buffer, OFFSET_COMPRESSEDFLAG);
		var position   = BitConverter.LittleEndian.GetUInt32(buffer, OFFSET_POSITION);
		var size       = BitConverter.LittleEndian.GetUInt32(buffer, OFFSET_SIZE);
		return new ArchiverChunk(type, version, compressed, position, size);
	}
	/* Properties */
	public readonly ArchiverChunkType Type;
	public readonly byte Version;
	public readonly bool Compressed;
	public uint Position;
	public uint Size;
	/* Class Properties */
	internal const byte SIZE                 = 12;
	private const byte OFFSET_TYPE           =  0;
	private const byte OFFSET_VERSION        =  2;
	private const byte OFFSET_COMPRESSEDFLAG =  3;
	private const byte OFFSET_POSITION       =  4;
	private const byte OFFSET_SIZE           =  8;
}
