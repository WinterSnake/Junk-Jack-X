/*
	Junk Jack X: Editor
	- World

	Written By: Ryan Smith
*/
using System;
using System.Numerics;
using Raylib_cs;
using JJx;

public sealed class WorldEditor
{
	/* Constructors */
	public WorldEditor()
	{
	}
	/* Instance Methods */
	public void Draw()
	{
		/// Camera
		/// World
		Raylib.BeginMode2D(this._Camera);
			// Borders
			Raylib.DrawTexture(BlockRenderer.Texture, 0, 0, Color.WHITE);
			// Background
			// Background: Decorations
			// Foreground
			// Foreground: Decorations
			// Spawn
			// Player
			// Entities
		Raylib.EndMode2D();
		/// GUI
		/// Cursor
	}
	public void Update(float delta)
	{
		/// Camera
		// Offset
		var screenVector = (new Vector2(Raylib.GetScreenWidth(), Raylib.GetScreenHeight())) / 2;
		if (_Camera.offset != screenVector)
			_Camera.offset = screenVector;
		// Movement
		if (Raylib.IsKeyDown(KeyboardKey.KEY_W))
			_Camera.target.Y -= Speed;
		else if (Raylib.IsKeyDown(KeyboardKey.KEY_S))
			_Camera.target.Y += Speed;
		if (Raylib.IsKeyDown(KeyboardKey.KEY_A))
			_Camera.target.X -= Speed;
		else if (Raylib.IsKeyDown(KeyboardKey.KEY_D))
			_Camera.target.X += Speed;
		// Position
		// Zoom
		var wheelVector = Raylib.GetMouseWheelMoveV();
		if (wheelVector.Y < 0) // --Out
			_Camera.zoom -= Scroll;
		else if (wheelVector.Y > 0)  // --In
			_Camera.zoom += Scroll;
		else if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_MIDDLE))  // --Reset
			_Camera.zoom = 1.0f;
		_Camera.zoom = Math.Clamp(_Camera.zoom, 0.5f, 2.0f);
		/// GUI
	}
	/* Properties */
	public JJx.World ActiveWorld {
		get { return this._ActiveWorld; }
		set {
			this._ActiveWorld = value;
			//this._Camera.target = new Vector2(value.Player.X * 32, (value.Size.Height - value.Player.Y) * 32);
		}
	}
	private JJx.World _ActiveWorld = null;
	private Camera2D _Camera = new Camera2D(Vector2.Zero, Vector2.Zero, 0.0f, 1.0f);
	public static float Speed  = 10.0f;
	public static float Scroll =  0.25f;
}
