/*
   Junk Jack X: Core
   - Archiver

   Written By: Ryan Smith
*/

using System;

namespace JJx.Core;

public interface IArchive { }

public static class Archiver
{
	/* Static Methods */
	public static IArchive Load(string path)
	{
		using var manager = ArchiveManager.Reader(path);
		return manager.Type switch
		{
			ArchiveType.Player => JJxPlayer.Deserialize(manager),
			ArchiveType.World => JJxWorld.Deserialize(manager),
			ArchiveType.Adventure => JJxAdventure.Deserialize(manager),
			_ => throw new InvalidOperationException($"Unknown archive type: {manager.Type}."),
		};
	}
	public static JJxPlayer LoadPlayer(string path)
	{
		using var manager = ArchiveManager.Reader(path);
		if (manager.Type is not ArchiveType.Player)
			throw new InvalidOperationException("Tried loading non-player file as player.");
		return JJxPlayer.Deserialize(manager);
	}
	public static JJxWorld LoadWorld(string path)
	{
		using var manager = ArchiveManager.Reader(path);
		if (manager.Type is not ArchiveType.World)
			throw new InvalidOperationException("Tried loading non-world file as world.");
		return JJxWorld.Deserialize(manager);
	}
	public static JJxAdventure LoadAdventure(string path)
	{
		using var manager = ArchiveManager.Reader(path);
		if (manager.Type is not ArchiveType.Adventure)
			throw new InvalidOperationException("Tried loading non-adventure file as adventure.");
		return JJxAdventure.Deserialize(manager);
	}
	public static void Save(IArchive archive, string path)
	{
		switch (archive)
		{
			case JJxPlayer player: JJxPlayer.Serialize(player, null); break;
			case JJxWorld world: JJxWorld.Serialize(world, null); break;
			case JJxAdventure adventure: JJxAdventure.Serialize(adventure, null); break;
			default: throw new InvalidOperationException($"Unknown archive type: {archive.GetType().Name}.");
		}
	}
}
