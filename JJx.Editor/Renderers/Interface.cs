/*
    Junk Jack X Editor: Renderers
    - Interface

    Written By: Ryan Smith
*/
using System;
using Raylib_cs;
using JJx;

public static class InterfaceRenderer
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
    /* Class Properties */
    public static bool _Loaded { get; private set; } = false;
    public static Texture2D Texture { get { return _Texture.Value; }}
    private static Texture2D? _Texture = null;
	// Mapping
	public static readonly Rectangle IconSelection   = new Rectangle(271, 213, 25, 25);
	public static readonly Rectangle IconClosedChest = new Rectangle(319, 415, 20, 20);
	public static readonly Rectangle IconOpenedChest = new Rectangle(340, 436, 20, 20);
	public static readonly Rectangle BackgroundItems = new Rectangle(721, 250, 74, 123);
	public static readonly Rectangle IconArrowLeft   = new Rectangle(298, 439, 20, 14);
	public static readonly Rectangle IconArrowRight  = new Rectangle(319, 439, 20, 14);
}
