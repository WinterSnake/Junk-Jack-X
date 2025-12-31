/*
	Junk Jack X: Core
	- [Archive]World

	Written By: Ryan Smith
*/

using System;
using JJx.Core.Serialization;

namespace JJx.Core;

public sealed class WorldArchive : IArchive
{
	/* Constructor */
	public WorldArchive(World world)
		:this(world, null) { }
	private WorldArchive(World world, IArchiveReader? reader)
	{
		this._World = world;
		this._Reader = reader;
	}
	/* Instance Methods */
	public void Dispose() => this._Reader?.Dispose();
	/* Static Methods */
	internal static WorldArchive Load(IArchiveReader reader)
	{
		World world;
		using (var infoChunk = reader.GetChunkStream(ArchiverChunkType.WorldInfo))
		{
			var streamReader = new JJxReader(infoChunk);
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
		return new(world, reader);
	}
	/* Properties */
	private readonly IArchiveReader? _Reader;
	private readonly World _World;
	// World
}
