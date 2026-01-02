/*
	Junk Jack X: Core
	- [Serialization]Converter - Lock

	Written By: Ryan Smith
*/

namespace JJx.Core.Serialization;

internal sealed class LockConverter : JJxConverter<Lock>
{
	/* Instance Methods */
	public override Lock Read(ref JJxReader reader) => new(
		reader.ReadUInt16(), reader.ReadUInt16(),
		reader.ReadUInt8()
	);
	public override void Write(in Lock @value, JJxWriter writer)
	{
		writer.Write(@value.Position.X);
		writer.Write(@value.Position.Y);
		writer.Write(@value.Radius);
	}
}
