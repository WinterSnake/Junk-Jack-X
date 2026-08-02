/*
	Junk Jack X: Core
	- [Player]Model

	Written By: Ryan Smith
*/

using System;

public enum HairColor : byte
{
	White       = 0x0,
	Gray        = 0x1,
	Black       = 0x2,
	Brown       = 0x3,
	DarkBrown   = 0x4,
	LightBrown  = 0x5,
	Blonde      = 0x6,
	DirtyBlonde = 0x7,
	LightBlonde = 0x8,
	Ginger      = 0x9,
	Red         = 0xA,
	Purple      = 0xB,
	Blue        = 0xC,
	Teal        = 0xD,
	Green       = 0xE,
	Yellow      = 0xF,
}

public sealed class CharacterModel
{
	/* Constructor */
	public CharacterModel(bool isFemale, byte skinTone, byte hairStyle, HairColor hairColor)
	{
		this.IsFemale = isFemale;
		this.SkinTone = skinTone;
		this.HairStyle = hairStyle;
		this.HairColor = hairColor;
	}
	/* Properties */
	public bool IsFemale;
	private byte _SkinTone;
	public byte SkinTone {
		get => this._SkinTone;
		set => this._SkinTone = Math.Min(value, MAX_SKINTONES);
	}
	private byte _HairStyle;
	public byte HairStyle {
		get => this._HairStyle;
		set => this._HairStyle = Math.Min(value, MAX_HAIRSTYLES);
	}
	public HairColor HairColor;
	/* Class Properties */
	public const byte MAX_SKINTONES   = 0x4;  // Maximum skin tones in game (5) [0-4]
	public const byte MAX_HAIRSTYLES  = 0xD;  // Maximum hair styles in game (15) [0-D]
}
