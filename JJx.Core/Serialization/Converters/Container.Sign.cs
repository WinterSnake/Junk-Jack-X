/*
	Junk Jack X: Core
	- [Serialization]Converter - Sign

	Written By: Ryan Smith
*/

namespace JJx.Core.Serialization;

internal sealed class SignConverter : JJxConverter<Sign>
{
	/* Instance Methods */
	public override Sign Read(ref JJxReader reader) => new(
		reader.ReadUInt16(), reader.ReadUInt16(),
		reader.ReadString()
	);
	public override void Write(in Sign @value, JJxWriter writer)
	{
		writer.Write(@value.Position.X);
		writer.Write(@value.Position.Y);
		writer.Write(@value.Text, 0);
	}
}
