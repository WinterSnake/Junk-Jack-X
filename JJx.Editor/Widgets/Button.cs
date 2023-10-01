/*
    Junk Jack X Editor: Widget
    - Button

    Written By: Ryan Smith
*/
using System;
using System.Numerics;
using Raylib_cs;

public class Button
{
	/* Constructors */
	public Button(Rectangle position, Rectangle sprite, Texture2D texture)
	{
		this.Position = position;
		this._Sources[0] = sprite;
		this._Texture = texture;
	}
	public Button(Rectangle position, Rectangle sprite, Texture2D texture, (int, int, int, int) padding): this(position, sprite, texture)
	{
		this._IsNPatch = true;
		this.Padding = padding;
		this.NPatchLayout = NPatchLayout.NPATCH_NINE_PATCH;
	}
	/* Instance Methods */
	public void Draw()
	{
		Rectangle source;
		switch (this._State)
		{
			case State.Hover:
			{
				if (this.SourceHover.HasValue)
				{
					source = this.SourceHover.Value;
					break;
				}
				goto default;
			}
			case State.Pressed:
			{
				if (this.SourcePressed.HasValue)
				{
					source = this.SourcePressed.Value;
					break;
				}
				goto default;
			}
			default:
			{
				source = this.SourceDefault;
			} break;
		}
		if (this._IsNPatch)
			Raylib.DrawTextureNPatch(
				this._Texture,
				new NPatchInfo {
					source = source,
					top = this.Padding.Value.Top,
					bottom = this.Padding.Value.Bottom,
					left = this.Padding.Value.Left,
					right = this.Padding.Value.Right,
					layout = this.NPatchLayout
				},
				this.Position,
				Vector2.Zero,
				0.0f,
				Color.WHITE
			);
		else
			Raylib.DrawTexturePro(
				this._Texture,
				source,
				this.Position,
				Vector2.Zero,
				0.0f,
				Color.WHITE
			);
	}
	public void Update(float delta)
	{
		var mouseVector = Raylib.GetMousePosition();
		if (Raylib.CheckCollisionPointRec(mouseVector, this.Position))
		{
			this._State = State.Hover;
			if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
			{
				this._State = State.Pressed;
				OnPressed(this, EventArgs.Empty);
			}
		}
	}
	/* Properties */
	public event EventHandler OnPressed;
	public Rectangle Position;
	private State _State = State.Default;
	private readonly NPatchLayout NPatchLayout;
	private readonly Rectangle?[] _Sources = { null, null, null };
	public (int Top, int Bottom, int Left, int Right)? Padding = null;
	private readonly Texture2D _Texture;
	private readonly bool _IsNPatch = false;
	public Rectangle SourceDefault {
		get { return this._Sources[0].Value; }
		set { this._Sources[0] = value; }
	}
	public Rectangle? SourceHover {
		get { return this._Sources[1]; }
		set { this._Sources[1] = value; }
	}
	public Rectangle? SourcePressed {
		get { return this._Sources[2]; }
		set { this._Sources[2] = value; }
	}
	/* Sub-Classes */
	public enum State : byte
	{
		Default = 0,
		Hover = 1,
		Pressed = 2,
	}
}
