/*
	Junk Jack X: Core
	- [Extensions]World

	Written By: Ryan Smith
*/

using System.IO;

namespace JJx.Core.Extensions;

public static class WorldExtensions
{
	/* Static Methods */
	public static (ushort Width, ushort Height) GetSize(this MapBounds bounds)
	{
		return bounds switch
		{
			MapBounds.Tiny => (512, 128),
			MapBounds.Small => (768, 256),
			MapBounds.Normal => (1024, 256),
			MapBounds.Large => (2048, 384),
			MapBounds.Huge => (4096, 512),
			_ => throw new InvalidDataException($"Unknown map size for bounds type '{bounds}'"),
		};
	}
	public static void Save(this World world, string file)
	{
		var archive = new WorldArchive(world);
		Archive.Save(file, archive);
	}
}
