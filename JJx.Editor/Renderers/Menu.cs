/*
	Junk Jack X Tools: Editor
	- [Renderer]Menu

	Written By: Ryan Smith
*/
using System;
using Raylib_cs;
using JJx;

public static class MenuRenderer
{
	/* Static Methods */
	public static void Load(string dataPath) => Texture = Raylib.LoadTexture(dataPath + "/gfx/mainmenu.png");
	public static void Unload() => Raylib.UnloadTexture(Texture);
	// Spritesheet
	public static Rectangle GetPlanetIcon(Planet planet)
	{
		switch (planet)
		{
			case Planet.Terra:  return new Rectangle( 20, 107, PLANET_ICON_SIZE, PLANET_ICON_SIZE);
			case Planet.Seth:   return new Rectangle(105, 107, PLANET_ICON_SIZE, PLANET_ICON_SIZE);
			case Planet.Alba:   return new Rectangle( 37, 107, PLANET_ICON_SIZE, PLANET_ICON_SIZE);
			case Planet.Xeno:   return new Rectangle( 54, 107, PLANET_ICON_SIZE, PLANET_ICON_SIZE);
			case Planet.Magmar: return new Rectangle( 71, 107, PLANET_ICON_SIZE, PLANET_ICON_SIZE);
			case Planet.Cryo:   return new Rectangle( 88, 107, PLANET_ICON_SIZE, PLANET_ICON_SIZE);
			case Planet.Yuca:   return new Rectangle(  3,  90, PLANET_ICON_SIZE, PLANET_ICON_SIZE);
			case Planet.Lilith: return new Rectangle( 20,  90, PLANET_ICON_SIZE, PLANET_ICON_SIZE);
			case Planet.Thetis: return new Rectangle( 37,  90, PLANET_ICON_SIZE, PLANET_ICON_SIZE);
			case Planet.Mykon:  return new Rectangle( 54,  90, PLANET_ICON_SIZE, PLANET_ICON_SIZE);
			case Planet.Umbra:  return new Rectangle( 71,  90, PLANET_ICON_SIZE, PLANET_ICON_SIZE);
			case Planet.Tor:    return new Rectangle( 88,  90, PLANET_ICON_SIZE, PLANET_ICON_SIZE);
			default: throw new ArgumentException($"Unknown planet type '{planet}'");
		}
	}
	public static Rectangle GetGamemodeIcon(Gamemode gamemode)
	{
		switch (gamemode)
		{
			case Gamemode.Adventure: return new Rectangle(1, 105, GAMEMODE_ICON_SIZE, GAMEMODE_ICON_SIZE);
			case Gamemode.Survival:  return new Rectangle(1, 155, GAMEMODE_ICON_SIZE, GAMEMODE_ICON_SIZE);
			case Gamemode.Creative:
			case Gamemode.Flat:
			{
				return new Rectangle(1, 122, GAMEMODE_ICON_SIZE, GAMEMODE_ICON_SIZE);
			}
			default: throw new ArgumentException($"Unknown gamemode type '{gamemode}'");
		}
	}
	public static Rectangle GetSeasonIcon(Season season)
	{
		switch(season)
		{
			case Season.Spring: return new Rectangle(63, 74, 10, 10);
			case Season.Summer: return new Rectangle(77, 72, 13, 13);
			case Season.Autumn: return new Rectangle(16, 72, 14, 13);
			case Season.Winter: return new Rectangle(31, 72, 13, 13);
			default: throw new ArgumentException($"Unknown season type '{season}'");
		}
	}
	/* Class Properties */
	public static Texture2D Texture { get; private set; }
	private const int PLANET_ICON_SIZE = 12;
	private const int GAMEMODE_ICON_SIZE = 16;
}
