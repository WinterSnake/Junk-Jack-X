/*
	Junk Jack X: Player
	- Character

	Segment Breakdown:
	---------------------------------------------------------------------------------------------
	Segment[0x0] = Hair.Color: data >> 4
	Segment[0x1] = Tone: (data & 0xE0) >> 5 | Gender: (data & 0x10) >> 4 | Hair.Style: data & 0xF
	---------------------------------------------------------------------------------------------
	Size: 2 (0x2)

	Written By: Ryan Smith
*/
using System;

namespace JJx;

public enum HairColor : byte
{
	White       = 0x0,
	Grey        = 0x1,
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
	public override string ToString()
	{
		var gender = this.Gender ? "Female" : "Male";
		return $"Gender={gender};Race={this.SkinTone};Hair[Style={this.HairStyle};Color={this.HairColor}]";
	}
	public void Pack(Span<byte> bytes, int offset = 0)
	{
		if (bytes.Length + offset < SIZE) throw new IndexOutOfRangeException();
		bytes[offset + 0] = (byte)((byte)this.HairColor << 4);
		bytes[offset + 1] = (byte)((((this._SkinTone << 1) | Convert.ToByte(this.Gender)) << 4) | this._HairStyle);
	}
	/* Static Methods */
	public static Character Unpack(ReadOnlySpan<byte> buffer, int offset = 0)
	{
		if (buffer.Length < offset + SIZE) throw new IndexOutOfRangeException();
		var gender = ((buffer[offset + 1] & 0x10) >> 4) == 1;
		var tone   = (byte)((buffer[offset + 1] & 0xE0) >> 5);
		var style  = (byte)(buffer[offset + 1] & 0xF);
		var color  = (HairColor)(buffer[offset + 0] >> 4);
		return new Character(gender, tone, style, color);
	}
	/* Properties */
	public bool Gender;  // Male: 0 | Female: 1
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
	internal const byte SIZE           =    2;
	public   const byte MAX_SKINTONES  =  0x4;  // Maximum skin tones in game (5) [0-4]
	public   const byte MAX_HAIRSTYLES =  0xD;  // Maximum hair styles in game (14) [0-D]
}
