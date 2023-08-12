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
		Models.Player player;
		using (var fs = File.Open("debug/player/Debug_0.0.dat", FileMode.Open, FileAccess.Read))
		{
			player = await Models.Player.FromStream(fs);
		}
		player.Name = "Debug_1";
		player.SkinTone = 2;
		player.Gender = true;
		player.HairStyle = 6;
		player.HairColor = Models.Color.Ginger;
		using (var fs = File.Open("debug/player/Debug_0.0.dat", FileMode.Open, FileAccess.Write))
		{
			await player.ToStream(fs);
		}
	}
}
