/*
	Junk Jack X: Core
	- [Archive]World

	Written By: Ryan Smith
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using JJx.Core.Serialization;

namespace JJx.Core;

public sealed class WorldArchive : IArchive
{
	/* Constructor */
	public WorldArchive(World world) :this(world, null)
	{
		this._IsSkylineLoaded = true;
		this._AreTilesLoaded = true;
		this._IsFogLoaded = true;
		this._IsTimeLoaded = true;
		this._IsWeatherLoaded = true;
		this._IsContainerChestLoaded = true;
		this._IsContainerForgeLoaded = true;
		this._IsContainerSignLoaded = true;
		this._IsContainerStableLoaded = true;
		this._IsContainerLabLoaded = true;
		this._IsContainerShelfLoaded = true;
		this._IsContainerPlantLoaded = true;
		this._IsContainerFruitLoaded = true;
		this._IsContainerDecayLoaded = true;
		this._IsContainerLockLoaded = true;
		this._IsContainerEntityLoaded = true;
		this._IsFluidLoaded = true;
		this._AreCircuitsLoaded = true;
	}
	private WorldArchive(World world, IArchiveReader? reader)
	{
		this._World = world;
		this._Reader = reader;
	}
	/* Instance Methods */
	public void Dispose() => this._Reader?.Dispose();
	// Reading
	private void _Load()
	{
		this._LoadSkyline();
		this._LoadTilemap();
		this._LoadFog();
		this._LoadTime();
		this._LoadWeather();
		this._LoadContainerChest();
		this._LoadContainerForge();
		this._LoadContainerSign();
		this._LoadContainerStable();
		this._LoadContainerLab();
		this._LoadContainerShelf();
		this._LoadContainerPlant();
		this._LoadContainerFruit();
		this._LoadContainerDecay();
		this._LoadContainerLock();
		this._LoadFluid();
		this._LoadCircuits();
		this._LoadContainerEntity();
		this._Reader!.Dispose();
	}
	private void _LoadSkyline()
	{
		if (this._IsSkylineLoaded) return;
		using (var chunk = this._Reader!.GetChunkStream(ArchiverChunkType.WorldSkyline))
		{
			var reader = new JJxReader(chunk);
			reader.ReadSpan(MemoryMarshal.Cast<ushort, byte>(this._World.Skyline));
		}
		this._IsSkylineLoaded = true;
	}
	private void _LoadTilemap()
	{
		if (this._AreTilesLoaded) return;
		using (var chunk = this._Reader!.GetChunkStream(ArchiverChunkType.WorldBlocks))
		{
			var reader = new JJxReader(chunk);
			var tiles = new Tile[this._World.Size.Width * this._World.Size.Height];
			for (var i = 0; i < tiles.Length; ++i)
				tiles[i] = reader.ReadObject<Tile>();
			this._World._Tilemap = new(tiles, this._World.Size);
		}
		this._AreTilesLoaded = true;
	}
	private void _LoadFog()
	{
		if (this._IsFogLoaded) return;
		if (this._Reader!.HasChunk(ArchiverChunkType.WorldFog))
		{
			using (var chunk = this._Reader!.GetChunkStream(ArchiverChunkType.WorldFog))
			{
				var reader = new JJxReader(chunk);
				var fogMap = new byte[this._World.Size.Height / 4 * this._World.Size.Width];
				reader.ReadSpan(fogMap.AsSpan());
				this._World._Fog = fogMap;
			}
		}
		this._IsFogLoaded = true;
	}
	private void _LoadTime()
	{
		if (this._IsTimeLoaded) return;
		using (var chunk = this._Reader!.GetChunkStream(ArchiverChunkType.WorldTime))
		{
			var reader = new JJxReader(chunk);
			reader.ReadSpan(this._World.Time);
		}
		this._IsTimeLoaded = true;
	}
	private void _LoadWeather()
	{
		if (this._IsWeatherLoaded) return;
		using (var chunk = this._Reader!.GetChunkStream(ArchiverChunkType.WorldWeather))
		{
			var reader = new JJxReader(chunk);
			reader.ReadSpan(this._World.Weather);
		}
		this._IsWeatherLoaded = true;
	}
	private void _LoadFluid()
	{
		if (this._IsFluidLoaded) return;
		using (var chunk = this._Reader!.GetChunkStream(ArchiverChunkType.WorldFluid))
		{
			var reader = new JJxReader(chunk);
			var buffer = new byte[chunk.Length];
			reader.ReadSpan(buffer.AsSpan());
			this._World._Fluid = buffer;
		}
		this._IsFluidLoaded = true;
	}
	private void _LoadCircuits()
	{
		if (this._AreCircuitsLoaded) return;
		using (var chunk = this._Reader!.GetChunkStream(ArchiverChunkType.WorldCircuitry))
		{
			var reader = new JJxReader(chunk);
			var buffer = new byte[chunk.Length];
			reader.ReadSpan(buffer.AsSpan());
			this._World._Circuitry = buffer;
		}
		this._AreCircuitsLoaded = true;
	}
	private void _LoadContainerChest()
	{
		if (this._IsContainerChestLoaded) return;
		using (var chunk = this._Reader!.GetChunkStream(ArchiverChunkType.WorldChests))
		{
			var reader = new JJxReader(chunk);
			var count = reader.ReadInt32();
		}
		this._IsContainerChestLoaded = true;
	}
	private void _LoadContainerForge()
	{
		if (this._IsContainerForgeLoaded) return;
		using (var chunk = this._Reader!.GetChunkStream(ArchiverChunkType.WorldForges))
		{
			var reader = new JJxReader(chunk);
			var count = reader.ReadInt32();
		}
		this._IsContainerForgeLoaded = true;
	}
	private void _LoadContainerSign()
	{
		if (this._IsContainerSignLoaded) return;
		using (var chunk = this._Reader!.GetChunkStream(ArchiverChunkType.WorldSigns))
		{
			var reader = new JJxReader(chunk);
			var count = reader.ReadInt32();
			this._World.Signs.EnsureCapacity(count);
			for (var i = 0; i < count; ++i)
				this._World.Signs.Add(reader.ReadObject<Sign>());
		}
		this._IsContainerSignLoaded = true;
	}
	private void _LoadContainerStable()
	{
		if (this._IsContainerStableLoaded) return;
		using (var chunk = this._Reader!.GetChunkStream(ArchiverChunkType.WorldStables))
		{
			var reader = new JJxReader(chunk);
			var count = reader.ReadInt32();
		}
		this._IsContainerStableLoaded = true;
	}
	private void _LoadContainerLab()
	{
		if (this._IsContainerLabLoaded) return;
		using (var chunk = this._Reader!.GetChunkStream(ArchiverChunkType.WorldLabs))
		{
			var reader = new JJxReader(chunk);
			var count = reader.ReadInt32();
		}
		this._IsContainerLabLoaded = true;
	}
	private void _LoadContainerShelf()
	{
		if (this._IsContainerShelfLoaded) return;
		using (var chunk = this._Reader!.GetChunkStream(ArchiverChunkType.WorldShelves))
		{
			var reader = new JJxReader(chunk);
			var count = reader.ReadInt32();
		}
		this._IsContainerShelfLoaded = true;
	}
	private void _LoadContainerPlant()
	{
		if (this._IsContainerPlantLoaded) return;
		using (var chunk = this._Reader!.GetChunkStream(ArchiverChunkType.WorldPlants))
		{
			var reader = new JJxReader(chunk);
			var count = reader.ReadInt32();
		}
		this._IsContainerPlantLoaded = true;
	}
	private void _LoadContainerFruit()
	{
		if (this._IsContainerFruitLoaded) return;
		using (var chunk = this._Reader!.GetChunkStream(ArchiverChunkType.WorldFruits))
		{
			var reader = new JJxReader(chunk);
			var count = reader.ReadInt32();
		}
		this._IsContainerFruitLoaded = true;
	}
	private void _LoadContainerDecay()
	{
		if (this._IsContainerDecayLoaded) return;
		using (var chunk = this._Reader!.GetChunkStream(ArchiverChunkType.WorldPlantDecay))
		{
			var reader = new JJxReader(chunk);
			var count = reader.ReadInt32();
		}
		this._IsContainerDecayLoaded = true;
	}
	private void _LoadContainerLock()
	{
		if (this._IsContainerLockLoaded) return;
		using (var chunk = this._Reader!.GetChunkStream(ArchiverChunkType.WorldLocks))
		{
			var reader = new JJxReader(chunk);
			var count = reader.ReadInt32();
			this._World.Locks.EnsureCapacity(count);
			for (var i = 0; i < count; ++i)
				this._World.Locks.Add(reader.ReadObject<Lock>());
		}
		this._IsContainerLockLoaded = true;
	}
	private void _LoadContainerEntity()
	{
		if (this._IsContainerEntityLoaded) return;
		using (var chunk = this._Reader!.GetChunkStream(ArchiverChunkType.WorldEntities))
		{
			var reader = new JJxReader(chunk);
			var count = reader.ReadInt32();
			this._World.Entities.EnsureCapacity(count);
			for (var i = 0; i < count; ++i)
				this._World.Entities.Add(reader.ReadObject<Entity>());
		}
		this._IsContainerEntityLoaded = true;
	}
	// Writing
	public void Write(IArchiveWriter writer)
	{
		if (!this.IsFullyLoaded) this._Load();
		Stream chunk;
		// Info
		chunk = writer.WriteChunk(ArchiverChunkType.WorldInfo);
		{
			var streamWriter = new JJxWriter(chunk);
			streamWriter.Write(this.Id);
			streamWriter.Write(this.LastPlayed);
			streamWriter.Write(this.Version);
			streamWriter.Write(this.Name, SIZEOF_NAME);
			streamWriter.Write(this.Author, SIZEOF_AUTHOR);
			streamWriter.Write(this.Size.Width);
			streamWriter.Write(this.Size.Height);
			streamWriter.Write(this.Player.X);
			streamWriter.Write(this.Player.Y);
			streamWriter.Write(this.Spawn.X);
			streamWriter.Write(this.Spawn.Y);
			streamWriter.Write(this.Planet);
			streamWriter.Write(this.Season);
			streamWriter.Write(this.Gamemode);
			streamWriter.Write(this.SizeBounds);
			streamWriter.Write(this.SkyBounds);
			streamWriter.Skip(4); // Unknown (likely padding)
			streamWriter.Skip(sizeof(uint) * 32); // Padding
		}
		// Skyline
		chunk = writer.WriteChunk(ArchiverChunkType.WorldSkyline);
		{
			var streamWriter = new JJxWriter(chunk);
			streamWriter.Write(MemoryMarshal.Cast<ushort, byte>(this.Skyline));
		}
		// Tiles
		chunk = writer.WriteChunk(ArchiverChunkType.WorldBlocks, version: 1, isCompressed: true);
		{
			var streamWriter = new JJxWriter(chunk);
			foreach (ref var tile in this.Tilemap.Tiles)
				streamWriter.Write(tile);
		}
		// Fog
		if (!this.HasFog)
			chunk = writer.WriteChunk(ArchiverChunkType.Padding);
		else
		{
			chunk = writer.WriteChunk(ArchiverChunkType.WorldFog, isCompressed: true);
			{
				var streamWriter = new JJxWriter(chunk);
				streamWriter.Write(this.Fog);
			}
		}
		// Time
		chunk = writer.WriteChunk(ArchiverChunkType.WorldTime);
		{
			var streamWriter = new JJxWriter(chunk);
			streamWriter.Write(this.Time);
		}
		// Weather
		chunk = writer.WriteChunk(ArchiverChunkType.WorldWeather);
		{
			var streamWriter = new JJxWriter(chunk);
			streamWriter.Write(this.Weather);
		}
		// Container[Chest]
		chunk = writer.WriteChunk(ArchiverChunkType.WorldChests);
		{
			var streamWriter = new JJxWriter(chunk);
			streamWriter.Write((int)0);
		}
		// Container[Forge]
		chunk = writer.WriteChunk(ArchiverChunkType.WorldForges);
		{
			var streamWriter = new JJxWriter(chunk);
			streamWriter.Write((int)0);
		}
		// Container[Sign]
		chunk = writer.WriteChunk(ArchiverChunkType.WorldSigns);
		{
			var streamWriter = new JJxWriter(chunk);
			streamWriter.Write(this.Signs.Count);
			foreach (var sign in CollectionsMarshal.AsSpan(this.Signs))
				streamWriter.Write(sign);
		}
		// Container[Stable]
		chunk = writer.WriteChunk(ArchiverChunkType.WorldStables);
		{
			var streamWriter = new JJxWriter(chunk);
			streamWriter.Write((int)0);
		}
		// Container[Lab]
		chunk = writer.WriteChunk(ArchiverChunkType.WorldLabs);
		{
			var streamWriter = new JJxWriter(chunk);
			streamWriter.Write((int)0);
		}
		// Container[Shelf]
		chunk = writer.WriteChunk(ArchiverChunkType.WorldShelves);
		{
			var streamWriter = new JJxWriter(chunk);
			streamWriter.Write((int)0);
		}
		// Container[Plant]
		chunk = writer.WriteChunk(ArchiverChunkType.WorldPlants);
		{
			var streamWriter = new JJxWriter(chunk);
			streamWriter.Write((int)0);
		}
		// Container[Fruit]
		chunk = writer.WriteChunk(ArchiverChunkType.WorldFruits);
		{
			var streamWriter = new JJxWriter(chunk);
			streamWriter.Write((int)0);
		}
		// Container[Decay]
		chunk = writer.WriteChunk(ArchiverChunkType.WorldPlantDecay);
		{
			var streamWriter = new JJxWriter(chunk);
			streamWriter.Write((int)0);
		}
		// Container[Lock]
		chunk = writer.WriteChunk(ArchiverChunkType.WorldLocks);
		{
			var streamWriter = new JJxWriter(chunk);
			streamWriter.Write(this.Locks.Count);
			foreach (var @lock in CollectionsMarshal.AsSpan(this.Locks))
				streamWriter.Write(@lock);
		}
		// Fluid
		chunk = writer.WriteChunk(ArchiverChunkType.WorldFluid);
		{
			var streamWriter = new JJxWriter(chunk);
			streamWriter.Write(this.Fluid);
		}
		// Circuits
		chunk = writer.WriteChunk(ArchiverChunkType.WorldCircuitry);
		{
			var streamWriter = new JJxWriter(chunk);
			streamWriter.Write(this.Circuitry);
		}
		// Container[Entity]
		chunk = writer.WriteChunk(ArchiverChunkType.WorldEntities);
		{
			var streamWriter = new JJxWriter(chunk);
			streamWriter.Write(this.Entities.Count);
			foreach (var entity in CollectionsMarshal.AsSpan(this.Entities))
				streamWriter.Write(entity);
		}
	}
	/* Static Methods */
	internal static WorldArchive Load(IArchiveReader reader, bool eagerLoad = false)
	{
		World world;
		using (var chunk = reader.GetChunkStream(ArchiverChunkType.WorldInfo))
		{
			var streamReader = new JJxReader(chunk);
			var guid = streamReader.ReadObject<Guid>();
			var lastPlayed = streamReader.ReadObject<DateTime>();
			var version = streamReader.ReadObject<Version>();
			var name = streamReader.ReadString(32);
			var author = streamReader.ReadString(16);
			var size = (
				streamReader.ReadUInt16(),
				streamReader.ReadUInt16()
			);
			var player = (
				streamReader.ReadUInt16(),
				streamReader.ReadUInt16()
			);
			var spawn = (
				streamReader.ReadUInt16(),
				streamReader.ReadUInt16()
			);
			var planet = streamReader.ReadObject<Planet>();
			var season = streamReader.ReadObject<Season>();
			var gamemode = streamReader.ReadObject<Gamemode>();
			var sizeBounds = streamReader.ReadObject<MapBounds>();
			var skyBounds = streamReader.ReadObject<MapBounds>();
			streamReader.Skip(4); // Unknown (likely padding)
			streamReader.Skip(sizeof(uint) * 32); // Padding
			world = new(
				guid, version, lastPlayed, name, author, planet,
				season, gamemode, size, sizeBounds, skyBounds
			);
			world.Size = size;
			world.Player = player;
			world.Spawn = spawn;
		}
		var archive = new WorldArchive(world, reader);
		if (eagerLoad) archive._Load();
		return archive;
	}
	/* Properties */
	internal bool IsFullyLoaded => this._IsSkylineLoaded &&
								   this._AreTilesLoaded &&
								   this._IsFogLoaded &&
								   this._IsTimeLoaded &&
								   this._IsWeatherLoaded &&
								   this._IsContainerChestLoaded &&
								   this._IsContainerForgeLoaded &&
								   this._IsContainerSignLoaded &&
								   this._IsContainerStableLoaded &&
								   this._IsContainerLabLoaded &&
								   this._IsContainerShelfLoaded &&
								   this._IsContainerPlantLoaded &&
								   this._IsContainerFruitLoaded &&
								   this._IsContainerDecayLoaded &&
								   this._IsContainerLockLoaded &&
								   this._IsFluidLoaded &&
								   this._AreCircuitsLoaded &&
								   this._IsContainerEntityLoaded;
	private readonly IArchiveReader? _Reader;
	internal readonly World _World;
	// Info
	public Guid Id => this._World.Id;
	public Version Version => this._World.Version;
	public DateTime LastPlayed { get => this._World.LastPlayed; set => this._World.LastPlayed = value; }
	public string Name { get => this._World.Name; set => this._World.Name = value; }
	public string Author { get => this._World.Author; set => this._World.Author = value; }
	public Planet Planet { get => this._World.Planet; set => this._World.Planet = value; }
	public Season Season { get => this._World.Season; set => this._World.Season = value; }
	public Gamemode Gamemode { get => this._World.Gamemode; set => this._World.Gamemode = value; }
	public MapBounds SizeBounds { get => this._World.SizeBounds; set => this._World.SizeBounds = value; }
	public MapBounds SkyBounds { get => this._World.SkyBounds; set => this._World.SkyBounds = value; }
	public (ushort Width, ushort Height) Size => this._World.Size;
	public (ushort X, ushort Y) Player { get => this._World.Player; set => this._World.Player = value; }
	public (ushort X, ushort Y) Spawn { get => this._World.Spawn; set => this._World.Spawn = value; }
	// Skyline
	private bool _IsSkylineLoaded = false;
	public Span<ushort> Skyline { get { this._LoadSkyline(); return this._World.Skyline; } }
	// Tiles
	private bool _AreTilesLoaded = false;
	public Tilemap Tilemap { get { this._LoadTilemap(); return this._World.Tilemap; } }
	// Fog
	private bool _IsFogLoaded = false;
	public bool HasFog { get { this._LoadFog(); return this._World.HasFog; } }
	public Span<byte> Fog { get { this._LoadFog(); return this._World.Fog; } }
	// Time
	private bool _IsTimeLoaded = false;
	public Span<byte> Time { get { this._LoadTime(); return this._World.Time; } }
	// Weather
	private bool _IsWeatherLoaded = false;
	public Span<byte> Weather { get { this._LoadWeather(); return this._World.Weather; } }
	// Containers
	private bool _IsContainerChestLoaded = false;
	private bool _IsContainerForgeLoaded = false;
	private bool _IsContainerSignLoaded = false;
	public List<Sign> Signs { get { this._LoadContainerSign(); return this._World.Signs; } }
	private bool _IsContainerStableLoaded = false;
	private bool _IsContainerLabLoaded = false;
	private bool _IsContainerShelfLoaded = false;
	private bool _IsContainerPlantLoaded = false;
	private bool _IsContainerFruitLoaded = false;
	private bool _IsContainerDecayLoaded = false;
	private bool _IsContainerLockLoaded = false;
	public List<Lock> Locks { get { this._LoadContainerLock(); return this._World.Locks; } }
	private bool _IsContainerEntityLoaded = false;
	public List<Entity> Entities { get { this._LoadContainerEntity(); return this._World.Entities; } }
	// Fluid
	private bool _IsFluidLoaded = false;
	public Span<byte> Fluid { get { this._LoadFluid(); return this._World.Fluid; } }
	// Circuit
	private bool _AreCircuitsLoaded = false;
	public Span<byte> Circuitry { get { this._LoadCircuits(); return this._World.Circuitry; } }
	/* Class Properties */
	internal const int SIZEOF_NAME   = 32;
	internal const int SIZEOF_AUTHOR = 16;
}
