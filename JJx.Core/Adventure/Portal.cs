/*
	Junk Jack X: Core
	- [Adventure]Portal

	Segment Breakdown:
	---------------------------------------------------------------------------------------------------
	Segment[0x0 : 0x3] = Origin Planet          | Length: 4 (0x4) | Type: enum[uint32] | Parent: Planet
	Segment[0x4 : 0x7] = Destination Planet     | Length: 4 (0x4) | Type: enum[uint32] | Parent: Planet
	Segment[0x8 : 0x9] = Origin X Position      | Length: 2 (0x2) | Type: uint16
	Segment[0xA : 0xB] = Origin Y Position      | Length: 2 (0x2) | Type: uint16
	Segment[0xC : 0xD] = Destination X Position | Length: 2 (0x2) | Type: uint16
	Segment[0xE : 0xF] = Destination Y Position | Length: 2 (0x2) | Type: uint16
	---------------------------------------------------------------------------------------------------
	Size: 16 (0x10)

	Written By: Ryan Smith
*/
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Portal
{
	/* Constructors */
	public Portal(ushort originX, ushort originY, Planet originPlanet, ushort destinationX, ushort destinationY, Planet destinationPlanet)
	{
		this.OriginPosition = (originX, originY);
		this.OriginPlanet = originPlanet;
		this.DestinationPosition = (destinationX, destinationY);
		this.DestinationPlanet = destinationPlanet;
	}
	public Portal((ushort, ushort) originPosition, Planet originPlanet, (ushort, ushort) destinationPosition, Planet destinationPlanet)
	{
		this.OriginPosition = originPosition;
		this.OriginPlanet = originPlanet;
		this.DestinationPosition = destinationPosition;
		this.DestinationPlanet = destinationPlanet;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var buffer = new byte[SIZE];
		BitConverter.LittleEndian.Write((uint)this.OriginPlanet, buffer, OFFSET_ORIGIN_PLANET);
		BitConverter.LittleEndian.Write((uint)this.DestinationPlanet, buffer, OFFSET_DESTINATION_PLANET);
		BitConverter.LittleEndian.Write(this.OriginPosition.X, buffer, OFFSET_ORIGIN_POSITION);
		BitConverter.LittleEndian.Write(this.OriginPosition.Y, buffer, OFFSET_ORIGIN_POSITION + sizeof(ushort));
		BitConverter.LittleEndian.Write(this.DestinationPosition.X, buffer, OFFSET_DESTINATION_POSITION);
		BitConverter.LittleEndian.Write(this.DestinationPosition.Y, buffer, OFFSET_DESTINATION_POSITION + sizeof(ushort));
		await stream.WriteAsync(buffer, 0, buffer.Length);
	}
	/* Static Methods */
	public static async Task<Portal> FromStream(Stream stream)
	{
		var bytesRead = 0;
		var buffer = new byte[SIZE];
		while (bytesRead < buffer.Length)
			bytesRead += await stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
		var originPlanet      = (Planet)BitConverter.LittleEndian.GetUInt32(buffer, OFFSET_ORIGIN_PLANET);
		var destinationPlanet = (Planet)BitConverter.LittleEndian.GetUInt32(buffer, OFFSET_DESTINATION_PLANET);
		var originPosition = (
			BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_ORIGIN_POSITION),
			BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_ORIGIN_POSITION + sizeof(ushort))
		);
		var destinationPosition = (
			BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_DESTINATION_POSITION),
			BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_DESTINATION_POSITION + sizeof(ushort))
		);
		return new Portal(originPosition, originPlanet, destinationPosition, destinationPlanet);
	}
	/* Properties */
	public (ushort X, ushort Y) OriginPosition;
	public Planet OriginPlanet;
	public (ushort X, ushort Y) DestinationPosition;
	public Planet DestinationPlanet;
	/* Class Properties */
	private const byte SIZE                        = 16;
	private const byte OFFSET_ORIGIN_PLANET        =  0;
	private const byte OFFSET_DESTINATION_PLANET   =  4;
	private const byte OFFSET_ORIGIN_POSITION      =  8;
	private const byte OFFSET_DESTINATION_POSITION = 12;
}
