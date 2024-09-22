/*
	Junk Jack X: Core
	- [Player]Character

	Written By: Ryan Smith
*/
using System;

namespace JJx.Serialization;

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
