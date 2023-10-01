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
		this._ShowItemsButton.OnPressed += _ToggleItems;
	}
	/* Instance Methods */
	public void Draw()
	{
		/// Player GUI
		/// Editor GUI
		// Items
		this._ShowItemsButton.Draw();
		//this._ShowItemsButton.Position = _ShowItems ? _ChestOpenedPosition : _ChestClosedPosition;
	}
	public void Update(float delta)
	{
		/// Player GUI
		/// Editor Gui
		// Items
		this._ShowItemsButton.Position = this._ShowItems ? _ChestOpenedPosition : _ChestClosedPosition;
		this._ShowItemsButton.Update(delta);
	}
	private void _ToggleItems(object sender, EventArgs args)
	{
		this._ShowItems = !this._ShowItems;
		this._ShowItemsButton.SourceDefault = this._ShowItems ? InterfaceRenderer.IconOpenedChest : InterfaceRenderer.IconClosedChest;
	}
	/* Properties */
	//public JJx.Player ActivePlayer;
	private bool _ShowItems = false;
	private readonly Button _ShowItemsButton = new Button(
		_ChestClosedPosition, InterfaceRenderer.IconClosedChest, InterfaceRenderer.Texture
	);
	//private readonly ItemMenu _Items = new ItemMenu();
	/* Class Properties */
	// Editor GUI
	private static Rectangle _ChestClosedPosition {
		get { return new Rectangle(Raylib.GetScreenWidth() - 64, 0, 64, 64); }
	}
	private static Rectangle _ChestOpenedPosition {
		get { return new Rectangle(Raylib.GetScreenWidth() - 64,  0, 64, 64); }
	}
}
