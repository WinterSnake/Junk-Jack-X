/*
	Junk Jack X: Player
	- Character Data

	Segment Breakdown:
	------------------------------------------------------------------------------------------------------------------------
	Segment[0x0] = Hair.Color: data >> 4
	Segment[0x1] = Tone: (data & 0xE0) >> 5 | Gender: (data & 0x10) >> 4 | Hair.Style: data & 0xF
	------------------------------------------------------------------------------------------------------------------------
	Length: 2 (0x2)

	Written By: Ryan Smith
*/
using System;

namespace JJx;

public sealed class Character
{
	/* Constructors */
	public Character(bool gender, byte tone, byte hairStyle, Hair.HColor hairColor)
	{
		this.Gender = gender;
		this.Tone = tone;
		this.Hair = new Hair(hairStyle, hairColor);
	}
	internal Character(ReadOnlySpan<byte> bytes)
	{
		this.Gender = ((bytes[1] & 0x10) >> 4) == 1;
		this._Tone = (byte)((bytes[1] & 0xE0) >> 5);
		this.Hair = new Hair(bytes);
	}
	/* Instance Methods */
	public void Pack(Span<byte> bytes)
	{
		bytes[0] = (byte)((byte)this.Hair.Color << 4);
		bytes[1] = (byte)((((this._Tone << 1) | Convert.ToByte(this.Gender)) << 4) | this.Hair.Style);
	}
	/* Properties */
	public bool Gender;  // Male: 0 | Female: 1
	private byte _Tone;
	public byte Tone {
		// Min: 0 | Max: 4
		get { return this._Tone; }
		set { this._Tone = Math.Min(value, MaxTones); }
	}
	public readonly Hair Hair;
	/* Class Properties */
	public const byte MaxTones = 0x4;  // Maximum skin tones in game (5) [0-4]
}
public sealed class Hair
{
	/* Constructors */
	public Hair(byte style, HColor color)
	{
		this.Style = style;
		this.Color = color;
	}
	internal Hair(ReadOnlySpan<byte> bytes)
	{
		this._Style = (byte)(bytes[1] & 0xF);
		this.Color = (HColor)(bytes[0] >> 4);
	}
	/* Properties */
	public HColor Color;
	private byte _Style;
	public byte Style {
		get { return this._Style; }
		set { this._Style = Math.Min(value, MaxStyles); }
	}
	/* Class Properties */
	public const byte MaxStyles = 0xD;  // Maximum hair styles in game (14) [0-D]
	/* Sub-Classes */
	public enum HColor : byte
	{
		White = 0x0,
		Grey,
		Black,
		Brown,
		DarkBrown,
		LightBrown,
		Blonde,
		DirtyBlonde,
		LightBlonde,
		Ginger,
		Red,
		Purple,
		Blue,
		Teal,
		Green,
		Yellow
	}
}
