/*
	Junk Jack X: Editor

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using Raylib_cs;
using JJx;

internal static class Program
{
	/* Static Methods */
	private static async Task Main(string[] args)
	{
		// Debug for now
		var player = await JJx.Player.Load(args[0]);
		//var world = await JJx.World.Load(args[0]);
		Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);
		Raylib.InitWindow(0, 0, "Junk Jack X Editor");
		Raylib.SetTargetFPS(144);
		// Load textures
		InterfaceRenderer.InitTexture(_TexturePaths[0]);
		ItemRenderer.InitTexture(_TexturePaths[1]);
		// Main loop
		var editor = new PlayerEditor();
		while (!Raylib.WindowShouldClose())
		{
			Raylib.ClearBackground(Color.BLACK);
			editor.Update(0.0f);
			Raylib.BeginDrawing();
				editor.Draw();
				Raylib.DrawText($"Mouse Position: {Raylib.GetMousePosition()}", 0, 0, 12, Color.WHITE);
			Raylib.EndDrawing();
		}
		InterfaceRenderer.UnloadTexture();
		ItemRenderer.UnloadTexture();
		Raylib.CloseWindow();
	}
	/* Class Properties */
	private static string[] _TexturePaths = {
		"data/gfx/interface.png",
		"data/gfx/treasures.png",
	};
	private const byte _Speed = 30;
}
