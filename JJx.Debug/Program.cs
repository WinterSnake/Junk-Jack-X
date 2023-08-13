/*
	Junk Jack X: Debug

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;
using JJx;

internal class Program
{
	/* Static Methods */
	private static async Task Main(string[] args)
	{
		if (args.Length < 2 || args.Length > 3) return;
		// Player Editing
		if (args[0] == "-p" || args[0] == "--player")
		{
			JJx.Player player;
			// Read player from file
			Console.WriteLine("Loading player from file...");
			using (var fs = File.Open(args[1], FileMode.Open, FileAccess.Read))
			{
				player = await Player.FromStream(fs);
			}
			// Do changes
			// Write player to file (if flagged)
			if (args.Length == 2 || (args.Length == 3 && args[2] != "--store")) return;
			Console.WriteLine("Writing player to file...");
			using (var fs = File.Open(args[1], FileMode.Open, FileAccess.Write))
			{
				await player.ToStream(fs);
			}
		}
		// World Editing
		else if (args[0] == "-w" || args[0] == "--world")
		{
			JJx.World world;
			// Read world from file
			Console.WriteLine("Loading world from file..");
			using (var fs = File.Open(args[1], FileMode.Open, FileAccess.Read))
			{
				world = await World.FromStream(fs);
			}
			// Do changes
			// Write world to file (if flagged)
			if (args.Length == 2 || (args.Length == 3 && args[2] != "--store")) return;
			Console.WriteLine("Writing world to file..");
			using (var fs = File.Open(args[1], FileMode.Open, FileAccess.Write))
			{
				await world.ToStream(fs);
			}
		}
	}
}
