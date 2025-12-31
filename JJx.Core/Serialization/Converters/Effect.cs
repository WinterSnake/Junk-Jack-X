/*
	Junk Jack X: Core
	- [Serialization]Converter - Effect

	Written By: Ryan Smith
*/

namespace JJx.Core.Serialization;

internal sealed class EffectConverter : JJxConverter<Effect>
{
	/* Instance Methods */
	public override Effect Read(ref JJxReader reader)
	{
		var id = reader.ReadUInt16();
		var duration = reader.ReadUInt16();
		return new(id, duration);
	}
	public override void Write(Effect value, JJxWriter writer)
	{
		writer.Write(value.Id);
		writer.Write(value.Duration);
	}
}
