/*
	Junk Jack X: Core
	- [Serialization]Converter - Chest

	Written By: Ryan Smith
*/

using System.Runtime.InteropServices;

namespace JJx.Core.Serialization;

internal sealed class ChestConverter : JJxConverter<Chest>
{
	/* Instance Methods */
	public override Chest Read(ref JJxReader reader)
	{
		var chest = new Chest(
			reader.ReadUInt32(), reader.ReadUInt32(),
			reader.ReadInt32()
		);
		CollectionsMarshal.SetCount(chest._Items, chest._Items.Capacity);
		foreach (ref var slot in chest.Items)
			slot = reader.ReadObject<Item>();
		return chest;
	}
	public override void Write(Chest @value, JJxWriter writer)
	{
		writer.Write(@value.Position.X);
		writer.Write(@value.Position.Y);
		writer.Write(@value.Capacity);
		foreach (ref var slot in @value.Items)
			writer.Write(slot);
	}
}
