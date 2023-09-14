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
		var world = await JJx.World.Load(args[0]);
		Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);
		Raylib.InitWindow(1920, 1080, "Junk Jack X Editor");
		Raylib.SetTargetFPS(144);
		// Load textures
		for (var i = 0; i < Gfx.Length; ++i)
			Gfx[i].Texture = Raylib.LoadTexture(Gfx[i].Path);
		// Main loop
		var camera = new Camera2D(
			new Vector2(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2),  // Offset
			WorldRenderer.GetPlayerVector(world),  // Target
			0.0f, 1.0f  // Rotation, Zoom
		);
		while (!Raylib.WindowShouldClose())
		{
			float deltaTime = Raylib.GetFrameTime();
			if (Raylib.IsKeyDown(KeyboardKey.KEY_W))
				camera.target.Y -= _Speed;
			else if (Raylib.IsKeyDown(KeyboardKey.KEY_S))
				camera.target.Y += _Speed;
			if (Raylib.IsKeyDown(KeyboardKey.KEY_A))
				camera.target.X -= _Speed;
			else if (Raylib.IsKeyDown(KeyboardKey.KEY_D))
				camera.target.X += _Speed;
			Raylib.ClearBackground(Color.BLACK);
			Raylib.BeginDrawing();
				Raylib.BeginMode2D(camera);
					// World
					WorldRenderer.Render(world);
					// Gui
				Raylib.EndMode2D();
				Raylib.DrawText($"FPS: {Raylib.GetFPS()}", 20, 20, 10, Color.WHITE);
			Raylib.EndDrawing();
		}
		Raylib.CloseWindow();
	}
	/* Class Properties */
	public static Texture2D BlockTexture { get { return Gfx[0].Texture.Value; }}
	private static (string Path, Texture2D? Texture)[] Gfx = {
		("data/gfx/rocks.png", null),
	};
	private const byte _Speed = 30;
}
