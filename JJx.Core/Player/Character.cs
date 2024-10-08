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
	/* Static Methods */
	#nullable enable
	public static Character Random(System.Random? rng = null)
	{
		rng ??= System.Random.Shared;
		var gender    = rng.NextDouble() >= 0.5f;
		var skinTone  = (byte)rng.Next(MAX_SKINTONES  + 1);
		var hairStyle = (byte)rng.Next(MAX_HAIRSTYLES + 1);
		var hairColor = (HairColor)(rng.Next(Enum.GetValues(typeof(HairColor)).Length + 1));
		return new Character(gender, skinTone, hairStyle, hairColor);
	}
	#nullable disable
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
