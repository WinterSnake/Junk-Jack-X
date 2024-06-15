/*
	Junk Jack X Tools: Editor

	Written By: Ryan Smith
*/
using System;
using System.Threading;
using System.Threading.Tasks;
using Raylib_cs;
using JJx;

internal static class Program
{
	/* Static Methods */
	private static void Main(string[] args)
	{
		Raylib.InitWindow(1920, 1080, "Junk Jack X Editor");

		ItemRenderer.InitRenderer("./data/data/");
		LoadEditor("./data/players/Debug.dat").Wait();

		Raylib.SetTargetFPS(60);
		while (!Raylib.WindowShouldClose())
		{
			var deltaTime = Raylib.GetFrameTime();
			_ActiveEditor.Update(deltaTime);
			Raylib.BeginDrawing();
			{
				Raylib.ClearBackground(Color.RayWhite);
				_ActiveEditor.Draw();
			}
			Raylib.EndDrawing();
		}

		Raylib.CloseWindow();
	}
	private static async Task LoadEditor(string filePath)
	{
		var stream = await ArchiverStream.Reader(filePath);
		switch(stream.Type)
		{
			case ArchiverStreamType.Player:
			{
				var player = await Player.FromStream(stream);
				_ActiveEditor = new PlayerEditor(filePath, player);
			} break;
			case ArchiverStreamType.World:
			case ArchiverStreamType.Adventure:
			{
				throw new NotImplementedException("World files not supported yet..");
			}
		}
	}
	/* Class Properties */
	private static EditorBase _ActiveEditor;
}
