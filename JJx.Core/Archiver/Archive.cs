/*
   Junk Jack X: Core
   - Archiver

   Written By: Ryan Smith
*/

using System;

namespace JJx.Core;

public interface IArchive { }

public static class Archive
{
	/* Static Methods */
	public static IArchive Load(string path)
	{
		var manager = ArchiveManager.Reader(path);
		return manager.Type switch
		{
			ArchiveType.Player => JJxPlayer.Deserialize(manager),
			ArchiveType.World => JJxWorld.Deserialize(manager),
			ArchiveType.Adventure => JJxAdventure.Deserialize(manager),
			_ => throw new InvalidOperationException($"Unknown archive type: {manager.Type}."),
		};
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
