/*
    Junk Jack X Editor: Renderers
    - Item

    Written By: Ryan Smith
*/
using System;
using Raylib_cs;
using JJx;

public static class ItemRenderer
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
	public static Rectangle GetSprite(ushort id, ushort icon) => new Rectangle((id / 64 + icon) * 16, id % 64 * 16, 16, 16);
    /* Class Properties */
    public static bool _Loaded { get; private set; } = false;
    public static Texture2D Texture { get { return _Texture.Value; }}
    private static Texture2D? _Texture = null;
}
