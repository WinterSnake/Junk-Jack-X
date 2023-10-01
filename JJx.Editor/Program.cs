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

internal enum Screen : byte
{
	Main = 0,
	Player = 1,
	World = 2
}

internal static class Program
{
	/* Static Methods */
	private static async Task Main(string[] args)
	{
		// Debug for now
		Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);
		Raylib.InitWindow(0, 0, "Junk Jack X Editor");
		Raylib.SetTargetFPS(144);
		// Load textures
		InterfaceRenderer.InitTexture(_TexturePaths[0]);
		ItemRenderer.InitTexture(_TexturePaths[1]);
		Program._PlayerEditor = new PlayerEditor();
		Program._WorldEditor = new WorldEditor();
		// Main loop
		while (!Raylib.WindowShouldClose())
		{
			var delta = Raylib.GetFrameTime();
			// Update
			switch (_CurrentScreen)
			{
				case Screen.Player:
				{
					_PlayerEditor.Update(delta);
				} break;
				case Screen.World:
				{
					_WorldEditor.Update(delta);
				} break;
				default:
				{

				} break;
			}
			// Drawing
			Raylib.ClearBackground(Color.BLACK);
			Raylib.BeginDrawing();
				switch (_CurrentScreen)
				{
					case Screen.Player:
					{
						_PlayerEditor.Draw();
					} break;
					case Screen.World:
					{
						_WorldEditor.Draw();
					} break;
					default:
					{

					} break;
				}
			Raylib.EndDrawing();
		}
		InterfaceRenderer.UnloadTexture();
		ItemRenderer.UnloadTexture();
		Raylib.CloseWindow();
	}
	/* Class Properties */
	private static Screen _CurrentScreen = Screen.Player;
	private static PlayerEditor _PlayerEditor;
	private static WorldEditor _WorldEditor;
	private static string[] _TexturePaths = {
		"data/gfx/interface.png",
		"data/gfx/treasures.png",
	};
	private const byte _Speed = 30;
}
