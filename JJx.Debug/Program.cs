/*
	Junk Jack X: Debug

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;
using JJx;

/*

	Usage:
	<program> --player "name" [--store]
	<program> --player --load <path> [--store]
	<program> --world <path> [--store] [new file]

*/
internal static class Program
{
	/* Static Methods */
	private static async Task Main(string[] args)
	{
		if (args.Length < 2) return;
		// Player Editing
		if (args[0] == "-p" || args[0] == "--player")
		{
			// Create or load player
			bool loaded = false;
			JJx.Player player;
			if (args.Length > 2 && (args[1] == "-l" || args[1] == "--load"))
			{
				loaded = true;
				player = await JJx.Player.Load(args[2]);
			}
			else
				player = new JJx.Player(args[1]);
			// Edit player
			// Save player
			if (
				(args.Length > 2 && (args[2] == "-s" || args[2] == "--store")) ||
				(args.Length > 3 && (args[3] == "-s" || args[3] == "--store"))
			)
			{
				string path = loaded ? args[2] : player.Name + ".dat";
				await player.Save(path);
			}
		}
		// Adventure Editing
		else if (args[0] == "-a" || args[0] == "--adventure")
		{
			// Create or load adventure
			var adventure = await JJx.Adventure.Load(args[1]);
			// Edit adventure
			// Save adventure
			if (args.Length > 2 && (args[2] == "-s" || args[2] == "--store"))
			{
				string path = args[1];
				await adventure.Save(path);
			}
		}
		// World Editing
		else if (args[0] == "-w" || args[0] == "--world")
		{
			// Create or load world
			var world = await JJx.World.Load(args[1]);
			// Edit world
			// Save world
			if (
				(args.Length > 1 && (args[1] == "-s" || args[1] == "--store")) ||
				(args.Length > 2 && (args[2] == "-s" || args[2] == "--store"))
			)
			{
				string path = args.Length > 3 ? args[3] + ".dat" : args[1];
				await world.Save(path);
			}
		}
	}
}
