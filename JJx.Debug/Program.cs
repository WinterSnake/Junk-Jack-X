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
			PrintPlayer(player);
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
		// World Editing
		if (args[0] == "-w" || args[0] == "--world")
		{
			// Create or load world
			bool loaded = false;
			JJx.World world;
			if (args.Length > 2 && (args[1] == "-l" || args[1] == "--load"))
			{
				loaded = true;
				world = await JJx.World.Load(args[2]);
			}
			else
				world = new JJx.World(args[1], JJx.InitSize.Normal);
			// Edit world
			PrintWorld(world);
			// Save world
			if (
				(args.Length > 2 && (args[2] == "-s" || args[2] == "--store")) ||
				(args.Length > 3 && (args[3] == "-s" || args[3] == "--store"))
			)
			{
				string path = loaded ? args[2] : world.Name + ".dat";
				await world.Save(path);
			}
		}
	}
	private static void PrintPlayer(JJx.Player player)
	{
		Console.WriteLine($"Id: {player.Id}");
		Console.WriteLine($"Name: {player.Name}");
		Console.WriteLine($"Version: {player.Version}");
		Console.WriteLine($"Unlocked Planets: {player.UnlockedPlanets}");
		var gender = player.Character.Gender ? "Female" : "Male";
		Console.WriteLine($"Character.Gender: {gender}");
		Console.WriteLine($"Character.SkinTone: {player.Character.SkinTone}");
		Console.WriteLine($"Character.HairStyle: {player.Character.HairStyle}");
		Console.WriteLine($"Character.HairColor: {player.Character.HairColor}");
		Console.WriteLine($"Gameplay.Difficulty: {player.Gameplay.Difficulty}");
		Console.WriteLine($"Gameplay.Flags: {player.Gameplay.Flags}");
	}
	private static void PrintWorld(JJx.World world)
	{
		Console.WriteLine($"Id: {world.Id}");
		Console.WriteLine($"Last Played: {world.LastPlayed}");
		Console.WriteLine($"Version: {world.Version}");
		Console.WriteLine($"Name: {world.Name}");
		Console.WriteLine($"Author: {world.Author}");
		Console.WriteLine($"Size: {world.Size}");
		Console.WriteLine($"Player: {world.Player}");
		Console.WriteLine($"Spawn: {world.Spawn}");
		Console.WriteLine($"Planet: {world.Planet}");
		Console.WriteLine($"Season: {world.Season}");
		Console.WriteLine($"Gamemode: {world.Gamemode}");
		Console.WriteLine($"World Init Size: {world.WorldInitSize}");
		Console.WriteLine($"Sky Init Size: {world.SkyInitSize}");
		Console.WriteLine($"Chest Count: {world.Chests.Count}");
		Console.WriteLine($"Forge Count: {world.Forges.Count}");
		Console.WriteLine($"Sign Count: {world.Signs.Count}");
		Console.WriteLine($"Stable Count: {world.Stables.Count}");
		Console.WriteLine($"Lab Count: {world.Labs.Count}");
		Console.WriteLine($"Shelves Count: {world.Shelves.Count}");
		Console.WriteLine($"Entities Count: {world.Entities.Count}");
	}
}
