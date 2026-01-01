/*
	Junk Jack X: Core
	- [Serialization]Converter - Shelf

	Written By: Ryan Smith
*/

namespace JJx.Core.Serialization;

internal sealed class ShelfConverter : JJxConverter<Shelf>
{
	/* Instance Methods */
	public override Shelf Read(ref JJxReader reader)
	{
		var shelf = new Shelf(
			reader.ReadUInt16(), reader.ReadUInt16()
		);
		foreach (ref var item in shelf.Items)
			item = reader.ReadObject<Item>();
		return shelf;
	}
	public override void Write(in Shelf @value, JJxWriter writer)
	{
		writer.Write(@value.Position.X);
		writer.Write(@value.Position.Y);
		foreach (ref var item in @value.Items)
			writer.Write(item);
	}
}
