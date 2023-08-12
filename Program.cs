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
		using var fs = File.Open("debug/player/Debug_1.0.dat", FileMode.Open);
		var player = await Models.Player.FromStream(fs);
		Console.WriteLine(player.Name);
	}
}
