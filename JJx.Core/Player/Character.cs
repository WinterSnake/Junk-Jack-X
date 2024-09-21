/*
	Junk Jack X: Core
	- [Player]Character

	Written By: Ryan Smith
*/
using System;

namespace JJx;
using Serialization;

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
	/* Constructor */
	public Character(bool gender, byte skinTone, byte hairStyle, HairColor hairColor)
	{
		this.Gender = gender;
		this.SkinTone = skinTone;
		this.HairStyle = hairStyle;
		this.HairColor = hairColor;
	}
	/* Instance Methods */
	public override string ToString()
	{
		var gender = this.Gender ? "Female" : "Male";
		return $"Gender: {gender} ; Skin: {this.SkinTone} ; Hair.Style: {this.HairStyle} ; Hair.Color: {this.HairColor}";
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
}

internal sealed class CharacterConverter : JJxConverter<Character>
{
	/* Instance Methods */
	public override Character Read(JJxReader reader)
	{
		var @value = reader.GetUInt16();
		byte tone       =      (byte)((@value & TONE_FLAG)   >> TONE_SHIFT);
		bool gender     =            ((@value & GENDER_FLAG) >> GENDER_SHIFT) == 1;
		byte style      =      (byte)((@value & STYLE_FLAG)  >> STYLE_SHIFT);
		HairColor color = (HairColor)((@value & COLOR_FLAG)  >> COLOR_SHIFT);
		return new Character(gender, tone, style, color);
	}
	/* Class Properties */
	private const ushort GENDER_FLAG  = 0x1000;
	private const ushort TONE_FLAG    = 0xE000;
	private const ushort STYLE_FLAG   = 0x0F00;
	private const ushort COLOR_FLAG   = 0x00F0;
	private const byte   TONE_SHIFT   = 13;
	private const byte   GENDER_SHIFT = 12;
	private const byte   STYLE_SHIFT  =  8;
	private const byte   COLOR_SHIFT  =  4;
}
