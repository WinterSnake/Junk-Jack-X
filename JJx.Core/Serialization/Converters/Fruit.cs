/*
	Junk Jack X: Core
	- [Serialization]Converter - Fruit

	Written By: Ryan Smith
*/

namespace JJx.Core.Serialization;

internal sealed class FruitConverter : JJxConverter<Fruit>
{
	/* Instance Methods */
	public override Fruit Read(ref JJxReader reader) => new(
		reader.ReadUInt16(), reader.ReadUInt16()
	);
	public override void Write(in Fruit @value, JJxWriter writer)
	{
		writer.Write(@value.Position.X);
		writer.Write(@value.Position.Y);
	}
}
