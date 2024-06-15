/*
	Junk Jack X Tools: Editor
	- [Editor]Player

	Written By: Ryan Smith
*/
using System;
using System.Numerics;
using Raylib_cs;
using JJx;

public sealed class PlayerEditor : EditorBase
{
	/* Constructor */
	public PlayerEditor(string filePath, Player player): base(filePath)
	{
		this._Player = player;
	}
	/* Instance Methods */
	public override void Draw()
	{
		// Draw Inventory
		for (var i = 0; i < 36; ++i)
		{
			var item = this._Player.Items[i + 29];
			if (item.Id == 0xFFFF)
				continue;
			var x = 128 + ((i / 6) * ITEM_DRAW_SIZE);
			var y = 128 + ((i % 6) * ITEM_DRAW_SIZE);
			var itemSrc = ItemRenderer.GetItemSource(item);
			var itemDest = new Rectangle(x, y, ITEM_DRAW_SIZE, ITEM_DRAW_SIZE);
			Raylib.DrawTexturePro(ItemRenderer.Texture, itemSrc, itemDest, Vector2.Zero, 0.0f, Color.White);
		}
	}
	public override void Update(float deltaTime)
	{

	}
	/* Properties */
	private readonly Player _Player;
	/* Class Properties */
	private const byte ITEM_DRAW_SIZE = 64;
}
