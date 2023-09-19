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
    public void InitRenderer(string path)
    {
        if (_Loaded)
            return;
        _Texture = Raylib.LoadTexture(path);
        _Loaded = true;
    }
	public void UnloadRenderer()
	{
		if (!_Loaded)
			return;
        Raylib.UnloadTexture(Texture);
        _Loaded = true;
		_Texture = null;
	}
    /* Class Properties */
    public static bool _Loaded { get; private set; } = false;
    private static Texture2D? _Texture = null;
    public static Texture2D Texture { get { return _Texture.Value; }}
    // Mapping
}
