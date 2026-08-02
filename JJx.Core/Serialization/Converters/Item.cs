/*
	Junk Jack X: Core
	- [Serialization]Converter - Item

	Written By: Ryan Smith
*/

namespace JJx.Core.Serialization;

internal sealed class ItemConverter : JJxConverter<Item>
{
	/* Instance Methods */
	public override Item Read(ref JJxReader reader, JJxSerializationOptions options)
	{
		var item = new Item() {
			Data=reader.ReadUInt32(),
			Id=reader.ReadUInt16(),
			Count=reader.ReadUInt16(),
			Durability=reader.ReadUInt16(),
			Variant=reader.ReadUInt8(),
		};
		if (!options.IsPacked)
			reader.Advance(1);
		return item;
	}
	public override void Write(in Item @value, JJxWriter writer, JJxSerializationOptions options)
	{
		writer.Write(@value.Data);
		writer.Write(@value.Id);
		writer.Write(@value.Count);
		writer.Write(@value.Durability);
		writer.Write(@value.Variant);
		if (!options.IsPacked)
			writer.Advance(1);
	}
}
