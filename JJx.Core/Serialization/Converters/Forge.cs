/*
	Junk Jack X: Core
	- [Serialization]Converter - Forge

	Written By: Ryan Smith
*/

namespace JJx.Core.Serialization;

internal sealed class ForgeConverter : JJxConverter<Forge>
{
	/* Instance Methods */
	public override Forge Read(ref JJxReader reader)
	{
		var forge = new Forge(
			reader.ReadUInt16(), reader.ReadUInt16()
		);
		reader.ReadSpan(forge.Unknown);
		foreach (ref var item in forge.Items)
			item = reader.ReadObject<Item>();
		return forge;
	}
	public override void Write(in Forge @value, JJxWriter writer)
	{
		writer.Write(@value.Position.X);
		writer.Write(@value.Position.Y);
		writer.Write(@value.Unknown);
		foreach (ref var item in @value.Items)
			writer.Write(item);
	}
}
