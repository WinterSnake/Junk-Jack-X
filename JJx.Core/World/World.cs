/*
	Junk Jack X: Core
	- World

	Segment Breakdown:
	-----------------------------------------------------------------------------------------------------------------------------
	:<Info>
	Segment[0xF0  :   0xFF] = UUID                  | Length:  16    (0x10) | Type: uuid
	Segment[0x100 :  0x103] = Last Played Timestamp | Length:   4     (0x4) | Type: DateTime[uint32]
	Segment[0x104 :  0x107] = Game Version          | Length:   4     (0x4) | Type: uint32             | Parent: JJx.Version
	Segment[0x108 :  0x127] = Name                  | Length:  32    (0x20) | Type: char*
	Segment[0x128 :  0x137] = Author                | Length:  16    (0x10) | Type: char*
	Segment[0x138 :  0x139] = World.Width           | Length:   2     (0x2) | Type: uint16             | Parent: Blocks.Width
	Segment[0x13A :  0x13B] = World.Height          | Length:   2     (0x2) | Type: uint16             | Parent: Blocks.Height
	Segment[0x13C :  0x13D] = Player.X              | Length:   2     (0x2) | Type: uint16             | Parent: Player.X
	Segment[0x13E :  0x13F] = Player.Y              | Length:   2     (0x2) | Type: uint16             | Parent: Player.Y
	Segment[0x140 :  0x141] = Spawn.X               | Length:   2     (0x2) | Type: uint16             | Parent: Spawn.X
	Segment[0x142 :  0x143] = Spawn.Y               | Length:   2     (0x2) | Type: uint16             | Parent: Spawn.Y
	Segment[0x144 :  0x147] = Planet                | Length:   4     (0x4) | Type: enum flag[uint32]
	Segment[0x148]          = Season                | Length:   1     (0x1) | Type: enum[uint8]        | Parent: Season
	Segment[0x149]          = Gamemode              | Length:   1     (0x1) | Type: enum[uint8]        | Parent: Gamemode
	Segment[0x14A]          = World Size            | Length:   1     (0x1) | Type: enum[uint8]        | Parent: InitSize
	Segment[0x14B]          = Sky Size              | Length:   1     (0x1) | Type: enum[uint8]        | Parent: InitSize
	Segment[0x14C :  0x14F] = UNKNOWN               | Length:   4     (0x4) | Type: ???
	Segment[0x150 :  0x1CF] = Padding               | Length: 128    (0x80) | Type: uint32[32] = {0}
	-----------------------------------------------------------------------------------------------------------------------------

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public enum Gamemode : byte
{
	Survival  = 0x0,
	Creative  = 0x1,
	Flat      = 0x2,
	Adventure = 0x3,
}

public sealed class World
{
	/* Constructors */
	public World(
		string name, SizeType worldSize, SizeType skySize, Gamemode gamemode,
		string author = "author", Season season = Season.None, Planet planet = Planet.Terra
	)
	{
		this.Name = name;
		this.Author = author;
		this.Player = (0, 1);
		this.Spawn  = (0, 1);
		this.Planet = planet;
		this.Season = season;
		this.Gamemode = gamemode;
		this.WorldSizeType = worldSize;
		this.SkySizeType = skySize;
	}
	private World(
		Guid id, DateTime lastPlayed, Version version, string name, string author, (ushort, ushort) player, (ushort, ushort) spawn,
		Planet planet, Season season, Gamemode gamemode, SizeType worldSizeType, SizeType skySizeType, ushort[] skyline,
		TileMap tileMap, uint ticks, Period period, float poissonSum, Weather weather, byte poissonSkipped,
		byte[] fluidLayer, byte[] circuitLayer
	)
	{
		// Info
		this.Id = id;
		this.LastPlayed = lastPlayed;
		this.Version = version;
		this._Name = name;
		this._Author = author;
		this.Player = player;
		this.Spawn = spawn;
		this.Planet = planet;
		this.Season = season;
		this.Gamemode = gamemode;
		this.WorldSizeType = worldSizeType;
		this.SkySizeType = skySizeType;
		// Skyline
		this.Skyline = skyline;
		// Blocks
		this._TileMap = tileMap;
		// Time
		this.Ticks = ticks;
		this.Time = period;
		// Weather
		this.PoissonSum = poissonSum;
		this.Weather = weather;
		this.PoissonSkipped = poissonSkipped;
		// Containers
		this.FluidLayer = fluidLayer;
		this.CircuitLayer = circuitLayer;
	}
	/* Instance Methods */
	public async Task Save(string fileName)
	{
		using var writer = await ArchiverStream.Writer(fileName, ArchiverStreamType.World);
		await this.ToStream(writer);
	}
	public async Task ToStream(ArchiverStream stream)
	{
		var buffer = new byte[SIZEOF_BUFFER];
		if (stream.Type != ArchiverStreamType.World || !stream.CanWrite)
			throw new ArgumentException($"Expected writeable world stream, either incorrect stream type (Type:{stream.Type}) or not writeable (Writeable:{stream.CanWrite})");
		/// Info
		var worldInfoChunk = stream.StartChunk(ArchiverChunkType.WorldInfo);
		{
			// UUID
			var uuid = this.Id.ToByteArray();
			await worldInfoChunk.WriteAsync(uuid, 0, uuid.Length);
			// {Last Played | Version | Name}
			var epoch = new DateTimeOffset(this.LastPlayed).ToUnixTimeSeconds();
			BitConverter.LittleEndian.Write((uint)epoch, buffer, OFFSET_TIMESTAMP - uuid.Length);
			BitConverter.LittleEndian.Write((uint)this.Version, buffer, OFFSET_VERSION - uuid.Length);
			BitConverter.Write(this.Name, buffer, (int)OFFSET_NAME - uuid.Length, SIZEOF_NAME);
			await worldInfoChunk.WriteAsync(buffer, 0, buffer.Length - uuid.Length);
			// {Author | World Size | Player Location | Spawn Location | Planet | Season | Gamemode | World GenSize | Sky GenSize | UNKNOWN}
			BitConverter.Write(this.Author, buffer, OFFSET_AUTHOR, SIZEOF_AUTHOR);
			BitConverter.LittleEndian.Write(this.Size.Width, buffer, OFFSET_WORLD);
			BitConverter.LittleEndian.Write(this.Size.Height, buffer, OFFSET_WORLD + sizeof(ushort));
			BitConverter.LittleEndian.Write(this.Player.X, buffer, OFFSET_PLAYER);
			BitConverter.LittleEndian.Write(this.Player.Y, buffer, OFFSET_PLAYER + sizeof(ushort));
			BitConverter.LittleEndian.Write(this.Spawn.X, buffer, OFFSET_SPAWN);
			BitConverter.LittleEndian.Write(this.Spawn.Y, buffer, OFFSET_SPAWN + sizeof(ushort));
			BitConverter.LittleEndian.Write((uint)this.Planet, buffer, OFFSET_PLANET);
			buffer[OFFSET_SEASON] = (byte)this.Season;
			buffer[OFFSET_GAMEMODE] = (byte)this.Gamemode;
			buffer[OFFSET_WORLDSIZETYPE] = (byte)this.WorldSizeType;
			buffer[OFFSET_SKYSIZETYPE] = (byte)this.SkySizeType;
			BitConverter.LittleEndian.Write((ulong)0, buffer, OFFSET_SKYSIZETYPE + sizeof(byte));
			await worldInfoChunk.WriteAsync(buffer, 0, SIZEOF_INFO);
			buffer = new byte[SIZEOF_PADDING];
			// Padding
			await worldInfoChunk.WriteAsync(buffer, 0, SIZEOF_PADDING);
		}
		stream.EndChunk();
		/// Skyline
		var worldSkylineChunk = stream.StartChunk(ArchiverChunkType.WorldSkyline);
		{
			// TODO: iterate without resizing buffer
			buffer = new byte[this.Skyline.Length * sizeof(ushort)];
			for (var i = 0; i < this.Skyline.Length; ++i)
				BitConverter.LittleEndian.Write(this.Skyline[i], buffer, i * sizeof(ushort));
			await worldSkylineChunk.WriteAsync(buffer, 0, buffer.Length);
		}
		stream.EndChunk();
		/// Blocks
		var worldBlocksChunk = stream.StartChunk(ArchiverChunkType.WorldBlocks, version: 1, compressed: true);
		{
			await this._TileMap.ToStream(worldBlocksChunk);
		}
		stream.EndChunk();
		/// Fog
		if (this.FogLayer != null)
		{
			var worldFogChunk = stream.StartChunk(ArchiverChunkType.WorldFog, compressed: true);
			{
			}
			stream.EndChunk();
		}
		/// Time
		var worldTimeChunk = stream.StartChunk(ArchiverChunkType.WorldTime);
		{
			BitConverter.LittleEndian.Write(this.Ticks, buffer, OFFSET_TIMETICKS);
			buffer[OFFSET_TIMEPERIOD] = (byte)this.Time;
			BitConverter.LittleEndian.Write((ulong)0, buffer, OFFSET_TIMEPERIOD + sizeof(byte));
			await worldTimeChunk.WriteAsync(buffer, 0, SIZEOF_TIME);
		}
		stream.EndChunk();
		/// Weather
		var worldWeatherChunk = stream.StartChunk(ArchiverChunkType.WorldWeather);
		{
			BitConverter.LittleEndian.Write(this.PoissonSum / 10.0f, buffer, OFFSET_WEATHERSUM);
			buffer[OFFSET_WEATHERTYPE] = (byte)this.Weather;
			buffer[OFFSET_WEATHERSKIP] = (byte)this.PoissonSkipped;
			BitConverter.LittleEndian.Write((ulong)0, buffer, OFFSET_WEATHERSKIP + sizeof(byte));
			await worldWeatherChunk.WriteAsync(buffer, 0, SIZEOF_WEATHER);
		}
		stream.EndChunk();
		/// Chests
		var worldChestsChunk = stream.StartChunk(ArchiverChunkType.WorldChests);
		{
			BitConverter.LittleEndian.Write((uint)0, buffer, 0);
			await worldChestsChunk.WriteAsync(buffer, 0, sizeof(uint));
		}
		stream.EndChunk();
		/// Forges
		var worldForgesChunk = stream.StartChunk(ArchiverChunkType.WorldForges);
		{
			BitConverter.LittleEndian.Write((uint)0, buffer, 0);
			await worldForgesChunk.WriteAsync(buffer, 0, sizeof(uint));
		}
		stream.EndChunk();
		/// Signs
		var worldSignsChunk = stream.StartChunk(ArchiverChunkType.WorldSigns);
		{
			BitConverter.LittleEndian.Write((uint)0, buffer, 0);
			await worldSignsChunk.WriteAsync(buffer, 0, sizeof(uint));
		}
		stream.EndChunk();
		/// Stables
		var worldStablesChunk = stream.StartChunk(ArchiverChunkType.WorldStables);
		{
			BitConverter.LittleEndian.Write((uint)0, buffer, 0);
			await worldStablesChunk.WriteAsync(buffer, 0, sizeof(uint));
		}
		stream.EndChunk();
		/// Labs
		var worldLabsChunk = stream.StartChunk(ArchiverChunkType.WorldLabs);
		{
			BitConverter.LittleEndian.Write((uint)0, buffer, 0);
			await worldLabsChunk.WriteAsync(buffer, 0, sizeof(uint));
		}
		stream.EndChunk();
		/// Shelves
		var worldShelvesChunk = stream.StartChunk(ArchiverChunkType.WorldShelves);
		{
			BitConverter.LittleEndian.Write((uint)0, buffer, 0);
			await worldShelvesChunk.WriteAsync(buffer, 0, sizeof(uint));
		}
		stream.EndChunk();
		/// Plants
		var worldPlantsChunk = stream.StartChunk(ArchiverChunkType.WorldPlants);
		{
			BitConverter.LittleEndian.Write((uint)0, buffer, 0);
			await worldPlantsChunk.WriteAsync(buffer, 0, sizeof(uint));
		}
		stream.EndChunk();
		/// Fruits
		var worldFruitsChunk = stream.StartChunk(ArchiverChunkType.WorldFruits);
		{
			BitConverter.LittleEndian.Write((uint)0, buffer, 0);
			await worldFruitsChunk.WriteAsync(buffer, 0, sizeof(uint));
		}
		stream.EndChunk();
		/// Plant Decay
		var worldPlantDecayChunk = stream.StartChunk(ArchiverChunkType.WorldPlantDecay);
		{
			BitConverter.LittleEndian.Write((uint)0, buffer, 0);
			await worldPlantDecayChunk.WriteAsync(buffer, 0, sizeof(uint));
		}
		stream.EndChunk();
		/// Locks
		var worldLocksChunk = stream.StartChunk(ArchiverChunkType.WorldLocks);
		{
			BitConverter.LittleEndian.Write((uint)0, buffer, 0);
			await worldLocksChunk.WriteAsync(buffer, 0, sizeof(uint));
		}
		stream.EndChunk();
		/// Fluid
		var worldFluidChunk = stream.StartChunk(ArchiverChunkType.WorldFluid);
		{
			await worldFluidChunk.WriteAsync(this.FluidLayer, 0, this.FluidLayer.Length);
		}
		stream.EndChunk();
		/// Circuitry
		var worldCircuitryChunk = stream.StartChunk(ArchiverChunkType.WorldCircuitry);
		{
			await worldCircuitryChunk.WriteAsync(this.CircuitLayer, 0, this.CircuitLayer.Length);
		}
		stream.EndChunk();
		/// Mobs
		var worldMobsChunk = stream.StartChunk(ArchiverChunkType.WorldMobs);
		{
			BitConverter.LittleEndian.Write((uint)0, buffer, 0);
			await worldMobsChunk.WriteAsync(buffer, 0, sizeof(uint));
		}
		stream.EndChunk();
		/// Padding
		if (this.FogLayer == null)
			stream.EmptyChunk();
	}
	/* Static Methods */
	public static async Task<World> Load(string fileName)
	{
		using var reader = await ArchiverStream.Reader(fileName);
		return await World.FromStream(reader);
	}
	public static async Task<World> FromStream(ArchiverStream stream)
	{
		var bytesRead = 0;
		var buffer = new byte[SIZEOF_BUFFER];
		if (stream.Type != ArchiverStreamType.World || !stream.CanRead)
			throw new ArgumentException($"Expected readable world stream, either incorrect stream type (Type:{stream.Type}) or not readable (Readable:{stream.CanRead})");
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
		(ushort x, ushort y) player = (
			BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_PLAYER),
			BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_PLAYER + sizeof(ushort))
		);
		(ushort x, ushort y) spawn = (
			BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_SPAWN),
			BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_SPAWN + sizeof(ushort))
		);
		var planet        = (Planet)BitConverter.LittleEndian.GetUInt32(buffer, OFFSET_PLANET);
		var season        = (Season)buffer[OFFSET_SEASON];
		var gamemode      = (Gamemode)buffer[OFFSET_GAMEMODE];
		var worldSizeType = (SizeType)buffer[OFFSET_WORLDSIZETYPE];
		var skySizeType   = (SizeType)buffer[OFFSET_SKYSIZETYPE];
		// Padding
		stream.Position += SIZEOF_PADDING;
		/// Skyline
		if (!stream.IsAtChunk(ArchiverChunkType.WorldSkyline))
			stream.JumpToChunk(ArchiverChunkType.WorldSkyline);
		bytesRead = 0;
		// TODO: iterate without resizing buffer
		buffer = new byte[size.width * sizeof(ushort)];
		var skyline = new ushort[size.width];
		while (bytesRead < buffer.Length)
			bytesRead += await stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
		for (var i = 0; i < skyline.Length; ++i)
			skyline[i] = BitConverter.LittleEndian.GetUInt16(buffer, i * sizeof(ushort));
		/// Blocks
		if (!stream.IsAtChunk(ArchiverChunkType.WorldBlocks))
			stream.JumpToChunk(ArchiverChunkType.WorldBlocks);
		// TODO: Figure out why GZipStream reads more than asked through this single line method (try to remove memorystream requirement)
		//var tileMap = await TileMap.FromStream(stream, size, stream.IsChunkCompressed(ArchiverChunkType.WorldBlocks));
		bytesRead = 0;
		TileMap tileMap;
		var blockChunk = new byte[stream.GetChunkSize(ArchiverChunkType.WorldBlocks)];
		using (var blockStream = new MemoryStream(blockChunk))
		{
			while (bytesRead < blockChunk.Length)
				bytesRead += await stream.ReadAsync(blockChunk, bytesRead, blockChunk.Length - bytesRead);
			tileMap = await TileMap.FromStream(blockStream, size, stream.IsChunkCompressed(ArchiverChunkType.WorldBlocks));
		}
		/// Fog
		if (stream.HasChunk(ArchiverChunkType.WorldFog))
		{
			if (!stream.IsAtChunk(ArchiverChunkType.WorldFog))
				stream.JumpToChunk(ArchiverChunkType.WorldFog);
		}
		/// Time
		if (!stream.IsAtChunk(ArchiverChunkType.WorldTime))
			stream.JumpToChunk(ArchiverChunkType.WorldTime);
		bytesRead = 0;
		while (bytesRead < SIZEOF_TIME)
			bytesRead += await stream.ReadAsync(buffer, bytesRead, SIZEOF_TIME - bytesRead);
		var ticks  = BitConverter.LittleEndian.GetUInt32(buffer, OFFSET_TIMETICKS);
		var period = (Period)buffer[OFFSET_TIMEPERIOD];
		/// Weather
		if (!stream.IsAtChunk(ArchiverChunkType.WorldWeather))
			stream.JumpToChunk(ArchiverChunkType.WorldWeather);
		bytesRead = 0;
		while (bytesRead < SIZEOF_WEATHER)
			bytesRead += await stream.ReadAsync(buffer, bytesRead, SIZEOF_WEATHER - bytesRead);
		var poissonSum     = BitConverter.LittleEndian.GetFloat(buffer, OFFSET_WEATHERSUM) * 10.0f;
		var weather        = (Weather)buffer[OFFSET_WEATHERTYPE];
		var poissonSkipped = buffer[OFFSET_WEATHERSKIP];
		/// Chests
		if (!stream.IsAtChunk(ArchiverChunkType.WorldChests))
			stream.JumpToChunk(ArchiverChunkType.WorldChests);
		bytesRead = 0;
		while (bytesRead < sizeof(uint))
			bytesRead += await stream.ReadAsync(buffer, bytesRead, sizeof(uint) - bytesRead);
		var chestCount = JJx.BitConverter.LittleEndian.GetUInt32(buffer);
		/// Forges
		if (!stream.IsAtChunk(ArchiverChunkType.WorldForges))
			stream.JumpToChunk(ArchiverChunkType.WorldForges);
		bytesRead = 0;
		while (bytesRead < sizeof(uint))
			bytesRead += await stream.ReadAsync(buffer, bytesRead, sizeof(uint) - bytesRead);
		var forgeCount = JJx.BitConverter.LittleEndian.GetUInt32(buffer);
		/// Signs
		if (!stream.IsAtChunk(ArchiverChunkType.WorldSigns))
			stream.JumpToChunk(ArchiverChunkType.WorldSigns);
		bytesRead = 0;
		while (bytesRead < sizeof(uint))
			bytesRead += await stream.ReadAsync(buffer, bytesRead, sizeof(uint) - bytesRead);
		var signCount = JJx.BitConverter.LittleEndian.GetUInt32(buffer);
		/// Stables
		if (!stream.IsAtChunk(ArchiverChunkType.WorldStables))
			stream.JumpToChunk(ArchiverChunkType.WorldStables);
		bytesRead = 0;
		while (bytesRead < sizeof(uint))
			bytesRead += await stream.ReadAsync(buffer, bytesRead, sizeof(uint) - bytesRead);
		var stableCount = JJx.BitConverter.LittleEndian.GetUInt32(buffer);
		/// Labs
		if (!stream.IsAtChunk(ArchiverChunkType.WorldLabs))
			stream.JumpToChunk(ArchiverChunkType.WorldLabs);
		bytesRead = 0;
		while (bytesRead < sizeof(uint))
			bytesRead += await stream.ReadAsync(buffer, bytesRead, sizeof(uint) - bytesRead);
		var labCount = JJx.BitConverter.LittleEndian.GetUInt32(buffer);
		/// Shelves
		if (!stream.IsAtChunk(ArchiverChunkType.WorldShelves))
			stream.JumpToChunk(ArchiverChunkType.WorldShelves);
		bytesRead = 0;
		while (bytesRead < sizeof(uint))
			bytesRead += await stream.ReadAsync(buffer, bytesRead, sizeof(uint) - bytesRead);
		var shelfCount = JJx.BitConverter.LittleEndian.GetUInt32(buffer);
		/// Plants
		if (!stream.IsAtChunk(ArchiverChunkType.WorldPlants))
			stream.JumpToChunk(ArchiverChunkType.WorldPlants);
		bytesRead = 0;
		while (bytesRead < sizeof(uint))
			bytesRead += await stream.ReadAsync(buffer, bytesRead, sizeof(uint) - bytesRead);
		var plantCount = JJx.BitConverter.LittleEndian.GetUInt32(buffer);
		/// Fruits
		if (!stream.IsAtChunk(ArchiverChunkType.WorldFruits))
			stream.JumpToChunk(ArchiverChunkType.WorldFruits);
		bytesRead = 0;
		while (bytesRead < sizeof(uint))
			bytesRead += await stream.ReadAsync(buffer, bytesRead, sizeof(uint) - bytesRead);
		var fruitCount = JJx.BitConverter.LittleEndian.GetUInt32(buffer);
		/// Plant Decay
		if (!stream.IsAtChunk(ArchiverChunkType.WorldPlantDecay))
			stream.JumpToChunk(ArchiverChunkType.WorldPlantDecay);
		bytesRead = 0;
		while (bytesRead < sizeof(uint))
			bytesRead += await stream.ReadAsync(buffer, bytesRead, sizeof(uint) - bytesRead);
		var plantDecayCount = JJx.BitConverter.LittleEndian.GetUInt32(buffer);
		/// Locks
		if (!stream.IsAtChunk(ArchiverChunkType.WorldLocks))
			stream.JumpToChunk(ArchiverChunkType.WorldLocks);
		bytesRead = 0;
		while (bytesRead < sizeof(uint))
			bytesRead += await stream.ReadAsync(buffer, bytesRead, sizeof(uint) - bytesRead);
		var lockCount = JJx.BitConverter.LittleEndian.GetUInt32(buffer);
		/// Fluid
		if (!stream.IsAtChunk(ArchiverChunkType.WorldFluid))
			stream.JumpToChunk(ArchiverChunkType.WorldFluid);
		bytesRead = 0;
		var fluidLayer = new byte[stream.GetChunkSize(ArchiverChunkType.WorldFluid)];
		while (bytesRead < fluidLayer.Length)
			bytesRead += await stream.ReadAsync(fluidLayer, bytesRead, fluidLayer.Length - bytesRead);
		/// Circuitry
		if (!stream.IsAtChunk(ArchiverChunkType.WorldCircuitry))
			stream.JumpToChunk(ArchiverChunkType.WorldCircuitry);
		bytesRead = 0;
		var circuitLayer = new byte[stream.GetChunkSize(ArchiverChunkType.WorldCircuitry)];
		while (bytesRead < circuitLayer.Length)
			bytesRead += await stream.ReadAsync(circuitLayer, bytesRead, circuitLayer.Length - bytesRead);
		/// Mobs
		if (!stream.IsAtChunk(ArchiverChunkType.WorldMobs))
			stream.JumpToChunk(ArchiverChunkType.WorldMobs);
		bytesRead = 0;
		while (bytesRead < sizeof(uint))
			bytesRead += await stream.ReadAsync(buffer, bytesRead, sizeof(uint) - bytesRead);
		var mobCount = JJx.BitConverter.LittleEndian.GetUInt32(buffer);
		/// Padding
		if (stream.HasChunk(ArchiverChunkType.Padding))
			stream.Position += sizeof(uint);
		// -World- \\
		return new World(
			id, lastPlayed, version, name, author, player, spawn, planet,
			season, gamemode, worldSizeType, skySizeType, skyline, tileMap,
			ticks, period, poissonSum, weather, poissonSkipped,
			fluidLayer, circuitLayer
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
	public (ushort Width, ushort Height) Size { get { return ((ushort)this._TileMap.Tiles.GetLength(0), (ushort)this._TileMap.Tiles.GetLength(1)); }}
	public (ushort X, ushort Y) Player;
	public (ushort X, ushort Y) Spawn;
	public Planet Planet;
	public Season Season;
	public Gamemode Gamemode;
	public SizeType WorldSizeType;
	public SizeType SkySizeType;
	// Skyline
	public readonly ushort[] Skyline;  // TODO: Make resizeable
	// Blocks
	public Tile[,] Blocks { get { return this._TileMap.Tiles; }}
	private readonly TileMap _TileMap;  // TODO: Make changeable (?)
	// Time
	public uint Ticks = 0;
	public Period Time = Period.Day;
	// Weather
	public float PoissonSum = 0.0f;
	public Weather Weather = Weather.None;
	public byte PoissonSkipped = 0;
	// Containers
	// TEMPORARY
	#nullable enable
	private readonly byte[]? FogLayer = null;
	#nullable disable
	private readonly byte[] FluidLayer = new byte[SIZEOF_FLUIDLAYER];
	private readonly byte[] CircuitLayer = new byte[SIZEOF_CIRCUITLAYER];
	/* Class Properties */
	private const byte SIZEOF_BUFFER        = 56;
	private const byte OFFSET_UUID          =  0;
	private const byte OFFSET_TIMESTAMP     = 16;
	private const byte OFFSET_VERSION       = 20;
	private const byte OFFSET_NAME          = 24;
	private const byte OFFSET_AUTHOR        =  0;
	private const byte OFFSET_WORLD         = 16;
	private const byte OFFSET_PLAYER        = 20;
	private const byte OFFSET_SPAWN         = 24;
	private const byte OFFSET_PLANET        = 28;
	private const byte OFFSET_SEASON        = 32;
	private const byte OFFSET_GAMEMODE      = 33;
	private const byte OFFSET_WORLDSIZETYPE = 34;
	private const byte OFFSET_SKYSIZETYPE   = 35;
	private const byte OFFSET_TIMETICKS     =  0;
	private const byte OFFSET_TIMEPERIOD    =  4;
	private const byte OFFSET_WEATHERSUM    =  0;
	private const byte OFFSET_WEATHERTYPE   =  4;
	private const byte OFFSET_WEATHERSKIP   =  5;
	private const byte SIZEOF_UUID          = 16;
	private const byte SIZEOF_NAME          = 32;
	private const byte SIZEOF_AUTHOR        = 16;
	// Author(16), World.[Width(2), Height(2)](4), Player.[X(2), Y(2)](4), Spawn.[X(2), Y(2)](4), Planet(4), Season(1), Gamemode(1), WorldSize(1), SkySize(1), UNKNOWN(4) == 40
	private const byte SIZEOF_INFO          = SIZEOF_AUTHOR + (6 * sizeof(ushort)) + sizeof(uint) + (4 * sizeof(byte)) + 4;
	private const byte SIZEOF_PADDING       = 128;
	private const byte SIZEOF_TIME          = sizeof(uint) + sizeof(byte) + 3;  // Ticks(4), Period(1), UNKNOWN(3) == 8
	private const byte SIZEOF_WEATHER       = sizeof(float) + (2 * sizeof(byte)) + 2;  // PoissonSum(4), WeatherType(1), PoissonSkipped(1), UNKNOWN(2) == 8
	// Temporary
	private const ushort SIZEOF_FLUIDLAYER   =  8;
	private const byte   SIZEOF_CIRCUITLAYER = 16;
}
