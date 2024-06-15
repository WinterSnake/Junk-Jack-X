/*
	Junk Jack X: Core
	- Adventure

	Segment Breakdown:
	-----------------------------------------------------------------------------------------------------------------------------
	:<Info>
	Segment[0xF0  :   0xFF] = UUID                 | Length:  16 (0x10) | Type: uuid
	Segment[0x100 :  0x103] = Last Played Datetime | Length:   4  (0x4) | Type: uint             | Parent: DateTime
	Segment[0x104 :  0x107] = Game Version         | Length:   4  (0x4) | Type: enum[uint32]     | Parent: JJx.Version
	Segment[0x108 :  0x127] = Name                 | Length:  32 (0x20) | Type: char*
	Segment[0x128 :  0x137] = Author               | Length:  16 (0x10) | Type: char*
	Segment[0x138 :  0x139] = World.Width          | Length:   2  (0x2) | Type: uint16
	Segment[0x13A :  0x13B] = World.Height         | Length:   2  (0x2) | Type: uint16t
	Segment[0x13C :  0x13D] = NO-OP[Player.X]      | Length:   2  (0x2) | Type: uint16
	Segment[0x13E :  0x13F] = NO-OP[Player.Y]      | Length:   2  (0x2) | Type: uint16
	Segment[0x140 :  0x141] = NO-OP[Spawn.X]       | Length:   2  (0x2) | Type: uint16
	Segment[0x142 :  0x143] = NO-OP[Spawn.Y]       | Length:   2  (0x2) | Type: uint16
	Segment[0x144 :  0x147] = Planet               | Length:   4  (0x4) | Type: enum[uint32]     | Parent: Planet
	Segment[0x148]          = NO-OP[Season]        | Length:   1  (0x1) | Type: enum[uint8]      | Parent: Season
	Segment[0x149]          = Gamemode             | Length:   1  (0x1) | Type: enum[uint8]      | Parent: Gamemode
	Segment[0x14A]          = World Size           | Length:   1  (0x1) | Type: enum[uint8]      | Parent: InitSize
	Segment[0x14B]          = Sky Size             | Length:   1  (0x1) | Type: enum[uint8]      | Parent: InitSize
	Segment[0x14C :  0x14F] = UNKNOWN              | Length:   4  (0x4) | Type: ???
	-----------------------------------------------------------------------------------------------------------------------------

	Written By: Ryan Smith
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Adventure
{
	/* Constructor */
	private Adventure(
		Guid id, DateTime lastPlayed, Version version, string name, string author, (ushort, ushort) size,
		Planet planet, Gamemode gamemode, SizeType worldSizeType, SizeType skySizeType, Portal[] portals
	)
	{
		// Info
		this.Id = id;
		this.LastPlayed = lastPlayed;
		this.Version = version;
		this.Name = name;
		this.Author = author;
		this.Size = size;
		this.Planet = planet;
		this.Gamemode = gamemode;
		this.WorldSizeType = worldSizeType;
		this.SkySizeType = skySizeType;
		// Portals
		this.Portals = new List<Portal>(portals);
	}
	/* Instance Methods */
	public async Task Save(string fileName)
	{
		using var writer = await ArchiverStream.Writer(fileName, ArchiverStreamType.Adventure);
		await this.ToStream(writer);
	}
	public async Task ToStream(ArchiverStream stream)
	{
		var buffer = new byte[SIZEOF_BUFFER];
		if (stream.Type != ArchiverStreamType.Adventure || !stream.CanWrite)
			throw new ArgumentException($"Expected writeable adventure stream, either incorrect stream type (Type:{stream.Type}) or not writeable (Writeable:{stream.CanWrite})");
		/// Info
		var adventureInfoChunk = stream.StartChunk(ArchiverChunkType.WorldInfo);
		{
			// UUID
			var uuid = this.Id.ToByteArray();
			await adventureInfoChunk.WriteAsync(uuid, 0, uuid.Length);
			// {Last Played | Version | Name}
			var epoch = new DateTimeOffset(this.LastPlayed).ToUnixTimeSeconds();
			BitConverter.LittleEndian.Write((uint)epoch, buffer, OFFSET_TIMESTAMP - uuid.Length);
			BitConverter.LittleEndian.Write((uint)this.Version, buffer, OFFSET_VERSION - uuid.Length);
			BitConverter.Write(this.Name, buffer, (int)OFFSET_NAME - uuid.Length, SIZEOF_NAME);
			await adventureInfoChunk.WriteAsync(buffer, 0, buffer.Length - uuid.Length);
			// {Author | World Size | Planet | Season | Gamemode | World GenSize | Sky GenSize | UNKNOWN}
			BitConverter.Write(this.Author, buffer, OFFSET_AUTHOR, SIZEOF_AUTHOR);
			BitConverter.LittleEndian.Write(this.Size.Width, buffer, OFFSET_WORLD);
			BitConverter.LittleEndian.Write(this.Size.Height, buffer, OFFSET_WORLD + sizeof(ushort));
			BitConverter.LittleEndian.Write((uint)this.Planet, buffer, OFFSET_PLANET);
			buffer[OFFSET_GAMEMODE] = (byte)this.Gamemode;
			buffer[OFFSET_WORLDSIZETYPE] = (byte)this.WorldSizeType;
			buffer[OFFSET_SKYSIZETYPE] = (byte)this.SkySizeType;
			// -UNKNOWN(4)-\\
			BitConverter.LittleEndian.Write((uint)0, buffer, OFFSET_SKYSIZETYPE + sizeof(byte));
			await adventureInfoChunk.WriteAsync(buffer, 0, SIZEOF_INFO);
		}
		stream.EndChunk();
		/// Portals
		var adventurePortalsChunk = stream.StartChunk(ArchiverChunkType.AdventurePortals);
		{
			BitConverter.LittleEndian.Write((uint)this.Portals.Count, buffer, 0);
			await adventurePortalsChunk.WriteAsync(buffer, 0, sizeof(uint));
			foreach (var portal in this.Portals)
				await portal.ToStream(adventurePortalsChunk);
		}
		stream.EndChunk();
	}
	/* Static Methods */
	public static async Task<Adventure> Load(string fileName)
	{
		using var reader = await ArchiverStream.Reader(fileName);
		return await Adventure.FromStream(reader);
	}
	public static async Task<Adventure> FromStream(ArchiverStream stream)
	{
		var bytesRead = 0;
		var buffer = new byte[SIZEOF_BUFFER];
		if (stream.Type != ArchiverStreamType.Adventure || !stream.CanRead)
			throw new ArgumentException($"Expected readable adventure stream, either incorrect stream type (Type:{stream.Type}) or not readable (Readable:{stream.CanRead})");
		/// Info
		if (!stream.IsAtChunk(ArchiverChunkType.WorldInfo))
			stream.JumpToChunk(ArchiverChunkType.WorldInfo);
		// {UUID | Last Played | Version | Name}
		while (bytesRead < buffer.Length)
			bytesRead += await stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
		var id = new Guid(new Span<byte>(buffer, OFFSET_UUID, SIZEOF_UUID));
		var lastPlayed = DateTimeOffset.FromUnixTimeSeconds(
			BitConverter.LittleEndian.GetUInt32(buffer, OFFSET_TIMESTAMP)
		).LocalDateTime;
		var version = (Version)BitConverter.LittleEndian.GetUInt32(buffer, OFFSET_VERSION);
		var name = BitConverter.GetString(buffer, OFFSET_NAME);
		// {Author | World Size | Player Location | Spawn Location | Planet | Season | Gamemode | World GenSize | Sky GenSize | UNKNOWN}
		bytesRead = 0;
		while (bytesRead < SIZEOF_INFO)
			bytesRead += await stream.ReadAsync(buffer, bytesRead, SIZEOF_INFO - bytesRead);
		var author = BitConverter.GetString(buffer, OFFSET_AUTHOR);
		(ushort width, ushort height) size = (
			BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_WORLD),
			BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_WORLD + sizeof(ushort))
		);
		var planet        = (Planet)BitConverter.LittleEndian.GetUInt32(buffer, OFFSET_PLANET);
		var gamemode      = (Gamemode)buffer[OFFSET_GAMEMODE];
		var worldSizeType = (SizeType)buffer[OFFSET_WORLDSIZETYPE];
		var skySizeType   = (SizeType)buffer[OFFSET_SKYSIZETYPE];
		// -UNKNOWN(4)-\\
		/// Portals
		if (!stream.IsAtChunk(ArchiverChunkType.AdventurePortals))
			stream.JumpToChunk(ArchiverChunkType.AdventurePortals);
		bytesRead = 0;
		while (bytesRead < sizeof(uint))
			bytesRead += await stream.ReadAsync(buffer, bytesRead, sizeof(uint) - bytesRead);
		var portalCount = JJx.BitConverter.LittleEndian.GetUInt32(buffer);
		var portals = new Portal[portalCount];
		for (var i = 0; i < portals.Length; ++i)
			portals[i] = await Portal.FromStream(stream);
		return new Adventure(
			id, lastPlayed, version, name, author, size, planet,
			gamemode, worldSizeType, skySizeType, portals
		);
	}
	/* Properties */
	// Info
	public readonly Guid Id = Guid.NewGuid();
	public DateTime LastPlayed = DateTime.Now;
	public readonly Version Version = Version.Latest;
	private string _Name;
	public string Name {
		get { return this._Name; }
		set {
			if (String.IsNullOrEmpty(value)) return;
			else if (value.Length < SIZEOF_NAME) this._Name = value;
			else this._Name = value.Substring(0, SIZEOF_NAME - 1);
		}
	}
	private string _Author;
	public string Author {
		get { return this._Author; }
		set {
			if (String.IsNullOrEmpty(value)) return;
			else if (value.Length < SIZEOF_AUTHOR) this._Author = value;
			else this._Author = value.Substring(0, SIZEOF_AUTHOR - 1);
		}
	}
	public (ushort Width, ushort Height) Size;
	public Planet Planet;
	public Gamemode Gamemode;
	public SizeType WorldSizeType;
	public SizeType SkySizeType;
	// Portals
	public readonly List<Portal> Portals = new List<Portal>();
	/* Class Properties */
	private const byte SIZEOF_BUFFER        = 56;
	private const byte OFFSET_UUID          =  0;
	private const byte OFFSET_TIMESTAMP     = 16;
	private const byte OFFSET_VERSION       = 20;
	private const byte OFFSET_NAME          = 24;
	private const byte OFFSET_AUTHOR        =  0;
	private const byte OFFSET_WORLD         = 16;
	private const byte OFFSET_PLANET        = 28;
	private const byte OFFSET_GAMEMODE      = 33;
	private const byte OFFSET_WORLDSIZETYPE = 34;
	private const byte OFFSET_SKYSIZETYPE   = 35;
	private const byte SIZEOF_UUID          = 16;
	private const byte SIZEOF_NAME          = 32;
	private const byte SIZEOF_AUTHOR        = 16;
	// Author(16), World.[Width(2), Height(2)](4), Player.[X(2), Y(2)](4), Spawn.[X(2), Y(2)](4), Planet(4), Season(1), Gamemode(1), WorldSize(1), SkySize(1), UNKNOWN(4) == 40
	private const byte SIZEOF_INFO          = SIZEOF_AUTHOR + (6 * sizeof(ushort)) + sizeof(uint) + (4 * sizeof(byte)) + 4;
}
