/*
	Junk Jack X: Core
	- [Player]Character

	Written By: Ryan Smith
*/
using System;

namespace JJx;

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

public sealed class Character
{
	/* Constructors */
	public Character(bool gender, byte skinTone, byte hairStyle, HairColor hairColor)
	{
		this.Gender = gender;
		this.SkinTone = skinTone;
		this.HairStyle = hairStyle;
		this.HairColor = hairColor;
	}
	/* Instance Methods */
	public ushort Pack() => (ushort)((this._SkinTone << 13) | (Convert.ToByte(this.Gender) << 12) | (this._HairStyle << 8) | ((byte)this.HairColor << 4));
	/* Static Methods */
	public static Character Unpack(ushort @value)
	{
		byte tone       = (byte)     ((@value & TONE_FLAG)   >> 13);
		bool gender     =            ((@value & GENDER_FLAG) >> 12) == 1;
		byte style      = (byte)     ((@value & STYLE_FLAG)  >>  8);
		HairColor color = (HairColor)((@value & COLOR_FLAG)  >>  4);
		return new Character(gender, tone, style, color);
	}
	/* Properties */
	public bool Gender;  // Male: false | Female: true
	private byte _SkinTone;
	public byte SkinTone {
		get { return this._SkinTone; }
		set { this._SkinTone = Math.Min(value, MAX_SKINTONES); }
	}
	private byte _HairStyle;
	public byte HairStyle {
		get { return this._HairStyle; }
		set { this._HairStyle = Math.Min(value, MAX_HAIRSTYLES); }
	}
	public HairColor HairColor;
	/* Class Properties */
	public const byte MAX_SKINTONES  = 0x4;  // Maximum skin tones in game (5) [0-4]
	public const byte MAX_HAIRSTYLES = 0xD;  // Maximum hair styles in game (14) [0-D]
	private const ushort GENDER_FLAG = 0x1000;
	private const ushort TONE_FLAG   = 0xE000;
	private const ushort STYLE_FLAG  = 0x0F00;
	private const ushort COLOR_FLAG  = 0x00F0;
}
