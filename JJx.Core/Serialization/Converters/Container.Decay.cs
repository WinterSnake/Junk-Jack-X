/*
	Junk Jack X: Core
	- [Serialization]Converter - Decay

	Written By: Ryan Smith
*/

namespace JJx.Core.Serialization;

internal sealed class DecayConverter : JJxConverter<Decay>
{
	/* Instance Methods */
	public override Decay Read(ref JJxReader reader) => new(
		reader.ReadUInt16(), reader.ReadUInt16()
	);
	public override void Write(in Decay @value, JJxWriter writer)
	{
		writer.Write(@value.Position.X);
		writer.Write(@value.Position.Y);
	}
}
