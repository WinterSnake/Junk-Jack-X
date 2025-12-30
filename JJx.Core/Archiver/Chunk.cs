/*
	Junk Jack X: Core
	- [Archiver]Chunk

	Segment Breakdown:
	--------------------------------------------------------------------------------------------------
	Segment[0x0 : 0x1] = Type       | Length: 2 (0x2) | Type: enum[uint16] | Parent: ArchiverChunkType
	Segment[0x2]       = Version    | Length: 1 (0x1) | Type: uint8
	Segment[0x3]       = Compressed | Length: 1 (0x1) | Type: bool
	Segment[0x4 : 0x7] = Location   | Length: 4 (0x4) | Type: uint32
	Segment[0x8 : 0xC] = Size       | Length: 4 (0x4) | Type: uint32
	--------------------------------------------------------------------------------------------------
	Size: 12 (0xC)

	Written By: Ryan Smith
*/

namespace JJx.Core;

public enum ArchiverChunkType : ushort
{
	Padding            = 0x0000,
	// World
	WorldInfo          = 0x0001,
	WorldBlocks        = 0x0002,
	WorldFog           = 0x0003,
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
	AdventurePortals   = 0x0014,
	// Player
	PlayerInfo         = 0x8000,
	PlayerItems        = 0x8001,
	PlayerCraftbooks   = 0x8002,
	PlayerAchievements = 0x8003,
	PlayerStatus       = 0x8004,
}

internal record struct ArchiverChunk
{
	/* Properties */
	public ArchiverChunkType Type;
	public byte Version;
	public bool IsCompressed;
	public int Offset;
	public int Length;
}
