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
		var areaMin = Raylib.GetScreenToWorld2D(Vector2.Zero, this._Camera);
		Raylib.BeginMode2D(this._Camera);
			for (int x = 0; x < this._World.Size.Width; ++x)
			{
				for (int y = 0; y < this._World.Size.Height; ++y)
				{
					var deltaY = this._World.Size.Height - y;
					var screenVector = Raylib.GetWorldToScreen2D(
						new Vector2(x * 32, deltaY * 32),
						this._Camera
					);
					if (
						screenVector.X < _OffScreenBuffer ||
						screenVector.Y < _OffScreenBuffer ||
						screenVector.X > Raylib.GetScreenWidth() ||
						screenVector.Y > Raylib.GetScreenHeight()
					)
						continue;
					// Border
					var tile = this._World.Blocks[x, y];
					var destination = new Rectangle(x * 32, deltaY * 32, 32, 32);
					if (y < this._World.Borders[x] && tile.BackgroundId == 0x0000 && tile.ForegroundId == 0x0000)
						Raylib.DrawTexturePro(
							BlockRenderer.Texture,
							BlockRenderer.Border,
							destination,
							Vector2.Zero,
							0.0f,
							Color.WHITE
						);
					// Background
					if (tile.BackgroundId != 0x0000)
					{
						Raylib.DrawTexturePro(
							BlockRenderer.Texture,
							BlockRenderer.GetIdSprite(tile.BackgroundId),
							destination,
							Vector2.Zero,
							0.0f,
							Color.DARKGRAY
						);
						// Background: Decorations
					}
					// Foreground
					if (tile.ForegroundId != 0x0000)
					{
						// Background: Decorations
						Raylib.DrawTexturePro(
							BlockRenderer.Texture,
							BlockRenderer.GetIdSprite(tile.ForegroundId),
							destination,
							Vector2.Zero,
							0.0f,
							Color.WHITE
						);
						// Foreground: Decorations
					}
				}
			}
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
	public JJx.World World {
		get { return this._World; }
		set {
			this._World = value;
			this._Camera.target = new Vector2(value.Player.X * 32, (value.Size.Height - value.Player.Y) * 32);
		}
	}
	private JJx.World _World = null;
	private Camera2D _Camera = new Camera2D(Vector2.Zero, Vector2.Zero, 0.0f, 1.0f);
	/* Class Properties */
	public static float Speed  = 10.0f;
	public static float Scroll =  0.25f;
	private const sbyte _OffScreenBuffer = -128;
}
