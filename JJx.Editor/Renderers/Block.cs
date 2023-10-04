/*
    Junk Jack X Editor: Renderers
    - Block

    Written By: Ryan Smith
*/
using System;
using Raylib_cs;
using JJx;

public sealed class BlockRenderer
{
    /* Static Methods */
    public static void InitRenderer(string path)
    {
        if (_Loaded)
            return;
        _Texture = Raylib.LoadTexture(path);
        _Loaded = true;
    }
	public static void UnloadRenderer()
	{
		if (!_Loaded)
			return;
        Raylib.UnloadTexture(Texture);
        _Loaded = false;
		_Texture = null;
	}
	public static Rectangle GetIdSprite(ushort id)
	{
		var special = (id & 0x7000) >> 12;
		var shift   = (id & 0x0800) >>  6;
		if (special > 0 || shift > 0)
			id &= 0x07FF;
		int x = ((id % 32) | shift) + special;
		int y = id / 32;
		return new Rectangle(x * 32, y * 32, 32, 32);
	}
    /* Class Properties */
    public static bool _Loaded { get; private set; } = false;
    private static Texture2D? _Texture = null;
    public static Texture2D Texture { get { return _Texture.Value; }}
    // Mapping
	public static readonly Rectangle Border = new Rectangle(31 * 32, 7 * 32, 32, 32);
}
