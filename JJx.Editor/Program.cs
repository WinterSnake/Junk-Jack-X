/*
	Junk Jack X Editor

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
		JJx.Player player;
		using (var fs = File.Open(args[0], FileMode.Open, FileAccess.Read))
		{
			player = await Player.FromStream(fs);
		}
		using (var fs = File.Open(args[0], FileMode.Open, FileAccess.Write))
		{
			await player.ToStream(fs);
		}
	}
}
