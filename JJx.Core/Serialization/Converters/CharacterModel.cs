/*
	Junk Jack X: Core
	- [Serialization]Converter - Character Model

	Written By: Ryan Smith
*/

using System;

namespace JJx.Core.Serialization;

internal sealed class CharacterModelConverter : JJxConverter<CharacterModel>
{
    /* Instance Methods */
    public override CharacterModel Read(ref JJxReader reader)
	{
		var model  = reader.ReadUInt16();
		var tone   =      (byte)((model & TONE_FLAG)   >> TONE_SHIFT);
		var gender =            ((model & GENDER_FLAG) >> GENDER_SHIFT) == 1;
		var style  =      (byte)((model & STYLE_FLAG)  >> STYLE_SHIFT);
		var color  = (HairColor)((model & COLOR_FLAG)  >> COLOR_SHIFT);
		return new(gender, tone, style, color);
	}
    public override void Write(in CharacterModel @value, JJxWriter writer)
    {
		var model = (ushort)(
			(@value.SkinTone << TONE_SHIFT) |
			(Convert.ToByte(@value.IsFemale) << GENDER_SHIFT) |
			(@value.HairStyle << STYLE_SHIFT) |
			((byte)@value.HairColor << COLOR_SHIFT)
		);
		writer.Write(model);
    }
	/* Class Properties */
	private const ushort GENDER_FLAG  = 0x1000;
	private const ushort TONE_FLAG    = 0xE000;
	private const ushort STYLE_FLAG   = 0x0F00;
	private const ushort COLOR_FLAG   = 0x00F0;
	private const int    TONE_SHIFT   = 13;
	private const int    GENDER_SHIFT = 12;
	private const int    STYLE_SHIFT  =  8;
	private const int    COLOR_SHIFT  =  4;
}
