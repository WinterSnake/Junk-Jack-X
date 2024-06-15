/*
	Junk Jack X Tools: Editor
	- [Renderer]Item

	Written By: Ryan Smith
*/
using Raylib_cs;
using JJx;

public static class ItemRenderer
{
	/* Static Methods */
	public static void InitRenderer(string dataPath)
	{
		Texture = Raylib.LoadTexture(dataPath + "gfx/treasures.png");
	}
	public static Rectangle GetItemSource(Item item)
	{
		var x = ((item.Id / (Texture.Height / ITEM_DRAW_SIZE)) + item.Icon) * ITEM_DRAW_SIZE;
		var y = (item.Id % (Texture.Height / ITEM_DRAW_SIZE)) * ITEM_DRAW_SIZE;
		return new Rectangle(x, y, ITEM_DRAW_SIZE, ITEM_DRAW_SIZE);
	}
	/* Class Properties */
	public static Texture2D Texture { get; private set; }
	private const byte ITEM_DRAW_SIZE = 16;
}
