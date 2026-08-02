/*
	Junk Jack X: Core
	- [Serialization]Converter - Effect

	Written By: Ryan Smith
*/

namespace JJx.Core.Serialization;

internal sealed class EffectConverter : JJxConverter<Effect>
{
	/* Instance Methods */
	public override Effect Read(ref JJxReader reader, JJxSerializationOptions options) => new Effect(
		reader.ReadUInt16(),
		reader.ReadUInt16()
	);
	public override void Write(in Effect @value, JJxWriter writer, JJxSerializationOptions options)
	{
		writer.Write(@value.Id);
		writer.Write(@value.Duration);
	}
}
