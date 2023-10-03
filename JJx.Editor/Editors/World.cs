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
	public WorldEditor()
	{
	}
	/* Instance Methods */
	public void Draw()
	{

	}
	public void Update(float delta)
	{
	}
	/* Properties */
	public JJx.World ActiveWorld {
		get { return this._ActiveWorld; }
		set {
			this._ActiveWorld = value;
			// Edit camera
		}
	}
	private JJx.World _ActiveWorld = null;
	public Camera2D Camera { get; private set; }
	private const byte _Speed = 10;
}
