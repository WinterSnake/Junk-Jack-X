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
		using var fs = File.Open("debug/player/Debug_0.4.dat", FileMode.Open);
		var player = await Models.Player.FromStream(fs);
		Console.WriteLine($"Id: {player.Id}");
		Console.WriteLine($"Name: {player.Name}");
		Console.WriteLine($"Features: {player._Features.ToString("x4")} | HairStyle: {player.HairStyle}");
		player.HairStyle = 1;
		Console.WriteLine($"Features: {player._Features.ToString("x4")} | HairStyle: {player.HairStyle}");
		player.HairStyle = 4;
		player.Gender = false;
		Console.WriteLine($"Features: {player._Features.ToString("x4")} | HairStyle: {player.HairStyle}");
	}
}
