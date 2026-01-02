/*
	Junk Jack X: Core
	- [World]Portal

	Segment Breakdown:
	-------------------------------------------------------------------------
	Segment[0x0 : 0x3] = Origin Planet       | Length: 4 (0x4) | Type: uint32
	Segment[0x4 : 0x7] = Destination Planet  | Length: 4 (0x4) | Type: uint32
	Segment[0x8 : 0x9] = Origin.X            | Length: 2 (0x2) | Type: uint16
	Segment[0xA : 0xB] = Origin.Y            | Length: 2 (0x2) | Type: uint16
	Segment[0xC : 0xD] = Destination.X       | Length: 2 (0x2) | Type: uint16
	Segment[0xE : 0xF] = Destination.Y       | Length: 2 (0x2) | Type: uint16
	-------------------------------------------------------------------------
	Size: 16 (0x10)

	Written By: Ryan Smith
*/

namespace JJx.Core;

public sealed class Portal
{
	/* Constructor */
	public Portal(Planet origin, ushort originX, ushort originY, Planet destination, ushort destinationX, ushort destinationY)
		:this(origin, (originX, originY), destination, (destinationX, destinationY)) { }
	public Portal(Planet origin, (ushort, ushort) originPosition, Planet destination, (ushort, ushort) destinationPosition)
	{
		this.OriginPlanet = origin;
		this.OriginPosition = originPosition;
		this.DestinationPlanet = destination;
		this.DestinationPosition = destinationPosition;
	}
	/* Properties */
	public Planet OriginPlanet;
	public (ushort X, ushort Y) OriginPosition;
	public Planet DestinationPlanet;
	public (ushort X, ushort Y) DestinationPosition;
}
