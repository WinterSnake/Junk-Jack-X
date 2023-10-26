/*
 *
	Junk Jack X: Archiver
	- Chunk

	Segment Breakdown:
	----------------------------------------------------------------------
	:<Chunk>
	Segment[0x0 : 0x1] = Type       | Length: 2 (0x2) | Type: enum[uint16]
	Segment[0x2]       = Version    | Length: 1 (0x1) | Type: uint8
	Segment[0x3]       = Compressed | Length: 1 (0x1) | Type: bool
	Segment[0x4 : 0x7] = Location   | Length: 4 (0x4) | Type: uint32
	Segment[0x8 : 0xC] = Size       | Length: 4 (0x4) | Type: uint32
	----------------------------------------------------------------------
	Size: 12 (0xC)

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public enum ChunkType : ushort
{
	Padding            = 0x0000,  // Used in Creative/Flat maps to fill 19th chunk
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
	WorldPlantFruits   = 0x000C,
	WorldPlantDecay    = 0x000D,
	WorldLocks         = 0x000E,
	WorldFluid         = 0x000F,
	WorldCircuitry     = 0x0010,
	WorldWeather       = 0x0011,
	WorldTime          = 0x0012,
	WorldMobs          = 0x0013,
	// Adventure
	AdventurePortals   = 0x0014,
	// Player
	PlayerInfo         = 0x8000,
	PlayerInventory    = 0x8001,
	PlayerCraftbook    = 0x8002,
	PlayerAchievements = 0x8003,
	PlayerStatus       = 0x8004,
}

internal struct Chunk
{
	/* Constructors */
	public Chunk(ChunkType type, byte version, bool compressed, uint position, uint size)
	{
		this.Type = type;
		this.Version = version;
		this.Compressed = compressed;
		this.Position = position;
		this.Size = size;
	}
	/* Instance Methods */
	public override string ToString()
	{
		return $"Type: {this.Type} | Version: {this.Version} | Compressed: {this.Compressed} | Position: 0x{this.Position:X4} | Size: 0x{this.Size:X4}";
	}
	/* Static Methods */
	public static async Task<Chunk> FromStream(Stream stream)
	{
		int bytesRead = 0;
		var workingData = new byte[SIZE];
		while (bytesRead < SIZE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZE - bytesRead);
		var type       = (ChunkType)BitConverter.GetUInt16(workingData, 0);
		var version    = BitConverter.GetUInt8(workingData, 2);
		var compressed = BitConverter.GetBool(workingData, 3);
		var position   = BitConverter.GetUInt32(workingData, 4);
		var size       = BitConverter.GetUInt32(workingData, 8);
		return new Chunk(type, version, compressed, position, size);
	}
	/* Properties */
	public readonly ChunkType Type;
	public readonly byte Version;
	public readonly bool Compressed;
	public readonly uint Position;
	public readonly uint Size;
	/* Class Properties */
	internal const byte SIZE = 12;
}
