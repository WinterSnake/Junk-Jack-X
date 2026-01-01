/*
	Junk Jack X: Core
	- [Archive]World

	Written By: Ryan Smith
*/

using System;
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
	}
	private void _LoadSkyline()
	{
		if (this._IsSkylineLoaded) return;
		using (var chunk = this._Reader!.GetChunkStream(ArchiverChunkType.WorldSkyline))
		{
			var reader = new JJxReader(chunk);
			var skyline = new ushort[this._World.Size.Width];
			reader.ReadSpan(MemoryMarshal.Cast<ushort, byte>(skyline.AsSpan()));
			this._World.Skyline = skyline;
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
	// Writing
	public void Write(IArchiveWriter writer)
	{
		if (!this.IsFullyLoaded)
		{
			this._Load();
			this.Dispose();
		}
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
			streamWriter.Skip(4); // Unknown
			streamWriter.Skip(sizeof(uint) * 32); // Padding
		}
		// Skyline
		chunk = writer.WriteChunk(ArchiverChunkType.WorldSkyline);
		{
			var streamWriter = new JJxWriter(chunk);
			streamWriter.Write(MemoryMarshal.Cast<ushort, byte>(this.Skyline.AsSpan()));
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
			streamReader.Skip(4); // Unknown
			streamReader.Skip(sizeof(uint) * 32); // Padding
			world = new(
				guid, version, lastPlayed, name, author,
				planet, season, gamemode, sizeBounds, skyBounds
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
	internal bool IsFullyLoaded => this._IsSkylineLoaded && this._AreTilesLoaded;
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
	public ushort[] Skyline { get { this._LoadSkyline(); return this._World.Skyline; } }
	// Tiles
	private bool _AreTilesLoaded = false;
	public Tilemap Tilemap { get { this._LoadTilemap(); return this._World.Tilemap; } }
	// Fog
	private bool _IsFogLoaded = false;
	public bool HasFog { get { this._LoadFog(); return this._World.HasFog; } }
	public Span<byte> Fog { get { this._LoadFog(); return this._World.Fog; } }
	/* Class Properties */
	private const int SIZEOF_NAME   = 32;
	private const int SIZEOF_AUTHOR = 16;
}
