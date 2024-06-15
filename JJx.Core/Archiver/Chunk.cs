/*
	Junk Jack X: Core
	- [Archiver]Chunk

	Segment Breakdown:
	----------------------------------------------------------------------------------------------------
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
using System.IO;
using System.Threading.Tasks;

namespace JJx;

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
	WorldEntities      = 0x0013,
	// Adventure
	// Player
	PlayerInfo         = 0x8000,
	PlayerItems        = 0x8001,
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
