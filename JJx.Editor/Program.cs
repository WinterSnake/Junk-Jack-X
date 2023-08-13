/*
	Junk Jack X: Editor

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;
using Raylib_cs;
using JJx;

internal class Program
{
	/* Static Methods */
	private static async Task Main(string[] args)
	{
		Raylib.InitWindow(1920, 1080, "Junk Jack X Editor");
		// Debug for now
		Raylib.SetWindowMonitor(3);
		Raylib.ToggleFullscreen();
		Raylib.SetTargetFPS(60);
		// Load textures
		var treasureTexture = Raylib.LoadTexture(TreasuresGfx);
		// Main loop
		while (!Raylib.WindowShouldClose())
		{
			Raylib.BeginDrawing();
				Raylib.ClearBackground(Color.WHITE);
				Raylib.DrawTexture(treasureTexture, 0, 0, Color.WHITE);
			Raylib.EndDrawing();
		}

		Raylib.CloseWindow();
	}
	/* Class Properties */
	public static readonly string InterfaceGfx = "data/gfx/interface.png";
	public static readonly string EquipGfx = "data/gfx/equip.png";
	public static readonly string TreasuresGfx = "data/gfx/treasures.png";
}
