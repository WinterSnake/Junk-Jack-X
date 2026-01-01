/*
	Junk Jack X: Core
	- [Extensions]Player

	Written By: Ryan Smith
*/

namespace JJx.Core.Extensions;

public static class PlayerExtensions
{
	/* Static Methods */
	public static void Save(this Player player, string file)
	{
		var archive = new PlayerArchive(player);
		Archive.Save(file, archive);
	}
}
