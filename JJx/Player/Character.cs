/*
	Junk Jack X: Player
	- Character

	Segment Breakdown:
	------------------------------------------------------------------------------------------------------------------------
	Segment[0x0] = Hair.Color: data >> 4
	Segment[0x1] = Tone:  (data & 0xE0) >> 5 | Gender: (data & 0x10) >> 4 | Hair.Style: data & 0xF
	------------------------------------------------------------------------------------------------------------------------
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
	internal Character(byte color, byte features)
	{
		this.Gender = ((features & 0x10) >> 4) == 1;
		this._Tone = (byte)((features & 0xE0) >> 5);
		this.Hair = new Hair(color, features);
	}
	/* Properties */
	public bool Gender;  // Male: 0 | Female: 1
	private byte _Tone;
	public byte Tone {
		// Min: 0 | Max: 4
		get { return this._Tone; }
		set {
			value = Math.Min(value, MaxTones);
			this._Tone = value;
		}
	}
	public readonly Hair Hair;
	/* Class Properties */
	public const byte MaxTones = 0x4;
}
public sealed class Hair
{
	/* Constructors */
	public Hair(byte style, HColor color)
	{
		this.Style = style;
		this.Color = color;
	}
	internal Hair(byte color, byte features)
	{
		this._Style = (byte)(features & 0xF);
		this.Color = (HColor)(color >> 4);
	}
	/* Properties */
	public HColor Color;
	private byte _Style;
	public byte Style {
		// Min: 0 | Max: 13
		get { return this._Style; }
		set {
			value = Math.Min(value, MaxStyles);
			this._Style = value;
		}
	}
	/* Class Properties */
	public const byte MaxStyles = 0xD;
	/* Sub-Classes */
	public enum HColor : byte
	{
		White       = 0x0,
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
