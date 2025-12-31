/*
	Junk Jack X: Core
	- Archiver

	Written By: Ryan Smith
*/

using System;
using System.IO;

namespace JJx.Core;

public interface IArchive : IDisposable
{
	/* Instance Methods */
	public void Write(IArchiveWriter writer);
}

public static class Archive
{
	/* Static Methods */
	// Reading
	private static IArchiveReader _Load(string file)
	{
		var fileStream = File.Open(file, FileMode.Open);
		try {
			return ArchiveStream.Reader(fileStream);
		} catch {
			fileStream.Dispose();
			throw;
		}
	}
	public static IArchive Load(string file, bool eagerLoad = false)
	{
		var reader = _Load(file);
		try {
			IArchive archive = reader.Type switch
			{
				ArchiveType.Player => PlayerArchive.Load(reader, eagerLoad),
				ArchiveType.World => WorldArchive.Load(reader, eagerLoad),
				_ => throw new InvalidOperationException($"Unhandled archive type '{reader.Type}'"),
			};
			if (eagerLoad)
				reader.Dispose();
			return archive;
		} catch {
			reader.Dispose();
			throw;
		}
	}
	public static Player LoadPlayer(string file)
	{
		using var reader = _Load(file);
		if (reader.Type is not ArchiveType.Player)
			throw new InvalidDataException($"Tried loading non-player file ({reader.Type}) as Player");
		var archive = PlayerArchive.Load(reader, true);
		return archive._Player;
	}
	public static World LoadWorld(string file)
	{
		using var reader = _Load(file);
		if (reader.Type is not ArchiveType.World)
			throw new InvalidDataException($"Tried loading non-world file ({reader.Type}) as World");
		var archive = WorldArchive.Load(reader, true);
		return archive._World;
	}
	// Writing
	public static void Save(string file, IArchive archive)
	{
		using var fileStream = File.Open(file, FileMode.Create);
		var type = archive switch
		{
			PlayerArchive => ArchiveType.Player,
			WorldArchive => ArchiveType.World,
			_ => throw new InvalidOperationException($"Unhandled archive type '{archive}'"),
		};
		var writer = ArchiveStream.Writer(fileStream, type);
		archive.Write(writer);
		writer.Flush();
	}
}
