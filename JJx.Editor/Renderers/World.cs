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
	/*
	public static void Render(JJx.World world)
	{
		// Background
		// Borders + Tiles
		//for (var x = 0; x < 1; ++x)
		for (var x = 0; x < world.Size.Width; ++x)
		{
			//for (var y = 0; y < 3; ++y)
			for (var y = 0; y < world.Size.Height; ++y)
			{
				var tile = world.Blocks[x, y];
				// Border
				if (y < world.Borders[x])
					Raylib.DrawTexturePro(
						Program.BlockTexture, BorderSprite, GetWorldLocation(x, y, world.Size.Height),
						new Vector2(1, 1), 0, Color.WHITE
					);
				/// Tile
				// Background
				/*
				if (tile.BackgroundId != 0x0000)
				{
					// Base
					Raylib.DrawTexturePro(
						Program.BlockTexture, GetSpriteLocation(tile.BackgroundId), GetWorldLocation(x, y, world.Size.Height),
						new Vector2(1, 1), 0, Color.DARKGRAY
					);
				}
				// Foreground
				if (tile.ForegroundId != 0x0000)
				{
					// Base
					Raylib.DrawTexturePro(
						Program.BlockTexture, GetSpriteLocation(tile.ForegroundId), GetWorldLocation(x, y, world.Size.Height),
						new Vector2(1, 1), 0, Color.WHITE
					);
					// Decoration
					/*
					for (var i = 0; i < tile.DecorationIds.Length; ++i)
					{
						var (id, background) = tile.GetDecoration((byte)i);
						if (!background && id != 0x0000)
						{
							Raylib.DrawTexturePro(
								Program.BlockTexture, GetSpriteLocation(id), GetWorldLocation(x, y, world.Size.Height),
								new Vector2(1, 1), 0, Color.WHITE
							);
						}
					}
				}
			}
		}
		// Spawn
		// Player
	}
	*/
	public static Vector2 GetPlayerVector(JJx.World world) => new Vector2((world.Player.X) * 32, (world.Size.Height - world.Player.Y) * 32);
	private static Rectangle GetWorldLocation(int x, int y, uint worldHeight) => new Rectangle(x * 32, (worldHeight - y) * 32, 32, 32);
	private static Rectangle GetSpriteLocation(ushort id)
	{
		var special = (id & 0x7000) >> 12;
		var shift   = (id & 0x0800) >>  6;
		if (special > 0 || shift > 0)
			id &= 0x07FF;
		int x = ((id % 32) | shift) + special;
		int y = id / 32;
		return new Rectangle(x * 32, y * 32, 32, 32);
	}
	/* Class Properties */
	private static readonly Rectangle BorderSprite = new Rectangle(31 * 32, 7 * 32, 32, 32);
}
