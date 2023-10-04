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
	/* Static Constructorss */
	static Program()
	{
		// Window
		Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);
		Raylib.InitWindow(0, 0, "Junk Jack X Editor");
		Raylib.SetTargetFPS(144);
		// Renderers
		InterfaceRenderer.InitRenderer(_TexturePaths[0]);
		BlockRenderer.InitRenderer(_TexturePaths[1]);
		ItemRenderer.InitRenderer(_TexturePaths[2]);
		// Editors
		Program._PlayerEditor = new PlayerEditor();
		Program._WorldEditor = new WorldEditor();
	}
	/* Static Methods */
	private static void Main(string[] args)
	{
		// Debug
		var task = Program.Load(args[0]);
		task.Wait();
		// Loop
		while (!Raylib.WindowShouldClose())
		{
			Program.Update();
			Raylib.ClearBackground(Color.BLACK);
			Raylib.BeginDrawing();
				Program.Draw();
				Raylib.DrawFPS(0, 0);
			Raylib.EndDrawing();
		}
		// Unloading
		InterfaceRenderer.UnloadRenderer();
		BlockRenderer.UnloadRenderer();
		ItemRenderer.UnloadRenderer();
		Raylib.CloseWindow();
	}
	private static void Draw()
	{
		switch (Program._CurrentScreen)
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
	}
	private static void Update()
	{
		var delta = Raylib.GetFrameTime();
		switch (Program._CurrentScreen)
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
	}
	public static async Task Load(string path)
	{
		var stream = await JJx.ArchiverStream.Reader(path);
		switch (stream.Type)
		{
			case JJx.ArchiverType.Map:
			{
				Program._WorldEditor.World = await JJx.World.Load(stream);
				Program._CurrentScreen = Screen.World;
			} break;
			case JJx.ArchiverType.Player:
			{
				Program._PlayerEditor.ActivePlayer = await JJx.Player.Load(stream);
				Program._CurrentScreen = Screen.Player;
			} break;
			default:
			{
				_CurrentScreen = Screen.Main;
			} break;
		}
	}
	/* Class Properties */
	private static Screen _CurrentScreen = Screen.Main;
	private static readonly PlayerEditor _PlayerEditor;
	private static readonly WorldEditor _WorldEditor;
	private static readonly string[] _TexturePaths = {
		"data/gfx/interface.png",
		"data/gfx/rocks.png",
		"data/gfx/treasures.png",
	};
	private const byte _Speed = 30;
}

internal enum Screen : byte
{
	Main = 0,
	Player = 1,
	World = 2
}
