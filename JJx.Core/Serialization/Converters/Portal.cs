/*
	Junk Jack X: Core
	- [Serialization]Converter - Portal

	Written By: Ryan Smith
*/

namespace JJx.Core.Serialization;

internal sealed class PortalConverter : JJxConverter<Portal>
{
	/* Instance Methods */
	public override Portal Read(ref JJxReader reader) => new(
		origin: reader.ReadObject<Planet>(), destination: reader.ReadObject<Planet>(),
		originX: reader.ReadUInt16(), originY: reader.ReadUInt16(),
		destinationX: reader.ReadUInt16(), destinationY: reader.ReadUInt16()
	);
	public override void Write(in Portal @value, JJxWriter writer)
	{
		writer.Write(@value.OriginPlanet);
		writer.Write(@value.DestinationPlanet);
		writer.Write(@value.OriginPosition.X);
		writer.Write(@value.OriginPosition.Y);
		writer.Write(@value.DestinationPosition.X);
		writer.Write(@value.DestinationPosition.Y);
	}
}
