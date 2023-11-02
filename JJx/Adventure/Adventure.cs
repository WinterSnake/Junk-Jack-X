/*
	Junk Jack X: Adventure

	Segment Breakdown:
	---------------------------------------------------------------------------------------------------------------------
	:<Info>
	Segment[0x24 :  0x33] = UUID                  | Length: 16  (0x10) | Type: uuid
	Segment[0x34 :  0x37] = Last Played Timestamp | Length: 4    (0x4) | Type: DateTime[uint32]
	Segment[0x38 :  0x3B] = Game Version          | Length: 4    (0x4) | Type: uint32             | Parent: JJx.Version
	Segment[0x3C :  0x5B] = Name                  | Length: 32  (0x20) | Type: char*
	Segment[0x5C :  0x6B] = Author                | Length: 16  (0x10) | Type: char*
	Segment[0x6C :  0x6D] = World.Width           | Length: 2    (0x2) | Type: uint16             | Parent: Blocks.Width
	Segment[0x6E :  0x6F] = World.Height          | Length: 2    (0x2) | Type: uint16             | Parent: Blocks.Height
	Segment[0x70 :  0x77] = Padding               | Length: 8    (0x8) | Type: uint64
	Segment[0x78 :  0x7B] = Planet                | Length: 4    (0x4) | Type: enum flag[uint32]
	Segment[0x7C]         = Season                | Length: 1    (0x1) | Type: enum[uint8]        | Parent: Season
	Segment[0x7D]         = Gamemode              | Length: 1    (0x1) | Type: enum[uint8]        | Parent: Gamemode
	Segment[0x7E]         = World Size            | Length: 1    (0x1) | Type: enum[uint8]        | Parent: GenSize
	Segment[0x7F]         = Sky Size              | Length: 1    (0x1) | Type: enum[uint8]        | Parent: GenSize
	Segment[0x80 :  0x83] = Language              | Length: 4    (0x4) | Type: char[4]
	Segment[0x84 : 0x103] = Padding               | Length: 128 (0x80) | Type: uint32[32] = {0}
	:<Portals>
	Segment[0x104 : 0x107] = Portal Count         | Length: 4    (0x4) | Type: uint32
		Portal[]
	---------------------------------------------------------------------------------------------------------------------

	Written By: Ryan Smith
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Adventure
{
	/* Constructors */
	public Adventure()
	{

	}
	private Adventure(Portal[] portals)
	{
		this.Portals = new List<Portal>(portals);
	}
	/* Instance Methods */
	public async Task Save(string filePath)
	{
		var workingData = new byte[SIZEOF_BUFFER];
		using var stream = await ArchiverStream.Writer(filePath, ArchiverType.World);
		/// Info
		var adventureInfo = stream.StartChunk(ChunkType.WorldInfo);
			// {UUID | Last Played | Version}
			// -UUID
			// -Last Played
			// -Version
			// Name
			// Author
			// {World Size | Player Location | Spawn Location | Planet | Season | Gamemode | Init GenSize | Sky GenSize | Language}
			// -World Size
			// -Player Location
			// -Spawn Location
			// -Planet
			// -Season
			// -Gamemode
			// -Init Gen Size
			// -Sky Gen Size
			// -Language
			// Padding
		stream.EndChunk();
		/// Portals
		var adventurePortals = stream.StartChunk(ChunkType.AdventurePortals);
			BitConverter.Write(workingData, (uint)this.Portals.Count);
			await adventurePortals.WriteAsync(workingData, 0, SIZEOF_TILECOUNT);
			foreach (var portal in this.Portals)
				await portal.ToStream(adventurePortals);
		stream.EndChunk();
	}
	/* Static Methods */
	public static async Task<Adventure> Load(string filePath) => await Adventure.FromStream(await ArchiverStream.Reader(filePath));
	public static async Task<Adventure> FromStream(ArchiverStream stream)
	{
		if (stream.Type != ArchiverType.Adventure && stream.CanRead)
			throw new ArgumentException($"Expected adventure stream, found {stream.Type} stream");
		int bytesRead = 0;
		var workingData = new byte[SIZEOF_BUFFER];
		/// Info
		if (!stream.AtChunk(ChunkType.WorldInfo))
			stream.JumpToChunk(ChunkType.WorldInfo);
		// {UUID | Last Played | Version}
		int _size = SIZEOF_UUID + SIZEOF_TIMESTAMP + SIZEOF_VERSION;
		while (bytesRead < _size)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, _size - bytesRead);
		// -UUID
		var id = new Guid(new Span<byte>(workingData, 0, SIZEOF_UUID));
		// -Last Played
		var lastPlayed = BitConverter.GetDateTime(workingData, 16);
		// -Version
		var version = (Version)BitConverter.GetUInt32(workingData, 20);
		// Name
		bytesRead = 0;
		while (bytesRead < SIZEOF_NAME)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_NAME - bytesRead);
		var name = BitConverter.GetString(workingData);
		// Author
		bytesRead = 0;
		while (bytesRead < SIZEOF_AUTHOR)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_AUTHOR - bytesRead);
		var author = BitConverter.GetString(workingData);
		// {World Size | Player Location | Spawn Location | Planet | Season | Gamemode | Init GenSize | Sky GenSize | Language}
		bytesRead = 0;
		_size = SIZEOF_POSITIONS + SIZEOF_PLANET + SIZEOF_FLAGS + SIZEOF_LANGUAGE;
		while (bytesRead < _size)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, _size - bytesRead);
		// -World Size
		(ushort width, ushort height) worldSize = (
			BitConverter.GetUInt16(workingData,  0),
			BitConverter.GetUInt16(workingData,  2)
		);
		// -Player Location
		(ushort x, ushort y) playerLocation = (
			BitConverter.GetUInt16(workingData,  4),
			BitConverter.GetUInt16(workingData,  6)
		);
		// -Spawn Location
		(ushort x, ushort y) spawnLocation = (
			BitConverter.GetUInt16(workingData,  8),
			BitConverter.GetUInt16(workingData, 10)
		);
		// -Planet
		var planet = (Planet)BitConverter.GetUInt32(workingData, 12);
		// -Season
		var season = (Season)workingData[16];
		// -Gamemode
		var gamemode = (Gamemode)workingData[17];
		// -Init GenSize
		var initSize = (GenSize)workingData[18];
		// -Sky GenSize
		var skySize = (GenSize)workingData[19];
		// -Language
		var language = BitConverter.GetString(workingData, 20);
		// Padding
		stream.Position += SIZEOF_PADDING;
		/// Portals
		if (!stream.AtChunk(ChunkType.AdventurePortals))
			stream.JumpToChunk(ChunkType.AdventurePortals);
		while (bytesRead < SIZEOF_TILECOUNT)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TILECOUNT - bytesRead);
		var portalCount = BitConverter.GetUInt32(workingData, 12);
		var portals = new Portal[portalCount];
		for (var i = 0; i < portals.Length; ++i)
			portals[i] = await Portal.FromStream(stream);
		return new Adventure(portals);
	}
	/* Properties */
	public List<Portal> Portals = new List<Portal>();
	/* Class Properties */
	private const byte SIZEOF_BUFFER    =    32;
	private const byte SIZEOF_UUID      =    16;
	private const byte SIZEOF_TIMESTAMP =     4;
	private const byte SIZEOF_VERSION   =     4;
	private const byte SIZEOF_NAME      =    32;
	private const byte SIZEOF_AUTHOR    =    16;
	private const byte SIZEOF_POSITIONS = 2 * 6;  // World.(Width, Height), Player.(X, Y), Spawn.(X, Y)
	private const byte SIZEOF_PLANET    =     4;
	private const byte SIZEOF_FLAGS     =     4;
	private const byte SIZEOF_LANGUAGE  =     4;
	private const byte SIZEOF_PADDING   =   128;
	private const byte SIZEOF_TILECOUNT =     4;
}
