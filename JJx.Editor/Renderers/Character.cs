/*
	Junk Jack X Tools: Editor
	- [Renderer]Character

	Written By: Ryan Smith
*/
using System;
using Raylib_cs;
using JJx;

public static class CharacterRenderer
{
	/* Static Methods */
	public static void Load(string dataPath) => Texture = Raylib.LoadTexture(dataPath + "/gfx/equip.png");
	public static void Unload() => Raylib.UnloadTexture(Texture);
	// Spritesheet
	public static Rectangle GetHeadSprite(Character character)
	{
		return new Rectangle(
			(character.SkinTone + Convert.ToInt32(character.Gender) * (Character.MAX_SKINTONES + 1)) * HEAD_SPRITE_SIZE,
			0, HEAD_SPRITE_SIZE, HEAD_SPRITE_SIZE
		);
	}
	public static Rectangle GetHairSprite(Character character)
	{
		var startPosition = character.Gender ? 235 : 237;
		var hairSpriteSize = character.Gender ? 18 : 15;
		return new Rectangle(
			startPosition + (character.HairStyle * 19),
			character.Gender ? 14 : 0,
			hairSpriteSize, hairSpriteSize
		);
	}
	// Colors
	public static Color GetHairColor(Character character)
	{
		switch (character.HairColor)
		{
			case HairColor.White:       return Color.LightGray;
			case HairColor.Gray:        return Color.Gray;
			case HairColor.Black:       return new(25, 25, 25, 255);
			case HairColor.Brown:       return new(98, 58, 36, 255);
			case HairColor.DarkBrown:   return Color.DarkBrown;
			case HairColor.LightBrown:  return new(145, 99, 54, 255);
			case HairColor.Blonde:      return Color.Gold;
			case HairColor.DirtyBlonde: return Color.Beige;
			case HairColor.LightBlonde: return new(232, 210, 150, 255);
			case HairColor.Ginger:      return Color.Orange;
			case HairColor.Red:         return Color.Maroon;
			case HairColor.Purple:      return Color.Violet;
			case HairColor.Blue:        return Color.Blue;
			case HairColor.Teal:        return new(7, 134, 135, 255);
			case HairColor.Green:       return Color.DarkGreen;
			case HairColor.Yellow:      return Color.Yellow;
			default: throw new ArgumentException($"Unknown hair color '{character.HairColor}'");
		}
	}
	/* Class Properties */
	public static Texture2D Texture { get; private set; }
	private const int HEAD_SPRITE_SIZE = 17;
}
