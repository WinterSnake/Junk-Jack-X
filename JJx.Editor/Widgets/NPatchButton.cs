/*
    Junk Jack X Editor: Widget
    - Button

    Written By: Ryan Smith
*/
using System;
using System.Numerics;
using Raylib_cs;

public class NPatchButton
{
	/* Constructors */
	public NPatchButton(Rectangle position, Texture2D texture, NPatchInfo sprite)
	{
		this.Position = position;
		this.NPatchSprite = sprite;
		this._Texture = texture;
	}
    /* Instance Methods */
	public void Draw()
	{
		Raylib.DrawTextureNPatch(
			this._Texture, this.NPatchSprite, this.Position, Vector2.Zero, 0.0f, Color.WHITE
		);
	}
	public void Update(float delta)
	{
		var mouseVector = Raylib.GetMousePosition();
		if (
			Raylib.CheckCollisionPointRec(mouseVector, this.Position) &&
			Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT)
		)
			this._Action();
	}
	public void OnClick(Action action)
	{
		this._Action = action;
	}
    /* Properties */
	public Rectangle Position;
	public NPatchInfo NPatchSprite;
	private Action _Action;
	private readonly Texture2D _Texture;
}
