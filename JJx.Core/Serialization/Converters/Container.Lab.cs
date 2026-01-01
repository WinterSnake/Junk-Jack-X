/*
	Junk Jack X: Core
	- [Serialization]Converter - Lab

	Written By: Ryan Smith
*/

namespace JJx.Core.Serialization;

internal sealed class LabConverter : JJxConverter<Lab>
{
	/* Instance Methods */
	public override Lab Read(ref JJxReader reader)
	{
		var shelf = new Lab(
			reader.ReadUInt16(), reader.ReadUInt16()
		);
		foreach (ref var item in shelf.Items)
			item = reader.ReadObject<Item>();
		return shelf;
	}
	public override void Write(in Lab @value, JJxWriter writer)
	{
		writer.Write(@value.Position.X);
		writer.Write(@value.Position.Y);
		foreach (ref var item in @value.Items)
			writer.Write(item);
	}
}
