/*
	Junk Jack X: World

	Segment Breakdown:
	-----------------------------------------------------------------------------------------------------------------------
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
	Segment[0x14C :  0x14F] = Language              | Length:   4     (0x4) | Type: char[4]
	Segment[0x150 :  0x1CF] = Padding               | Length: 128    (0x80) | Type: uint32[32] = {0}
	:<Skyline>
	Segment[0x1D0 : {size}] = Skyline               | Length: {world.width} | Type: uint16[{size.width}]
	:<Blocks>
	Segment[{pos} : {size}] = Blocks                | Length:      {header} | Type: struct Block[{size.x][{size.y}]
	-----------------------------------------------------------------------------------------------------------------------

	Written By: Ryan Smith
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
	#nullable enable
	public World(
		string name, GenSize initSize, GenSize skySize, Gamemode gamemode,
		string author = "author", Season season = Season.None, Planet planet = Planet.Terra,
		(ushort width, ushort height)? size = null, TileMap? blockmap = null
	)
	{
		// Info
		this.Id = Guid.NewGuid();
		this.Version = Version.Latest;
		this.LastPlayed = DateTime.Now;
		this.Name = name;
		this.Author = author;
		this.Player = (0, 1);
		this.Spawn = (0, 1);
		this.Planet = planet;
		this.Season = season;
		this.Gamemode = gamemode;
		this.InitSize = initSize;
		this.SkySize = skySize;
		this._Language = "en";
		// -Size | BlockMap
		if (blockmap != null)
			this.BlockMap = blockmap;
		else
		{
			if (!size.HasValue)
			{
				switch (initSize)
				{
					case GenSize.Tiny:   size = ( 512, 128); break;
					case GenSize.Small:  size = ( 768, 256); break;
					case GenSize.Normal: size = (1024, 256); break;
					case GenSize.Large:  size = (2048, 384); break;
					case GenSize.Huge:   size = (4096, 512); break;
					default: throw new ArgumentException("Cannot create world with custom init size without specifying actual world size");
				}
			}
			this.BlockMap = new TileMap(size.Value);
		}
		// Skyline
		this.Skyline = new ushort[this.Size.Width];
		for (var i = 0; i < this.Skyline.Length; ++i)
			this.Skyline[i] = (ushort)(this.Size.Height / 1.5);
	}
	#nullable disable
	private World(
		Guid id, Version version, DateTime lastPlayed, string name, string author,
		(ushort, ushort) player, (ushort, ushort) spawn, Planet planet, Season season,
		Gamemode gamemode, GenSize initSize, GenSize skySize, string language, ushort[] skyline,
		TileMap blockmap, uint ticks, DayPhase phase, float poissonSum, Weather weather, byte poissonSkipped,
		Chest[] chests, Forge[] forges, Sign[] signs, Stable[] stables, Lab[] labs, Shelf[] shelves,
		Fruit[] fruits, Lock[] locks, Mob[] mobs
	)
	{
		// Info
		this.Id = id;
		this.Version = version;
		this.LastPlayed = lastPlayed;
		this._Name = name;
		this._Author = author;
		this.Player = player;
		this.Spawn = spawn;
		this.Planet = planet;
		this.Season = season;
		this.Gamemode = gamemode;
		this.InitSize = initSize;
		this.SkySize = skySize;
		this._Language = language;
		// Skyline
		this.Skyline = skyline;
		// Blocks
		this.BlockMap = blockmap;
		// Time
		this.Ticks = ticks;
		this.Phase = phase;
		// Weather
		this.PoissonSum = poissonSum;
		this.Weather = weather;
		this.PoissonSkipped = poissonSkipped;
		// Containers
		this.Chests = new List<Chest>(chests);
		this.Forges = new List<Forge>(forges);
		this.Signs = new List<Sign>(signs);
		this.Stables = new List<Stable>(stables);
		this.Labs = new List<Lab>(labs);
		this.Shelves = new List<Shelf>(shelves);
		this.Fruits = new List<Fruit>(fruits);
		this.Locks = new List<Lock>(locks);
		this.Mobs = new List<Mob>(mobs);
	}
	/* Instance Methods */
	public void Resize(ushort width, ushort height)
	{
	}
	public async Task Save(string filePath)
	{
		var workingData = new byte[SIZEOF_BUFFER];
		using var stream = await ArchiverStream.Writer(filePath, ArchiverType.World);
		/// Info
		var worldInfo = stream.StartChunk(ChunkType.WorldInfo);
			// UUID
			var uuid = this.Id.ToByteArray();
			await worldInfo.WriteAsync(uuid, 0, uuid.Length);
			// {Last Played | Version}
			int _size = SIZEOF_TIMESTAMP + SIZEOF_VERSION;
			// -Last Played
			BitConverter.Write(workingData, this.LastPlayed, 0);
			// -Version
			BitConverter.Write(workingData, (uint)this.Version, 4);
			await worldInfo.WriteAsync(workingData, 0, _size);
			// Name
			BitConverter.Write(workingData, this._Name, length: SIZEOF_NAME);
			await worldInfo.WriteAsync(workingData, 0, SIZEOF_NAME);
			// Author
			BitConverter.Write(workingData, this._Author, length: SIZEOF_AUTHOR);
			await worldInfo.WriteAsync(workingData, 0, SIZEOF_AUTHOR);
			// {World Size | Player Location | Spawn Location | Planet | Season | Gamemode | Init GenSize | Sky GenSize | Language}
			_size = SIZEOF_POSITIONS + SIZEOF_PLANET + SIZEOF_FLAGS + SIZEOF_LANGUAGE;
			// -World Size
			BitConverter.Write(workingData, this.Size.Width,  0);
			BitConverter.Write(workingData, this.Size.Height, 2);
			// -Player Location
			BitConverter.Write(workingData, this.Player.X, 4);
			BitConverter.Write(workingData, this.Player.Y, 6);
			// -Spawn Location
			BitConverter.Write(workingData, this.Spawn.X,  8);
			BitConverter.Write(workingData, this.Spawn.Y, 10);
			// -Planet
			BitConverter.Write(workingData, (uint)this.Planet, 12);
			// -Season
			workingData[16] = (byte)this.Season;
			// -Gamemode
			workingData[17] = (byte)this.Gamemode;
			// -Init GenSize
			workingData[18] = (byte)this.InitSize;
			// -Sky GenSize
			workingData[19] = (byte)this.SkySize;
			// -Language
			BitConverter.Write(workingData, this._Language, 20, length: SIZEOF_LANGUAGE);
			await worldInfo.WriteAsync(workingData, 0, _size);
			// Padding
			workingData = new byte[SIZEOF_PADDING];
			await worldInfo.WriteAsync(workingData, 0, workingData.Length);
		stream.EndChunk();
		/// Skyline
		var worldSkyline = stream.StartChunk(ChunkType.WorldSkyline);
			workingData = new byte[this.Skyline.Length * 2];
			for (var i = 0; i < this.Skyline.Length; ++i)
				BitConverter.Write(workingData, this.Skyline[i], i * 2);
			await worldSkyline.WriteAsync(workingData, 0, workingData.Length);
		stream.EndChunk();
		/// Blocks
		var worldBlocks = stream.StartChunk(ChunkType.WorldBlocks, 1, compressed: true);
			await this.BlockMap.ToStream(worldBlocks, compressed: true);
		stream.EndChunk();
		/// Fog
		if (this.Gamemode == Gamemode.Survival || this.Gamemode == Gamemode.Adventure)
		{
			var worldFog = stream.StartChunk(ChunkType.WorldFog, compressed: true);
			stream.EndChunk();
		}
		/// Time
		var worldTime = stream.StartChunk(ChunkType.WorldTime);
			workingData = new byte[SIZEOF_TIME];
			BitConverter.Write(workingData, this.Ticks, 0);
			workingData[4] = (byte)this.Phase;
			await worldTime.WriteAsync(workingData, 0, workingData.Length);
		stream.EndChunk();
		/// Weather
		var worldWeather = stream.StartChunk(ChunkType.WorldWeather);
			workingData = new byte[SIZEOF_WEATHER];
			BitConverter.Write(workingData, this.PoissonSum, 0);
			workingData[4] = (byte)this.Weather;
			workingData[5] = (byte)this.PoissonSkipped;
			await worldWeather.WriteAsync(workingData, 0, workingData.Length);
		stream.EndChunk();
		/// Containers \\\
		workingData = new byte[SIZEOF_CIRCUITRY];
		/// Chests
		var worldChests = stream.StartChunk(ChunkType.WorldChests);
			BitConverter.Write(workingData, (uint)this.Chests.Count);
			await worldChests.WriteAsync(workingData, 0, SIZEOF_TILECOUNT);
			foreach (var chest in this.Chests)
				await chest.ToStream(worldChests);
		stream.EndChunk();
		/// Forges
		var worldForges = stream.StartChunk(ChunkType.WorldForges);
			BitConverter.Write(workingData, (uint)this.Forges.Count);
			await worldForges.WriteAsync(workingData, 0, SIZEOF_TILECOUNT);
			foreach (var forge in this.Forges)
				await forge.ToStream(worldForges);
		stream.EndChunk();
		/// Signs
		var worldSigns = stream.StartChunk(ChunkType.WorldSigns);
			BitConverter.Write(workingData, (uint)this.Signs.Count);
			await worldSigns.WriteAsync(workingData, 0, SIZEOF_TILECOUNT);
			foreach (var sign in this.Signs)
				await sign.ToStream(worldSigns);
		stream.EndChunk();
		/// Stables
		var worldStables = stream.StartChunk(ChunkType.WorldStables);
			BitConverter.Write(workingData, (uint)this.Stables.Count);
			await worldStables.WriteAsync(workingData, 0, SIZEOF_TILECOUNT);
			foreach (var stable in this.Stables)
				await stable.ToStream(worldStables);
		stream.EndChunk();
		/// Labs
		var worldLabs = stream.StartChunk(ChunkType.WorldLabs);
			BitConverter.Write(workingData, (uint)this.Labs.Count);
			await worldLabs.WriteAsync(workingData, 0, SIZEOF_TILECOUNT);
			foreach (var lab in this.Labs)
				await lab.ToStream(worldLabs);
		stream.EndChunk();
		/// Shelves
		var worldShelves = stream.StartChunk(ChunkType.WorldShelves);
			BitConverter.Write(workingData, (uint)this.Shelves.Count);
			await worldShelves.WriteAsync(workingData, 0, SIZEOF_TILECOUNT);
			foreach (var shelf in this.Shelves)
				await shelf.ToStream(worldShelves);
		stream.EndChunk();
		/// Plants
		var worldPlants = stream.StartChunk(ChunkType.WorldPlants);
			BitConverter.Write(workingData, (uint)0);
			await worldPlants.WriteAsync(workingData, 0, SIZEOF_TILECOUNT);
			//BitConverter.Write(workingData, (uint)this.Plants.Count);
			//await worldPlants.WriteAsync(workingData, 0, SIZEOF_TILECOUNT);
			//foreach (var plant in this.Plants)
			//	await plant.ToStream(worldPlants);
		stream.EndChunk();
		/// Plant: Fruits
		var worldFruits = stream.StartChunk(ChunkType.WorldFruits);
			BitConverter.Write(workingData, (uint)this.Fruits.Count);
			await worldFruits.WriteAsync(workingData, 0, SIZEOF_TILECOUNT);
			foreach (var fruit in this.Fruits)
				await fruit.ToStream(worldFruits);
		stream.EndChunk();
		/// Plant: Decay
		var worldPlantDecay = stream.StartChunk(ChunkType.WorldPlantDecay);
			BitConverter.Write(workingData, (uint)0);
			await worldPlantDecay.WriteAsync(workingData, 0, SIZEOF_TILECOUNT);
			//BitConverter.Write(workingData, (uint)this.PlantDecay.Count);
			//await worldPlantDecay.WriteAsync(workingData, 0, SIZEOF_TILECOUNT);
			//foreach (var planetDecay in this.PlantDecay)
			//	await planetDecay.ToStream(worldPlantDecay);
		stream.EndChunk();
		/// Locks
		var worldLocks = stream.StartChunk(ChunkType.WorldLocks);
			BitConverter.Write(workingData, (uint)this.Locks.Count);
			await worldLocks.WriteAsync(workingData, 0, SIZEOF_TILECOUNT);
			foreach (var _lock in this.Locks)
				await _lock.ToStream(worldLocks);
		stream.EndChunk();
		/// Fluid
		var worldFluid = stream.StartChunk(ChunkType.WorldFluid);
			BitConverter.Write(workingData, (ulong)0, 0);
			await worldFluid.WriteAsync(workingData, 0, SIZEOF_FLUID);
		stream.EndChunk();
		/// Circuitry
		var worldCircuitry = stream.StartChunk(ChunkType.WorldCircuitry);
			BitConverter.Write(workingData, (ulong)0, 0);
			BitConverter.Write(workingData, (ulong)0, 8);
			await worldCircuitry.WriteAsync(workingData, 0, SIZEOF_CIRCUITRY);
		stream.EndChunk();
		/// Mobs
		var worldMobs = stream.StartChunk(ChunkType.WorldMobs);
			BitConverter.Write(workingData, (uint)this.Mobs.Count);
			await worldMobs.WriteAsync(workingData, 0, SIZEOF_TILECOUNT);
			foreach (var mob in this.Mobs)
				await mob.ToStream(worldMobs);
		stream.EndChunk();
		/// Padding
		if (this.Gamemode == Gamemode.Creative || this.Gamemode == Gamemode.Flat)
			stream.AddEmptyChunk();
	}
	/* Static Methods */
	public static async Task<World> Load(string filePath)
	{
		using (var reader = await ArchiverStream.Reader(filePath))
			return await World.FromStream(reader);
	}
	public static async Task<World> FromStream(ArchiverStream stream)
	{
		if (stream.Type != ArchiverType.World && stream.CanRead)
			throw new ArgumentException($"Expected world stream, found {stream.Type} stream");
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
		/// Skyline
		if (!stream.AtChunk(ChunkType.WorldSkyline))
			stream.JumpToChunk(ChunkType.WorldSkyline);
		var skyline = new ushort[worldSize.width];
		for (var i = 0; i < skyline.Length / (workingData.Length / 2); ++i)
		{
			bytesRead = 0;
			while (bytesRead < workingData.Length)
				bytesRead += await stream.ReadAsync(workingData, bytesRead, workingData.Length - bytesRead);
			for (var x = 0; x < workingData.Length / 2; ++x)
			{
				var index = x + (i * (workingData.Length / 2));
				var offset = x * 2;
				skyline[index] = BitConverter.GetUInt16(workingData, offset);
			}
		}
		/// Blocks
		if (!stream.AtChunk(ChunkType.WorldBlocks))
			stream.JumpToChunk(ChunkType.WorldBlocks);
		bytesRead = 0;
		TileMap blockmap;
		using (var blockStream = new MemoryStream())
		{
			blockStream.SetLength(stream.GetChunkSize(ChunkType.WorldBlocks));
			while (bytesRead < blockStream.Length)
				bytesRead += await stream.ReadAsync(blockStream.GetBuffer(), bytesRead, (int)blockStream.Length - bytesRead);
			blockmap = await TileMap.FromStream(blockStream, worldSize, stream.IsChunkCompressed(ChunkType.WorldBlocks));
		}
		/// Fog
		if (gamemode == Gamemode.Survival || gamemode == Gamemode.Adventure)
		{
			if (!stream.AtChunk(ChunkType.WorldFog))
				stream.JumpToChunk(ChunkType.WorldFog);
		}
		/// Time
		if (!stream.AtChunk(ChunkType.WorldTime))
			stream.JumpToChunk(ChunkType.WorldTime);
		bytesRead = 0;
		while (bytesRead < SIZEOF_TIME)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TIME - bytesRead);
		var ticks = BitConverter.GetUInt32(workingData, 0);
		var phase = (DayPhase)workingData[4];
		/// Weather
		if (!stream.AtChunk(ChunkType.WorldWeather))
			stream.JumpToChunk(ChunkType.WorldWeather);
		bytesRead = 0;
		while (bytesRead < SIZEOF_WEATHER)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_WEATHER - bytesRead);
		var poissonSum     = BitConverter.GetFloat32(workingData, 0);
		var weather        = (Weather)workingData[4];
		var poissonSkipped = BitConverter.GetUInt8(workingData,   5);
		/// Chests
		if (!stream.AtChunk(ChunkType.WorldChests))
			stream.JumpToChunk(ChunkType.WorldChests);
		bytesRead = 0;
		while (bytesRead < SIZEOF_TILECOUNT)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TILECOUNT - bytesRead);
		var chestCount = BitConverter.GetUInt32(workingData, 0);
		var chests = new Chest[chestCount];
		for (var i = 0; i < chests.Length; ++i)
			chests[i] = await Chest.FromStream(stream);
		/// Forges
		if (!stream.AtChunk(ChunkType.WorldForges))
			stream.JumpToChunk(ChunkType.WorldForges);
		bytesRead = 0;
		while (bytesRead < SIZEOF_TILECOUNT)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TILECOUNT - bytesRead);
		var forgeCount = BitConverter.GetUInt32(workingData, 0);
		var forges = new Forge[forgeCount];
		for (var i = 0; i < forges.Length; ++i)
			forges[i] = await Forge.FromStream(stream);
		/// Signs
		if (!stream.AtChunk(ChunkType.WorldSigns))
			stream.JumpToChunk(ChunkType.WorldSigns);
		bytesRead = 0;
		while (bytesRead < SIZEOF_TILECOUNT)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TILECOUNT - bytesRead);
		var signCount = BitConverter.GetUInt32(workingData, 0);
		var signs = new Sign[signCount];
		for (var i = 0; i < signs.Length; ++i)
			signs[i] = await Sign.FromStream(stream);
		/// Stables
		if (!stream.AtChunk(ChunkType.WorldStables))
			stream.JumpToChunk(ChunkType.WorldStables);
		bytesRead = 0;
		while (bytesRead < SIZEOF_TILECOUNT)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TILECOUNT - bytesRead);
		var stableCount = BitConverter.GetUInt32(workingData, 0);
		var stables = new Stable[stableCount];
		for (var i = 0; i < stables.Length; ++i)
			stables[i] = await Stable.FromStream(stream);
		/// Labs
		if (!stream.AtChunk(ChunkType.WorldLabs))
			stream.JumpToChunk(ChunkType.WorldLabs);
		bytesRead = 0;
		while (bytesRead < SIZEOF_TILECOUNT)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TILECOUNT - bytesRead);
		var labCount = BitConverter.GetUInt32(workingData, 0);
		var labs = new Lab[labCount];
		for (var i = 0; i < labs.Length; ++i)
			labs[i] = await Lab.FromStream(stream);
		/// Shelves
		if (!stream.AtChunk(ChunkType.WorldShelves))
			stream.JumpToChunk(ChunkType.WorldShelves);
		bytesRead = 0;
		while (bytesRead < SIZEOF_TILECOUNT)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TILECOUNT - bytesRead);
		var shelfCount = BitConverter.GetUInt32(workingData, 0);
		var shelves = new Shelf[shelfCount];
		for (var i = 0; i < shelves.Length; ++i)
			shelves[i] = await Shelf.FromStream(stream);
		/// Plants
		if (!stream.AtChunk(ChunkType.WorldPlants))
			stream.JumpToChunk(ChunkType.WorldPlants);
		bytesRead = 0;
		while (bytesRead < SIZEOF_TILECOUNT)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TILECOUNT - bytesRead);
		var plantCount = BitConverter.GetUInt32(workingData, 0);
		Console.WriteLine($"Plant[Size: {stream.GetChunkSize(ChunkType.WorldPlants)} | Count: {plantCount}]");
		//var plants = new Plant[plantCount];
		//for (var i = 0; i < plants.Length; ++i)
		//	plants[i] = await Plant.FromStream(stream);
		/// Plant: Fruits
		if (!stream.AtChunk(ChunkType.WorldFruits))
			stream.JumpToChunk(ChunkType.WorldFruits);
		bytesRead = 0;
		while (bytesRead < SIZEOF_TILECOUNT)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TILECOUNT - bytesRead);
		var fruitCount = BitConverter.GetUInt32(workingData, 0);
		var fruits = new Fruit[fruitCount];
		for (var i = 0; i < fruits.Length; ++i)
			fruits[i] = await Fruit.FromStream(stream);
		/// Plant: Decay
		if (!stream.AtChunk(ChunkType.WorldPlantDecay))
			stream.JumpToChunk(ChunkType.WorldPlantDecay);
		bytesRead = 0;
		while (bytesRead < SIZEOF_TILECOUNT)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TILECOUNT - bytesRead);
		var plantDecayCount = BitConverter.GetUInt32(workingData, 0);
		Console.WriteLine($"Plant Decay Count: {plantDecayCount}");
		var decayData = new byte[stream.GetChunkSize(ChunkType.WorldPlantDecay) - SIZEOF_TILECOUNT];
		//var plantDecay = new PlantDecay[planetDecayCount];
		//for (var i = 0; i < plantDecay.Length; ++i)
		//	PlantDecay[i] = await PlantDecay.FromStream(stream);
		/// Locks
		if (!stream.AtChunk(ChunkType.WorldLocks))
			stream.JumpToChunk(ChunkType.WorldLocks);
		bytesRead = 0;
		while (bytesRead < SIZEOF_TILECOUNT)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TILECOUNT - bytesRead);
		var lockCount = BitConverter.GetUInt32(workingData, 0);
		var locks = new Lock[lockCount];
		for (var i = 0; i < locks.Length; ++i)
			locks[i] = await Lock.FromStream(stream);
		/// Fluid
		if (!stream.AtChunk(ChunkType.WorldFluid))
			stream.JumpToChunk(ChunkType.WorldFluid);
		bytesRead = 0;
		while (bytesRead < SIZEOF_FLUID)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_FLUID - bytesRead);
		var fluidRemainingSize = stream.GetChunkSize(ChunkType.WorldFluid) - SIZEOF_FLUID;
		stream.Position += fluidRemainingSize;
		/// Circuitry
		if (!stream.AtChunk(ChunkType.WorldCircuitry))
			stream.JumpToChunk(ChunkType.WorldCircuitry);
		bytesRead = 0;
		while (bytesRead < SIZEOF_CIRCUITRY)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_CIRCUITRY - bytesRead);
		var circuitryRemainingSize = stream.GetChunkSize(ChunkType.WorldCircuitry) - SIZEOF_CIRCUITRY;
		stream.Position += circuitryRemainingSize;
		/// Mobs
		if (!stream.AtChunk(ChunkType.WorldMobs))
			stream.JumpToChunk(ChunkType.WorldMobs);
		bytesRead = 0;
		while (bytesRead < SIZEOF_TILECOUNT)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TILECOUNT - bytesRead);
		var mobCount = BitConverter.GetUInt32(workingData, 0);
		var mobs = new Mob[mobCount];
		for (var i = 0; i < mobs.Length; ++i)
			mobs[i] = await Mob.FromStream(stream);
		/// World
		return new World(
			id, version, lastPlayed, name, author, playerLocation, spawnLocation,
			planet, season, gamemode, initSize, skySize, language, skyline, blockmap,
			ticks, phase, poissonSum, weather, poissonSkipped, chests, forges, signs,
			stables, labs, shelves, fruits, locks, mobs 
		);
	}
	/* Properties */
	// Info
	public readonly Guid Id;
	public readonly Version Version;
	public DateTime LastPlayed;
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
	public (ushort Width, ushort Height) Size { get { return (this.BlockMap.Width, this.BlockMap.Height); }}
	public (ushort X, ushort Y) Player;
	public (ushort X, ushort Y) Spawn;
	public Planet Planet;
	public Season Season;
	public Gamemode Gamemode;
	public GenSize InitSize;
	public GenSize SkySize;
	private string _Language;
	public string Language {
		get { return this._Language; }
		set {
			if (String.IsNullOrEmpty(value)) return;
			else if (value.Length < SIZEOF_LANGUAGE) this._Language = value;
			else this._Language = value.Substring(0, SIZEOF_LANGUAGE - 1);
		}
	}
	// Skyline
	public ushort[] Skyline { get; private set; }
	// Blocks
	public readonly TileMap BlockMap;
	public Tile[,] Blocks { get { return this.BlockMap.Tiles; }}
	// Time
	public uint Ticks = 0;
	public DayPhase Phase = DayPhase.Day;
	// Weather
	public float PoissonSum = 0.0f;
	public Weather Weather = Weather.None;
	public byte PoissonSkipped = 0;
	// Containers
	public readonly List<Chest>  Chests         = new List<Chest>();
	public readonly List<Forge>  Forges         = new List<Forge>();
	public readonly List<Sign>   Signs          = new List<Sign>();
	public readonly List<Stable> Stables        = new List<Stable>();
	public readonly List<Lab>    Labs           = new List<Lab>();
	public readonly List<Shelf>  Shelves        = new List<Shelf>();
	//public readonly List<Plant>  Plants         = new List<Plant>();
	public readonly List<Fruit>  Fruits         = new List<Fruit>();
	//public readonly List<PlantDecay> PlantDecay = new List<PlantDecay>();
	public readonly List<Lock>   Locks          = new List<Lock>();
	public readonly List<Mob> Mobs              = new List<Mob>();
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
	private const byte SIZEOF_TIME      =     8;
	private const byte SIZEOF_WEATHER   =     8;
	private const byte SIZEOF_TILECOUNT =     4;
	private const byte SIZEOF_FLUID     =     8;
	private const byte SIZEOF_CIRCUITRY =    16;
}
