/*
	Junk Jack X: Core
	- World

	Segment Breakdown:
	---------------------------------------------------------------------------------------------------------------------
	:<Info>
	Segment[0xF0  :   0xFF] = UUID                 | Length:  16 (0x10) | Type: uuid
	Segment[0x100 :  0x103] = Last Played Datetime | Length:   4  (0x4) | Type: uint             | Parent: DateTime
	Segment[0x104 :  0x107] = Game Version         | Length:   4  (0x4) | Type: enum[uint32]     | Parent: JJx.Version
	Segment[0x108 :  0x127] = Name                 | Length:  32 (0x20) | Type: char*
	Segment[0x128 :  0x137] = Author               | Length:  16 (0x10) | Type: char*
	Segment[0x138 :  0x139] = World.Width          | Length:   2  (0x2) | Type: uint16
	Segment[0x13A :  0x13B] = World.Height         | Length:   2  (0x2) | Type: uint16t
	Segment[0x13C :  0x13D] = Player.X             | Length:   2  (0x2) | Type: uint16
	Segment[0x13E :  0x13F] = Player.Y             | Length:   2  (0x2) | Type: uint16
	Segment[0x140 :  0x141] = Spawn.X              | Length:   2  (0x2) | Type: uint16
	Segment[0x142 :  0x143] = Spawn.Y              | Length:   2  (0x2) | Type: uint16
	Segment[0x144 :  0x147] = Planet               | Length:   4  (0x4) | Type: enum[uint32]     | Parent: Planet
	Segment[0x148]          = Season               | Length:   1  (0x1) | Type: enum[uint8]      | Parent: Season
	Segment[0x149]          = Gamemode             | Length:   1  (0x1) | Type: enum[uint8]      | Parent: Gamemode
	Segment[0x14A]          = World Size           | Length:   1  (0x1) | Type: enum[uint8]      | Parent: InitSize
	Segment[0x14B]          = Sky Size             | Length:   1  (0x1) | Type: enum[uint8]      | Parent: InitSize
	Segment[0x14C :  0x14F] = UNKNOWN              | Length:   4  (0x4) | Type: ???
	Segment[0x150 :  0x1CF] = Padding              | Length: 128 (0x80) | Type: uint32[32] = {0}
	---------------------------------------------------------------------------------------------------------------------

	Written By: Ryan Smith
*/

using System;
using System.Collections.Generic;

namespace JJx.Core;

public enum Gamemode : byte
{
	Survival  = 0x0,
	Creative  = 0x1,
	Flat      = 0x2,
	Adventure = 0x3,
}

public sealed class World
{
	/* Constructor */
	internal World(
		Guid id, Version version, DateTime lastPlayed, string name, string author, Planet planet,
		Season season, Gamemode gamemode, (ushort Width, ushort Height) size, MapBounds sizeBounds, MapBounds skyBounds
	)
	{
		this.Id = id;
		this.Version = version;
		this.LastPlayed = lastPlayed;
		this._Name = name;
		this._Author = author;
		this.Planet = planet;
		this.Season = season;
		this.Gamemode = gamemode;
		this.SizeBounds = sizeBounds;
		this.SkyBounds = skyBounds;
		this.Size = size;
		this._Skyline = new ushort[size.Width];
	}
	/* Instance Methods */
	/* Properties */
	// Info
	public readonly Guid Id = Guid.NewGuid();
	public readonly Version Version = Version.Latest;
	public DateTime LastPlayed = DateTime.Now;
	private string _Name;
	public string Name {
		get => this._Name;
		set => this._Name = value.Length > MAX_NAME_LENGTH ? value[..MAX_NAME_LENGTH] : value;
	}
	private string _Author;
	public string Author {
		get => this._Author;
		set => this._Author = value.Length > MAX_AUTHOR_LENGTH ? value[..MAX_AUTHOR_LENGTH] : value;
	}
	public Planet Planet;
	public Season Season;
	public Gamemode Gamemode;
	public MapBounds SizeBounds;
	public MapBounds SkyBounds;
	public (ushort Width, ushort Height) Size { get; internal set; }
	public (ushort X, ushort Y) Player;
	public (ushort X, ushort Y) Spawn;
	// Skyline
	private readonly ushort[] _Skyline;
	public Span<ushort> Skyline => this._Skyline.AsSpan();
	// Tiles
	internal Tilemap? _Tilemap;
	public Tilemap Tilemap => this._Tilemap!;
	// Fog
	internal byte[]? _Fog = null;
	public bool HasFog => this._Fog is not null;
	public Span<byte> Fog => this._Fog.AsSpan();
	// Time
	private readonly byte[] _Time = new byte[SIZEOF_TIME];
	public Span<byte> Time => this._Time.AsSpan();
	// Weather
	private readonly byte[] _Weather = new byte[SIZEOF_WEATHER];
	public Span<byte> Weather => this._Weather.AsSpan();
	// Containers
	public readonly List<Sign> Signs = new();
	public readonly List<Lab> Labs = new();
	public readonly List<Shelf> Shelves = new();
	public readonly List<Lock> Locks = new();
	public readonly List<Entity> Entities = new();
	// Fluids
	internal byte[] _Fluid = Array.Empty<byte>();
	public Span<byte> Fluid => this._Fluid.AsSpan();
	// Circuitry
	internal byte[] _Circuitry = Array.Empty<byte>();
	public Span<byte> Circuitry => this._Circuitry.AsSpan();
	/* Class Properties */
	private const int MAX_NAME_LENGTH   = WorldArchive.SIZEOF_NAME - 1;
	private const int MAX_AUTHOR_LENGTH = WorldArchive.SIZEOF_AUTHOR - 1;
	private const int SIZEOF_TIME       = 8;
	private const int SIZEOF_WEATHER    = 8;
}
