/*
	Junk Jack X Editor

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

internal class Program
{
	/* Static Methods */
	private static async Task Main(string[] args)
	{
		Player player;
		using (var fs = File.Open("debug/player/Player.dat", FileMode.Open, FileAccess.Read))
		{
			player = await Player.FromStream(fs);
		}
		player.Gameplay.Flags = (Gameplay.Flag.Hardcore | Gameplay.Flag.ContinuousTime);
		using (var fs = File.Open("debug/player/Debug.0.dat", FileMode.Open, FileAccess.Write))
		{
			await player.ToStream(fs);
		}
	}
}
