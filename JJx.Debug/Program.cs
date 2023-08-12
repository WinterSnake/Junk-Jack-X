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
		JJx.Player player;
		using (var fs = File.Open(args[0], FileMode.Open, FileAccess.Read))
		{
			player = await Player.FromStream(fs);
		}
		player.Character.Hair.Style = 1;
		player.Character.Hair.Color = Character.HairColor.Ginger;
		player.Gameplay.Flags = Gameplay.Flag.Hardcore;
		player.Name = "NewPlayer";
		player.Character.Tone = 0;
		using (var fs = File.Open(args[0], FileMode.Open, FileAccess.Write))
		{
			await player.ToStream(fs);
		}
	}
}
