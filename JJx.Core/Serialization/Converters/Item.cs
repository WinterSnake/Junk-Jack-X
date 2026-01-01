/*
	Junk Jack X: Core
	- [Serialization]Converter - Item

	Written By: Ryan Smith
*/

namespace JJx.Core.Serialization;

internal sealed class ItemConverter : JJxConverter<Item>
{
	/* Instance Methods */
	public override Item Read(ref JJxReader reader) => new() {
		Data=reader.ReadUInt32(),
		Id=reader.ReadUInt16(),
		Count=reader.ReadUInt16(),
		Durability=reader.ReadUInt16(),
		Variant=reader.ReadUInt8(),
		Unknown=reader.ReadUInt8()
	};
	public override void Write(Item @value, JJxWriter writer)
	{
		writer.Write(@value.Data);
		writer.Write(@value.Id);
		writer.Write(@value.Count);
		writer.Write(@value.Durability);
		writer.Write(@value.Variant);
		writer.Write(@value.Unknown);
	}
}
