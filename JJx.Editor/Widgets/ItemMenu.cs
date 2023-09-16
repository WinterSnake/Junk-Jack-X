/*
    Junk Jack X Editor: Widget
    - Item Menu

    Written By: Ryan Smith
*/
using System;
using System.Numerics;
using Raylib_cs;

public sealed class ItemMenu
{
	/* Constructors */
	public ItemMenu()
	{
		this._LeftArrow.OnClick(() => { if (this._Page > 0) --this._Page; });
		this._RightArrow.OnClick(() => { if (this._Page < _MaxPage) ++this._Page; });
	}
    /* Instance Methods */
	public void Draw()
	{
		// Draw backdrop
		Raylib.DrawTextureNPatch(InterfaceRenderer.Texture, InterfaceRenderer.BackgroundItems, _BackgroundPosition, Vector2.Zero, 0.0f, Color.WHITE);
		// Draw page buttons
		if (this._Page > 0)
		{
			this._LeftArrow.Position = _PageLeftPosition;
			this._LeftArrow.Draw();
		}
		if (this._Page < _MaxPage)
		{
			this._RightArrow.Position = _PageRightPosition;
			this._RightArrow.Draw();
		}
		var (textX, textY) = this._PageTextPosition;
		Raylib.DrawText(this._PageText, textX, textY, _FontSize, Color.WHITE);
		// Items
		Raylib.DrawTextureRec(
			ItemRenderer.Texture, new Rectangle(0, 0, Width, Raylib.GetScreenHeight() - 32), new Vector2(Raylib.GetScreenWidth() - Width, 0), Color.WHITE
		);
	}
	public void Update(float delta)
	{
		this._LeftArrow.Update(delta);
		this._RightArrow.Update(delta);
	}
    /* Properties */
	private byte _Page = 0;
	private string _PageText { get { return $"Page: {_Page}/{_MaxPage}"; }}
	private (int X, int Y) _PageTextPosition {
		get {
			var xOffset = Raylib.MeasureText(this._PageText, _FontSize) / 2;
			return ((int)(Raylib.GetScreenWidth() - Width / 2 - xOffset), Raylib.GetScreenHeight() - 24);
		}
	}
	private readonly NPatchButton _LeftArrow = new NPatchButton(_PageLeftPosition, InterfaceRenderer.Texture, InterfaceRenderer.IconArrowLeft);
	private readonly NPatchButton _RightArrow = new NPatchButton(_PageRightPosition, InterfaceRenderer.Texture, InterfaceRenderer.IconArrowRight);
	/* Class Properties */
	public const uint Width = 512;
	private const byte _MaxPage = 10;
	private const byte _FontSize = 22;
	private static Rectangle _BackgroundPosition {
		get { return new Rectangle(Raylib.GetScreenWidth() - Width, 0, Width, Raylib.GetScreenHeight()); }
	}
	private static Rectangle _PageLeftPosition {
		get { return new Rectangle(Raylib.GetScreenWidth() - Width, Raylib.GetScreenHeight() - 32, 64, 32); }
	}
	private static Rectangle _PageRightPosition {
		get { return new Rectangle(Raylib.GetScreenWidth() - 64, Raylib.GetScreenHeight() - 32, 64, 32); }
	}
}
