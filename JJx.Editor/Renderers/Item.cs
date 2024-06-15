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
		var x = ((item.Id / (Texture.Height / 16)) + item.Icon) * 16;
		var y = (item.Id % (Texture.Height / 16)) * 16;
		return new Rectangle(x, y, 16, 16);
	}
	/* Class Properties */
	public static Texture2D Texture { get; private set; }
}
