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
    public static void InitTexture(string path)
    {
        if (_Loaded)
            return;
        _Texture = Raylib.LoadTexture(path);
        _Loaded = true;
    }
	public static void UnloadTexture()
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
	public static readonly NPatchInfo IconClosedChest = new NPatchInfo {
		source = new Rectangle(319, 415, 20, 20),
		left = 1, top = 1, right = 1, bottom = 1,
		layout = NPatchLayout.NPATCH_NINE_PATCH
	};
	public static readonly NPatchInfo IconOpenedChest = new NPatchInfo {
		source = new Rectangle(340, 436, 20, 20),
		left = 1, top = 1, right = 1, bottom = 1,
		layout = NPatchLayout.NPATCH_NINE_PATCH
	};
}
