/*
	Junk Jack X: Core
	- [Serialization]Converter - Archiver Chunk

	Written By: Ryan Smith
*/

namespace JJx.Core.Serialization;

internal sealed class ArchiverChunkConverter : JJxConverter<ArchiverChunk>
{
	/* Instance Methods */
	public override ArchiverChunk Read(ref JJxReader reader)
	{
		return new() {
			Type=reader.ReadObject<ArchiverChunkType>(),
			Version=reader.ReadUInt8(),
			IsCompressed=reader.ReadBool(),
			Offset=reader.ReadInt32(),
			Length=reader.ReadInt32(),
		};
	}
	public override void Write(ArchiverChunk @value, JJxWriter writer)
	{
		writer.Write(@value.Type);
		writer.Write(@value.Version);
		writer.Write(@value.IsCompressed);
		writer.Write(@value.Offset);
		writer.Write(@value.Length);
	}
}
