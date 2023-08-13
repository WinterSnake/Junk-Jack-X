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
		if (args.Length != 1) return;
		JJx.World world;
		using (var fs = File.Open(args[0], FileMode.Open, FileAccess.Read))
		{
			world = await World.FromStream(fs);
		}
		Console.WriteLine(world.Gamemode);
		world.Gamemode = Gamemode.Survival;
		using (var fs = File.Open(args[0], FileMode.Open, FileAccess.Write))
		{
			world.ToStream(fs);
		}
	}
}
