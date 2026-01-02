/*
	Junk Jack X: Core
	- [Serialization]Converter - Entity

	Written By: Ryan Smith
*/

namespace JJx.Core.Serialization;

internal sealed class EntityConverter : JJxConverter<Entity>
{
	/* Instance Methods */
	public override Entity Read(ref JJxReader reader) => new(
		reader.ReadUInt16(), reader.ReadUInt16(),
		reader.ReadUInt8(),
		reader.ReadUInt16()
	);
	public override void Write(in Entity @value, JJxWriter writer)
	{
		writer.Write(@value.Position.X);
		writer.Write(@value.Position.Y);
		writer.Write(@value.Unknown);
		writer.Write(@value.Id);
	}
}
