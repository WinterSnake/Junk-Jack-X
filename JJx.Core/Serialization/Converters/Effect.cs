/*
	Junk Jack X: Core
	- [Serialization]Converter - Effect

	Written By: Ryan Smith
*/

namespace JJx.Core.Serialization;

internal sealed class EffectConverter : JJxConverter<Effect>
{
	/* Instance Methods */
	public override Effect Read(ref JJxReader reader) => new Effect(
		reader.ReadUInt16(),
		reader.ReadUInt16()
	);
	public override void Write(Effect @value, JJxWriter writer)
	{
		writer.Write(@value.Id);
		writer.Write(@value.Duration);
	}
}
