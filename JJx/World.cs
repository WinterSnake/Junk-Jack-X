/*
	Junk Jack X: World

	Segment Breakdown:
	-----------------------------------------------------------------------------------------------------------------------
	:<Info>
	Segment[0xF0  :  0xFF] = UUID                   | Length: 16  (0x10) | Type: uuid
	Segment[0x100 : 0x103] = Last Played Timestamp  | Length: 4   (0x4)  | Type: DateTime[uint32]
	Segment[0x104 : 0x107] = Game Version           | Length: 4   (0x4)  | Type: uint32             | Parent: JJx.Version
	Segment[0x108 : 0x127] = Name                   | Length: 32  (0x20) | Type: char*
	Segment[0x128 : 0x137] = Author                 | Length: 16  (0x10) | Type: char*
	Segment[0x138 : 0x139] = World.Width            | Length: 2   (0x2)  | Type: uint16             | Parent: Blocks.Width
	Segment[0x13A : 0x13B] = World.Height           | Length: 2   (0x2)  | Type: uint16             | Parent: Blocks.Height
	Segment[0x13C : 0x13D] = Player.X               | Length: 2   (0x2)  | Type: uint16             | Parent: Player.X
	Segment[0x13E : 0x13F] = Player.Y               | Length: 2   (0x2)  | Type: uint16             | Parent: Player.Y
	Segment[0x140 : 0x141] = Spawn.X                | Length: 2   (0x2)  | Type: uint16             | Parent: Spawn.X
	Segment[0x142 : 0x143] = Spawn.Y                | Length: 2   (0x2)  | Type: uint16             | Parent: Spawn.Y
	Segment[0x144 : 0x147] = Planet                 | Length: 4   (0x4)  | Type: enum flag[uint32]
	Segment[0x148]         = Season                 | Length: 1   (0x1)  | Type: enum[uint8]        | Parent: Season
	Segment[0x149]         = Gamemode               | Length: 1   (0x1)  | Type: enum[uint8]        | Parent: Gamemode
	Segment[0x14A]         = World Size             | Length: 1   (0x1)  | Type: enum[uint8]        | Parent: InitSize
	Segment[0x14B]         = Sky Size               | Length: 1   (0x1)  | Type: enum[uint8]        | Parent: InitSize
	Segment[0x14C : 0x14F] = Language(?)            | Length: 4   (0x4)  | Type: char[4] (?)
	Segment[0x150 : 0x1CF] = Padding                | Length: 128 (0x80) | Type: uint32[32] = {0}
	:<Border>

	-----------------------------------------------------------------------------------------------------------------------

	Written By: Ryan Smith
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.IO.Compression;

namespace JJx;

public enum Gamemode : byte
{
	Survival = 0x0,
	Creative,
	Flat
}

public sealed class World
{
	/* Constructors */
	public World(
		string name, InitSize worldSize, string author = "author", InitSize skySize = InitSize.Normal,
		Gamemode gamemode = Gamemode.Creative, Season season = Season.None, Planet planet = Planet.Terra,
		(ushort, ushort)? customSize = null
	)
	{
		this.Id = Guid.NewGuid();
		this.LastPlayed = DateTime.Now;
		this.Version = Version.Latest;
		this.Name = name;
		this.Author = author;
		switch (worldSize)
		{
			case InitSize.Tiny:
				this.Size = (512, 128); break;
			case InitSize.Small:
				this.Size = (768, 256); break;
			case InitSize.Normal:
				this.Size = (1024, 256); break;
			case InitSize.Large:
				this.Size = (2048, 384); break;
			case InitSize.Huge:
				this.Size = (4096, 512); break;
			case InitSize.Custom:
			default:
				// Minimum working size: 128 x 64
				this.Size = customSize.Value; break;
		}
		this.Player = (0x0, 1);
		this.Spawn  = (0x0, 1);
		this.Season = season;
		this.Gamemode = gamemode;
		this.WorldInitSize = worldSize;
		this.SkyInitSize = skySize;
		this.Borders = new ushort[this.Size.Width];
		for (var i = 0; i < this.Borders.Length; ++i)
			this.Borders[i] = (ushort)(this.Size.Height / 2);
		this.Blocks = new Tile[this.Size.Width, this.Size.Height];
		for (var x = 0; x < this.Size.Width; ++x)
		{
			for (var y = 0; y < this.Size.Height; ++y)
			{
				if (y == 0)
					this.Blocks[x, y] = new Tile(0x0054, 0x0054);
				else
					this.Blocks[x, y] = new Tile(0x0000, 0x0000);
			}
		}
	}
	private World(
		Guid id, DateTime lastPlayed, Version version, string name, string author,
		(ushort, ushort) size, (ushort, ushort) player, (ushort, ushort) spawn, Planet planet,
		Season season, Gamemode gamemode, InitSize worldInitSize, InitSize skyInitSize, ushort[] borders,
		Tile[,] blocks, Chest[] chests, Forge[] forges, Sign[] signs, Stable[] stables, Lab[] labs,
		Shelf[] shelves, /*Plant[] plants,*/ Lock[] locks, Entity[] entities
	)
	{
		this.Id = id;
		this.LastPlayed = lastPlayed;
		this.Version = version;
		this._Name = name;
		this._Author = author;
		this.Size = size;
		this.Player = player;
		this.Spawn = spawn;
		this.Planet = planet;
		this.Season = season;
		this.Gamemode = gamemode;
		this.WorldInitSize = worldInitSize;
		this.SkyInitSize = skyInitSize;
		this.Borders = borders;
		this.Blocks = blocks;
		this.Chests.AddRange(chests);
		this.Forges.AddRange(forges);
		this.Signs.AddRange(signs);
		this.Stables.AddRange(stables);
		this.Labs.AddRange(labs);
		this.Shelves.AddRange(shelves);
		//this.Plants.AddRange(plants);
		this.Locks.AddRange(locks);
		this.Entities.AddRange(entities);
	}
	/* Instance Methods */
	public ulong GetTileFlatPosition(ushort x, ushort y) => (ulong)((y + x * this.Size.Height) * Tile.SIZE);
	public async Task Save(string path)
	{
		using var stream = await ArchiverStream.Writer(path, ArchiverType.Map);
		var workingData = new byte[BUFFER_SIZE];
		/// Info
		using (var chunk = stream.NewChunk(ChunkType.WorldInfo))
		{
			// Uuid
			var uuid = this.Id.ToByteArray();
			for (var i = uuid.Length - 1; i <= 0; --i)
				workingData[uuid.Length - i] = uuid[i];
			await chunk.WriteAsync(uuid, 0, uuid.Length);
			// Last Played
			Utilities.ByteConverter.Write(new Span<byte>(workingData), this.LastPlayed,    0);
			// Game Version
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (uint)this.Version, 4);
			await chunk.WriteAsync(workingData, 0, 8);
			// Name
			Utilities.ByteConverter.Write(new Span<byte>(workingData), this._Name, length: SIZEOF_NAME);
			await chunk.WriteAsync(workingData, 0, SIZEOF_NAME);
			// Author
			Utilities.ByteConverter.Write(new Span<byte>(workingData), this._Author, length: SIZEOF_AUTHOR);
			await chunk.WriteAsync(workingData, 0, SIZEOF_AUTHOR);
			// World Size
			Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Size.Width,  0);
			Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Size.Height, 2);
			// Player Location
			Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Player.X, 4);
			Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Player.Y, 6);
			// Spawn Location
			Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Spawn.X,  8);
			Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Spawn.Y, 10);
			// Planet
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (uint)this.Planet, 12);
			// Season
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (byte)this.Season, 16);
			// Gamemode
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (byte)this.Gamemode, 17);
			// World Init Size
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (byte)this.WorldInitSize, 18);
			// Sky Init Size
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (byte)this.SkyInitSize, 19);
			// =UNKNOWN=
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (uint)0, 20);
			await chunk.WriteAsync(workingData, 0, 24);
			// Padding
			workingData = new byte[SIZEOF_PADDING];
			await chunk.WriteAsync(workingData, 0, workingData.Length);
		}
		/// Border
		using (var chunk = stream.NewChunk(ChunkType.WorldBorders))
		{
			workingData = new byte[this.Borders.Length * 2];
			for (var i = 0; i < this.Borders.Length; ++i)
				Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Borders[i], i * 2);
			await chunk.WriteAsync(workingData, 0, workingData.Length);
		}
		/// Blocks
		using (var chunk = stream.NewChunk(ChunkType.WorldBlocks, 1, compressed: true))
		{
			using var blocksDecompressedStream = new MemoryStream(this.Size.Width * this.Size.Height * Tile.SIZE);
			for (var x = 0; x < this.Size.Width; ++x)
				for (var y = 0; y < this.Size.Height; ++y)
					await this.Blocks[x, y].ToStream(blocksDecompressedStream);
			blocksDecompressedStream.Position = 0;
			using (var blocksCompressionStream = new GZipStream(chunk, CompressionLevel.Optimal, true))
				await blocksDecompressedStream.CopyToAsync(blocksCompressionStream);
		}
		/// Fog
		if (this.Gamemode == Gamemode.Survival)
		{
			using (var chunk = stream.NewChunk(ChunkType.WorldFog, compressed: true))
			{

			}
		}
		/// Time
		workingData = new byte[BUFFER_SIZE];
		using (var chunk = stream.NewChunk(ChunkType.WorldTime))
		{
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (ulong)0, 0);
			await chunk.WriteAsync(workingData, 0, 8);
		}
		/// Weather
		using (var chunk = stream.NewChunk(ChunkType.WorldWeather))
		{
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (ulong)0, 0);
			await chunk.WriteAsync(workingData, 0, 8);
		}
		/// Chests
		using (var chunk = stream.NewChunk(ChunkType.WorldChests))
		{
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (uint)this.Chests.Count, 0);
			await chunk.WriteAsync(workingData, 0, 4);
			foreach (var chest in this.Chests)
				await chest.ToStream(chunk);
		}
		/// Forges
		using (var chunk = stream.NewChunk(ChunkType.WorldForges))
		{
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (uint)this.Forges.Count, 0);
			await chunk.WriteAsync(workingData, 0, 4);
			foreach (var forge in this.Forges)
				await forge.ToStream(chunk);
		}
		/// Signs
		using (var chunk = stream.NewChunk(ChunkType.WorldSigns))
		{
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (uint)this.Signs.Count, 0);
			await chunk.WriteAsync(workingData, 0, 4);
			foreach (var sign in this.Signs)
				await sign.ToStream(chunk);
		}
		/// Stables
		using (var chunk = stream.NewChunk(ChunkType.WorldStables))
		{
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (uint)this.Stables.Count, 0);
			await chunk.WriteAsync(workingData, 0, 4);
			foreach (var stable in this.Stables)
				await stable.ToStream(chunk);
		}
		/// Labs
		using (var chunk = stream.NewChunk(ChunkType.WorldLabs))
		{
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (uint)this.Labs.Count, 0);
			await chunk.WriteAsync(workingData, 0, 4);
			foreach (var lab in this.Labs)
				await lab.ToStream(chunk);
		}
		/// Shelves
		using (var chunk = stream.NewChunk(ChunkType.WorldShelves))
		{
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (uint)this.Shelves.Count, 0);
			await chunk.WriteAsync(workingData, 0, 4);
			foreach (var shelf in this.Shelves)
				await shelf.ToStream(chunk);
		}
		/// Plants
		using (var chunk = stream.NewChunk(ChunkType.WorldPlants))
		{
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (uint)0, 0);
			await chunk.WriteAsync(workingData, 0, 4);
			//Utilities.ByteConverter.Write(new Span<byte>(workingData), (uint)this.Plants.Count, 0);
			//await chunk.WriteAsync(workingData, 0, 4);
			//foreach (var plant in this.Plants)
			//	await plant.ToStream(chunk);
		}
		/// =UNKNOWN=
		using (var chunk = stream.NewChunk(ChunkType.WorldUnknown01))
		{
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (uint)0, 0);
			await chunk.WriteAsync(workingData, 0, 4);
		}
		/// =UNKNOWN=
		using (var chunk = stream.NewChunk(ChunkType.WorldUnknown02))
		{
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (uint)0, 0);
			await chunk.WriteAsync(workingData, 0, 4);
		}
		/// Locks
		using (var chunk = stream.NewChunk(ChunkType.WorldLocks))
		{
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (uint)this.Locks.Count, 0);
			await chunk.WriteAsync(workingData, 0, 4);
			foreach (var @lock in this.Locks)
				await @lock.ToStream(chunk);
		}
		/// Fluid
		using (var chunk = stream.NewChunk(ChunkType.WorldFluid))
		{
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (ulong)0, 0);
			await chunk.WriteAsync(workingData, 0, 8);
		}
		/// Circuitry
		using (var chunk = stream.NewChunk(ChunkType.WorldCircuitry))
		{
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (ulong)0, 0);
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (ulong)0, 0);
			await chunk.WriteAsync(workingData, 0, 16);
		}
		/// Entities
		using (var chunk = stream.NewChunk(ChunkType.WorldEntities))
		{
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (uint)this.Entities.Count, 0);
			await chunk.WriteAsync(workingData, 0, 4);
			foreach (var entity in this.Entities)
				await entity.ToStream(chunk);
		}
		/// Pad Chunk
		if (this.Gamemode == Gamemode.Creative || this.Gamemode == Gamemode.Flat)
			stream.EmptyChunk();
	}
	public async Task LoadBlocks(string path, bool compressed = false)
	{
		using var stream = File.Open(path, FileMode.Open, FileAccess.Read);
		if (!compressed)
		{
			for (var x = 0; x < this.Size.Width; ++x)
				for (var y = 0; y < this.Size.Height; ++y)
					this.Blocks[x, y] = await Tile.FromStream(stream);
			return;
		}
	}
	public async Task SaveBlocks(string path, bool compressed = false)
	{
		using var stream = File.Open(path, FileMode.Create, FileAccess.Write);
		if (!compressed)
		{
			for (var x = 0; x < this.Size.Width; ++x)
				for (var y = 0; y < this.Size.Height; ++y)
					await this.Blocks[x, y].ToStream(stream);
			return;
		}
	}
	/* Static Methods */
	public static async Task<World> Load(string path)
	{
		using var stream = await ArchiverStream.Reader(path);
		if (stream.Type != ArchiverType.Map)
			throw new ArgumentException($"Expected world stream, found {stream.Type} stream");
		// DEBUG
		#if (PRINT_CHUNKS)
			Console.WriteLine(String.Join("\n", stream.GetChunkStrings()));
		#endif
		int bytesRead = 0;
		var workingData = new byte[BUFFER_SIZE];
		/// Info
		// Uuid
		while (bytesRead < SIZEOF_UUID)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_UUID - bytesRead);
		var id = new Guid(new Span<byte>(workingData, 0, SIZEOF_UUID));
		// Last Played
		bytesRead = 0;
		while (bytesRead < SIZEOF_TIMESTAMP)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TIMESTAMP - bytesRead);
		var lastPlayed = Utilities.ByteConverter.GetDateTime(new Span<byte>(workingData));
		// Game Version
		bytesRead = 0;
		while (bytesRead < SIZEOF_VERSION)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_VERSION - bytesRead);
		var version = (Version)Utilities.ByteConverter.GetUInt32(new Span<byte>(workingData));
		// Name
		bytesRead = 0;
		while (bytesRead < SIZEOF_NAME)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_NAME - bytesRead);
		var name = Utilities.ByteConverter.GetString(new Span<byte>(workingData));
		// Author
		bytesRead = 0;
		while (bytesRead < SIZEOF_AUTHOR)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_AUTHOR - bytesRead);
		var author = Utilities.ByteConverter.GetString(new Span<byte>(workingData));
		// {World Size, Player Location, Spawn Location}
		bytesRead = 0;
		while (bytesRead < SIZEOF_POSITIONS)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_POSITIONS - bytesRead);
			// World Size
			(ushort width, ushort height) worldSize = (
				Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData),  0),
				Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData),  2)
			);
			// Player Location
			var playerPos = (
				Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData),  4),
				Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData),  6)
			);
			// Spawn Location
			var spawnPos = (
				Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData),  8),
				Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData), 10)
			);
		// Planet
		bytesRead = 0;
		while (bytesRead < SIZEOF_PLANET)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_PLANET - bytesRead);
		var planet = (Planet)Utilities.ByteConverter.GetUInt32(new Span<byte>(workingData));
		// Season
		var season = (Season)((byte)stream.ReadByte());
		// Gamemode
		var gamemode = (Gamemode)((byte)stream.ReadByte());
		// World Init Size
		var worldInitSize = (InitSize)((byte)stream.ReadByte());
		// Sky Init Size
		var skyInitSize = (InitSize)((byte)stream.ReadByte());
		// =UNKNOWN=
		stream.Seek(4, SeekOrigin.Current);
		// Padding
		stream.Seek(SIZEOF_PADDING, SeekOrigin.Current);
		/// Border
		var borders = new ushort[worldSize.width];
		for (var i = 0; i < borders.Length / (workingData.Length / 2); ++i)
		{
			bytesRead = 0;
			while (bytesRead < workingData.Length)
				bytesRead += await stream.ReadAsync(workingData, bytesRead, workingData.Length - bytesRead);
			for (var j = 0; j < workingData.Length / 2; ++j)
			{
				var index = j + (i * (workingData.Length / 2));
				var offset = j * 2;
				borders[index] = Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData), offset);
			}
		}
		/// Blocks
		bytesRead = 0;
		var blocks = new Tile[worldSize.width, worldSize.height];
		var blocksCompressedSize = stream.GetChunkSize(ChunkType.WorldBlocks);
		using (var blocksCompressedStream = new MemoryStream((int)blocksCompressedSize))
		{
			// Clone block compressed data to memory stream
			blocksCompressedStream.SetLength(blocksCompressedSize.Value);
			while (bytesRead < blocksCompressedSize)
				bytesRead += await stream.ReadAsync(blocksCompressedStream.GetBuffer(), bytesRead, (int)blocksCompressedSize - bytesRead);
			// Decompress stream
			using (var blocksDecompressionStream = new GZipStream(blocksCompressedStream, CompressionMode.Decompress))
			{
				for (var x = 0; x < worldSize.width; ++x)
					for (var y = 0; y < worldSize.height; ++y)
						blocks[x, y] = await Tile.FromStream(blocksDecompressionStream);
			}
		}
		/// Fog
		if (stream.IsAtChunk(ChunkType.WorldFog))
		{
			Console.WriteLine($"Position: {stream.Position:X8} | Postion = Fog Location: {stream.IsAtChunk(ChunkType.WorldFog)} | Size: {stream.GetChunkSize(ChunkType.WorldFog):X4}");
			stream.Seek(stream.GetChunkSize(ChunkType.WorldFog).Value, SeekOrigin.Current);
		}
		/// Time
		Console.WriteLine($"Position: {stream.Position:X8} | Postion = Time Location: {stream.IsAtChunk(ChunkType.WorldTime)} | Size: {stream.GetChunkSize(ChunkType.WorldTime):X4}");
		bytesRead = 0;
		while (bytesRead < SIZEOF_TIME)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TIME - bytesRead);
		/// Weather
		Console.WriteLine($"Position: {stream.Position:X8} | Postion = Weather Location: {stream.IsAtChunk(ChunkType.WorldWeather)} | Size: {stream.GetChunkSize(ChunkType.WorldWeather):X4}");
		bytesRead = 0;
		while (bytesRead < SIZEOF_WEATHER)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_WEATHER - bytesRead);
		/// Chests
		bytesRead = 0;
		while (bytesRead < SIZEOF_TILECOUNT)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TILECOUNT - bytesRead);
		var chestCount = Utilities.ByteConverter.GetUInt32(new Span<byte>(workingData));
		var chests = new Chest[chestCount];
		for (var i = 0; i < chests.Length; ++i)
			chests[i] = await Chest.FromStream(stream);
		/// Forges
		bytesRead = 0;
		while (bytesRead < SIZEOF_TILECOUNT)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TILECOUNT - bytesRead);
		var forgeCount = Utilities.ByteConverter.GetUInt32(new Span<byte>(workingData));
		var forges = new Forge[forgeCount];
		for (var i = 0; i < forges.Length; ++i)
			forges[i] = await Forge.FromStream(stream);
		/// Signs
		bytesRead = 0;
		while (bytesRead < SIZEOF_TILECOUNT)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TILECOUNT - bytesRead);
		var signCount = Utilities.ByteConverter.GetUInt32(new Span<byte>(workingData));
		var signs = new Sign[signCount];
		for (var i = 0; i < signs.Length; ++i)
			signs[i] = await Sign.FromStream(stream);
		/// Stables
		Console.WriteLine($"Position: {stream.Position:X8} | Postion = Stables Location: {stream.IsAtChunk(ChunkType.WorldStables)} | Size: {stream.GetChunkSize(ChunkType.WorldStables):X4}");
		bytesRead = 0;
		while (bytesRead < SIZEOF_TILECOUNT)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TILECOUNT - bytesRead);
		var stableCount = Utilities.ByteConverter.GetUInt32(new Span<byte>(workingData));
		Console.WriteLine($"Stable Count: {stableCount}");
		var stables = new Stable[stableCount];
		for (var i = 0; i < stables.Length; ++i)
			stables[i] = await Stable.FromStream(stream);
		stream.Seek(stream.GetChunkSize(ChunkType.WorldStables).Value - SIZEOF_TILECOUNT, SeekOrigin.Current);
		/// Labs
		Console.WriteLine($"Position: {stream.Position:X8} | Postion = Labs Location: {stream.IsAtChunk(ChunkType.WorldLabs)} | Size: {stream.GetChunkSize(ChunkType.WorldLabs):X4}");
		bytesRead = 0;
		while (bytesRead < SIZEOF_TILECOUNT)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TILECOUNT - bytesRead);
		var labCount = Utilities.ByteConverter.GetUInt32(new Span<byte>(workingData));
		var labs = new Lab[labCount];
		for (var i = 0; i < labs.Length; ++i)
			labs[i] = await Lab.FromStream(stream);
		/// Shelves
		bytesRead = 0;
		while (bytesRead < SIZEOF_TILECOUNT)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TILECOUNT - bytesRead);
		var shelfCount = Utilities.ByteConverter.GetUInt32(new Span<byte>(workingData));
		var shelves = new Shelf[shelfCount];
		for (var i = 0; i < shelves.Length; ++i)
			shelves[i] = await Shelf.FromStream(stream);
		/// Plants
		Console.WriteLine($"Position: {stream.Position:X8} | Postion = Plants Location: {stream.IsAtChunk(ChunkType.WorldPlants)} | Size: {stream.GetChunkSize(ChunkType.WorldPlants):X4}");
		bytesRead = 0;
		while (bytesRead < SIZEOF_TILECOUNT)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TILECOUNT - bytesRead);
		var plantCount = Utilities.ByteConverter.GetUInt32(new Span<byte>(workingData));
		Console.WriteLine($"Plant Count: {plantCount}");
		//var plants = new Plant[plantCount];
		//for (var i = 0; i < plants.Length; ++i)
		//	plants[i] = await Plant.FromStream(stream);
		stream.Seek(stream.GetChunkSize(ChunkType.WorldPlants).Value - SIZEOF_TILECOUNT, SeekOrigin.Current);
		/// =UNKNOWN=
		Console.WriteLine($"Position: {stream.Position:X8} | Postion = UNKOWN Location: {stream.IsAtChunk(ChunkType.WorldUnknown01)} | Size: {stream.GetChunkSize(ChunkType.WorldUnknown01):X4}");
		bytesRead = 0;
		while (bytesRead < SIZEOF_TILECOUNT)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TILECOUNT - bytesRead);
		var unknown01Count = Utilities.ByteConverter.GetUInt32(new Span<byte>(workingData));
		Console.WriteLine($"Unknown 01 Count: {unknown01Count}");
		stream.Seek(stream.GetChunkSize(ChunkType.WorldUnknown01).Value - SIZEOF_TILECOUNT, SeekOrigin.Current);
		/// =UNKNOWN=
		Console.WriteLine($"Position: {stream.Position:X8} | Postion = UNKOWN Location: {stream.IsAtChunk(ChunkType.WorldUnknown02)} | Size: {stream.GetChunkSize(ChunkType.WorldUnknown02):X4}");
		bytesRead = 0;
		while (bytesRead < SIZEOF_TILECOUNT)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TILECOUNT - bytesRead);
		var unknown02Count = Utilities.ByteConverter.GetUInt32(new Span<byte>(workingData));
		Console.WriteLine($"Unknown 02 Count: {unknown02Count}");
		stream.Seek(stream.GetChunkSize(ChunkType.WorldUnknown02).Value - SIZEOF_TILECOUNT, SeekOrigin.Current);
		/// Locks
		Console.WriteLine($"Position: {stream.Position:X8} | Postion = Locks Location: {stream.IsAtChunk(ChunkType.WorldLocks)} | Size: {stream.GetChunkSize(ChunkType.WorldLocks):X4}");
		bytesRead = 0;
		while (bytesRead < SIZEOF_TILECOUNT)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TILECOUNT - bytesRead);
		var lockCount = Utilities.ByteConverter.GetUInt32(new Span<byte>(workingData));
		var locks = new Lock[lockCount];
		for (var i = 0; i < locks.Length; ++i)
			locks[i] = await Lock.FromStream(stream);
		/// Fluid
		Console.WriteLine($"Position: {stream.Position:X8} | Postion = Fluid Location: {stream.IsAtChunk(ChunkType.WorldFluid)} | Size: {stream.GetChunkSize(ChunkType.WorldFluid):X4}");
		stream.Seek(stream.GetChunkSize(ChunkType.WorldFluid).Value, SeekOrigin.Current);
		/// Circuitry
		Console.WriteLine($"Position: {stream.Position:X8} | Postion = Circuitry Location: {stream.IsAtChunk(ChunkType.WorldCircuitry)} | Size: {stream.GetChunkSize(ChunkType.WorldCircuitry):X4}");
		stream.Seek(stream.GetChunkSize(ChunkType.WorldCircuitry).Value, SeekOrigin.Current);
		/// Entities
		Console.WriteLine($"Position: {stream.Position:X8} | Postion = Entities Location: {stream.IsAtChunk(ChunkType.WorldEntities)} | Size: {stream.GetChunkSize(ChunkType.WorldEntities):X4}");
		bytesRead = 0;
		while (bytesRead < SIZEOF_TILECOUNT)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TILECOUNT - bytesRead);
		var entityCount = Utilities.ByteConverter.GetUInt32(new Span<byte>(workingData));
		var entities = new Entity[entityCount];
		for (var i = 0; i < entities.Length; ++i)
			entities[i] = await Entity.FromStream(stream);

		return new World(
			id, lastPlayed, version, name, author, worldSize, playerPos, spawnPos,
			planet, season, gamemode, worldInitSize, skyInitSize, borders, blocks,
			chests, forges, signs, stables, labs, shelves, /*plants,*/ locks, entities
		);
	}
	/* Properties */
	// Info
	public readonly Guid Id;
	public DateTime LastPlayed;
	public readonly Version Version;
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
	public (ushort Width, ushort Height) Size { get; private set; }
	public (ushort X, ushort Y) Player;
	public (ushort X, ushort Y) Spawn;
	public Planet Planet;
	public Season Season;
	public Gamemode Gamemode;
	public InitSize WorldInitSize;
	public InitSize SkyInitSize;
	// Border
	public ushort[] Borders { get; private set; }
	// Blocks
	public Tile[,] Blocks { get; private set; }
	/*
		TODO: Memory Mapped File Implementation
		https://learn.microsoft.com/en-us/dotnet/api/system.io.memorymappedfiles.memorymappedfile?view=net-7.0
		https://learn.microsoft.com/en-us/dotnet/api/system.io.memorymappedfiles.memorymappedviewstream?view=net-7.0
		Implement Block array as memory mapped file with a viewer and getblock/setblock implementation
		
		Separate Blocks class with x/y indexer class
		https://stackoverflow.com/questions/6111049/2d-array-property
	*/
	// Time
	// Weather
	// Containers
	public readonly List<Chest>  Chests   = new List<Chest>();
	public readonly List<Forge>  Forges   = new List<Forge>();
	public readonly List<Sign>   Signs    = new List<Sign>();
	public readonly List<Stable> Stables  = new List<Stable>();
	public readonly List<Lab>    Labs     = new List<Lab>();
	public readonly List<Shelf>  Shelves  = new List<Shelf>();
	//public readonly List<Plant>  Plants   = new List<Plant>();
	public readonly List<Lock>   Locks    = new List<Lock>();
	public readonly List<Entity> Entities = new List<Entity>();
	/* Class Properties */
	private const byte BUFFER_SIZE           =  32;
	private const byte SIZEOF_UUID           =  16;
	private const byte SIZEOF_TIMESTAMP      =   4;
	private const byte SIZEOF_VERSION        =   4;
	private const byte SIZEOF_NAME           =  32;
	private const byte SIZEOF_AUTHOR         =  16;
	private const byte SIZEOF_POSITIONS      = 2 * 6;  // World.Width, World.Height, Player.X, Player.Y, Spawn.X, Spawn.Y
	private const byte SIZEOF_PLANET         =   4;
	private const byte SIZEOF_PADDING        = 128;
	private const byte SIZEOF_TIME           =   8;
	private const byte SIZEOF_WEATHER        =   8;
	private const byte SIZEOF_TILECOUNT      =   4;
}
