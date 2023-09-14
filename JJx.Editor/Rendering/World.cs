/*
	Junk Jack X Editor: Rendering
	- World

	Written By: Ryan Smith
*/
using System;
using System.Numerics;
using Raylib_cs;
using JJx;

public static class WorldRenderer
{
	/* Static Methods */
	public static void Render(JJx.World world)
	{
		// Background
		// Borders
		for (var x = 0; x < world.Size.Width; ++x)
			for (var y = 0; y < world.Borders[x]; ++y)
				Raylib.DrawTexturePro(Program.BlockTexture, BorderSprite, GetLocation(x, y, world.Size.Height), new Vector2(1, 1), 0, Color.WHITE);
		// Spawn
		// Player
		// Blocks
	}
	private static Rectangle GetLocation(int x, int y, uint worldHeight) => new Rectangle(x * 32, (worldHeight * 32) - (y * 32), 32, 32);
	/* Class Properties */
	private static readonly Rectangle BorderSprite = new Rectangle(31 * 32, 7 * 32, 32, 32);
}
