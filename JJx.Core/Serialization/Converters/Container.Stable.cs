/*
	Junk Jack X: Core
	- [Serialization]Converter - Stable

	Written By: Ryan Smith
*/

namespace JJx.Core.Serialization;

internal sealed class StableConverter : JJxConverter<Stable>
{
	/* Instance Methods */
	public override Stable Read(ref JJxReader reader)
	{
		var stable = new Stable(
			reader.ReadUInt16(), reader.ReadUInt16()
		);
		reader.ReadSpan(stable.Unknown);
		stable.Feed = reader.ReadObject<Item>();
		return stable;
	}
	public override void Write(in Stable @value, JJxWriter writer)
	{
		writer.Write(@value.Position.X);
		writer.Write(@value.Position.Y);
		writer.Write(@value.Unknown);
		writer.Write(@value.Feed);
	}
}
