/*
	Junk Jack X: Editors
	- Player

	Written By: Ryan Smith
*/
using System;
using System.Numerics;
using Raylib_cs;
using JJx;

internal sealed class PlayerEditor
{
	/* Constructors */
	public PlayerEditor()
	{
		this._ShowItemsButton = new NPatchButton(
			_ChestClosedLocation, InterfaceRenderer.Texture, InterfaceRenderer.IconClosedChest
		);
		this._ShowItemsButton.OnClick(_ToggleItems);
	}
	/* Instance Methods */
	public void Draw()
	{
		/// Player GUI
		/// Editor GUI
		// Items
		this._ShowItemsButton.Draw();
	}
	public void Update(float delta)
	{
		/// Player GUI
		/// Editor Gui
		// Items
		this._ShowItemsButton.Update(delta);
	}
	private void _ToggleItems()
	{
		this._ShowItems = !_ShowItems;
		this._ShowItemsButton.Position = _ShowItems ? _ChestOpenLocation : _ChestClosedLocation;
		this._ShowItemsButton.NPatchSprite = _ShowItems ? InterfaceRenderer.IconOpenedChest : InterfaceRenderer.IconClosedChest;
	}
	/* Properties */
	private bool _ShowItems = false;
	/* Class Properties */
	private NPatchButton _ShowItemsButton;
	// Editor GUI
	private static readonly Rectangle _ChestOpenLocation = new Rectangle(Raylib.GetScreenWidth() - 32, 0, 64, 64);
	private static readonly Rectangle _ChestClosedLocation = new Rectangle(Raylib.GetScreenWidth() - 32, 0, 64, 64);
}
