/*
	Junk Jack X Tools: Editor

	Written By: Ryan Smith
*/
using System;
using System.Threading;
using System.Threading.Tasks;
using Raylib_cs;

internal static class Program
{
	/* Static Methods */
	private static void Main(string[] args)
	{
		Raylib.InitWindow(1920, 1080, "Junk Jack X Editor");

		Raylib.SetTargetFPS(144);
		while (!Raylib.WindowShouldClose())
		{
			var deltaTime = Raylib.GetFrameTime();
			Raylib.BeginDrawing();
				Raylib.ClearBackground(Color.RayWhite);
			Raylib.EndDrawing();
		}

		Raylib.CloseWindow();
	}
	/* Class Properties */
}
