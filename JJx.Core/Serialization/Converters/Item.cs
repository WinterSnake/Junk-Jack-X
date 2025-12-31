/*
	Junk Jack X: Core
	- [Serialization]Converter - Item

	Written By: Ryan Smith
*/

namespace JJx.Core.Serialization;

internal sealed class ItemConverter : JJxConverter<Item>
{
	/* Instance Methods */
	public override Item Read(ref JJxReader reader)
	{
		var data = reader.ReadUInt32();
		var id = reader.ReadUInt16();
		var count = reader.ReadUInt16();
		var durabiltiy = reader.ReadUInt16();
		var variant = reader.ReadUInt8();
		var unknown = reader.ReadUInt8();
		return new(id, count, durabiltiy, data, variant, unknown);
	}
	public override void Write(Item value, JJxWriter writer)
	{
		writer.Write(value.Data);
		writer.Write(value.Id);
		writer.Write(value.Count);
		writer.Write(value.Durability);
		writer.Write(value.Variant);
		writer.Write(value.Unknown);
	}
}
