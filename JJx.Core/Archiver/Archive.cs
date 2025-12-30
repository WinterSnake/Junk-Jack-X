/*
	Junk Jack X: Core
	- Archiver

	Written By: Ryan Smith
*/

using System;
using System.IO;

namespace JJx.Core;

public interface IArchive : IDisposable { }

public static class Archive
{
	/* Static Methods */
	public static IArchive Load(string file)
	{
		var fileStream = File.Open(file, FileMode.Open);
		try {
			var reader = ArchiveStream.Reader(fileStream);
			var archive = reader.Type switch
			{
				ArchiveType.Player => PlayerArchive.Load(reader),
				_ => throw new InvalidOperationException($"Unhandled archive type '{reader.Type}'"),
			};
			return archive;
		} catch {
			fileStream.Dispose();
			throw;
		}
	}
}
