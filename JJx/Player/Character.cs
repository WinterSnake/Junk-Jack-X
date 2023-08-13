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

public struct Character
{
	/* Constructors */
	internal Character(byte color, byte features)
	{
		this.Gender = ((features & 0x10) >> 4) == 1;
		this._Tone = (byte)((features & 0xE0) >> 5);
		this.Hair = new _Hair(color, features);
	}
	public Character(bool gender, byte tone, byte hairStyle, HairColor hairColor)
	{
		this.Gender = gender;
		this.Tone = tone;
		this.Hair = new _Hair(hairStyle, hairColor);
	}
	/* Instance Methods */
	public byte[] ToByteArray()
	{
		var data = new byte[2];
		data[0] = (byte)((byte)this.Hair.Color << 4);
		data[1] = (byte)((((this._Tone << 3) | Convert.ToByte(this.Gender)) << 4) | this.Hair.Style);
		return data;
	}
	public void ToByteArray(ref byte[] bytes) => Array.Copy(this.ToByteArray(), bytes, 2);
	/* Properties */
	public bool Gender;  // Male: 0 | Female: 1
	private byte _Tone;
	public byte Tone {
		// Min: 0 | Max: 5
		get { return this._Tone; }
		set {
			value = Math.Min(value, (byte)0x4);
			this._Tone = value;
		}
	}
	public _Hair Hair;
	/* Sub-Classes */
	public class _Hair
	{
		/* Constructor */
		internal _Hair(byte color, byte features)
		{
			this._Style = (byte)(features & 0xF);
			this.Color = (HairColor)(color >> 4);
		}
		public _Hair(byte style, HairColor color)
		{
			this.Style = style;
			this.Color = color;
		}
		/* Properties */
		public HairColor Color;
		private byte _Style;
		public byte Style {
			// Min: 0 | Max: 13
			get { return this._Style; }
			set {
				value = Math.Min(value, (byte)0xD);
				this._Style = value;
			}
		}
	}
	public enum HairColor : byte
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
