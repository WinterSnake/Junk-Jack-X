/*
	Junk Jack X: Editor
	- Editor

	Written By: Ryan Smith
*/
using System;
using System.Numerics;
using Raylib_cs;
using JJx;

public sealed class WorldEditor
{
	/* Constructors */
	public WorldEditor(JJx.World world)
	{
		this._ActiveWorld = world;
		this.Camera = new Camera2D(
			new Vector2(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2),  // Offset
			new Vector2(world.Player.X * 32, (world.Size.Height - world.Player.Y) * 32),  // Target
			0.0f, 1.0f  // Rotation, Zoom
		);
	}
	/* Instance Methods */
	public void Draw()
	{

	}
	public void Update(float delta)
	{
		/*
		float speed = (float)_Speed * delta;
		if (Raylib.IsKeyDown(KeyboardKey.KEY_W))
			this.Camera.target.Y -= speed;
		else if (Raylib.IsKeyDown(KeyboardKey.KEY_S))
			this.Camera.target.Y += speed;
		if (Raylib.IsKeyDown(KeyboardKey.KEY_A))
			this.Camera.target.X -= speed;
		else if (Raylib.IsKeyDown(KeyboardKey.KEY_D))
			this.Camera.target.X += speed;
		*/
	}
	/* Properties */
	public Camera2D Camera { get; private set; }
	private JJx.World _ActiveWorld;
	private const byte _Speed = 10;
}
