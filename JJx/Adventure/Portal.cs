/*
	Junk Jack X: Adventure
	- Portal

	Segment Breakdown:
	---------------------------------------------------------------------------------------
	Segment[0x0 : 0x3] = Origin Planet          | Length: 4 (0x4) | Type: enum flag[uint32]
	Segment[0x4 : 0x7] = Destination Planet     | Length: 4 (0x4) | Type: enum flag[uint32]
	Segment[0x8 : 0x9] = Origin X Position      | Length: 2 (0x2) | Type: uint16
	Segment[0xA : 0xB] = Origin Y Position      | Length: 2 (0x2) | Type: uint16
	Segment[0xC : 0xD] = Destination X Position | Length: 2 (0x2) | Type: uint16
	Segment[0xE : 0xF] = Destination Y Position | Length: 2 (0x2) | Type: uint16
	---------------------------------------------------------------------------------------
	Size: 16 (0x10)

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Portal
{
	/* Constructors */
	public Portal(Planet originPlanet, (ushort, ushort) originPosition, Planet destinationPlanet)
		: this(originPlanet, originPosition, destinationPlanet, (0xFF, 0xFF)) {}
	public Portal(
		Planet originPlanet, (ushort, ushort) originPosition,
		Planet destinationPlanet, (ushort, ushort) destinationPosition
	)
	{
		this.OriginPlanet = originPlanet;
		this.OriginPosition = originPosition;
		this.DestinationPlanet = destinationPlanet;
		this.DestinationPosition = destinationPosition;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var workingData = new byte[SIZE];
		// Planet: Origin
		BitConverter.Write(workingData, (uint)this.OriginPlanet, 0);
		// Planet: Destination
		BitConverter.Write(workingData, (uint)this.DestinationPlanet, 4);
		// Position: Origin
		BitConverter.Write(workingData, this.OriginPosition.X,  8);
		BitConverter.Write(workingData, this.OriginPosition.Y, 10);
		// Position: Destination
		BitConverter.Write(workingData, this.DestinationPosition.X, 12);
		BitConverter.Write(workingData, this.DestinationPosition.Y, 14);
		await stream.WriteAsync(workingData, 0, workingData.Length);
	}
	/* Static Methods */
	public static async Task<Portal> FromStream(Stream stream)
	{
		int bytesRead = 0;
		var workingData = new byte[SIZE];
		while (bytesRead < workingData.Length)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, workingData.Length - bytesRead);
		// Planet: Origin
		var originPlanet = (Planet)BitConverter.GetUInt32(workingData, 0);
		// Planet: Destination
		var destinationPlanet = (Planet)BitConverter.GetUInt32(workingData, 4);
		// Position: Origin
		var originPosition = (
			BitConverter.GetUInt16(workingData,  8),
			BitConverter.GetUInt16(workingData, 10)
		);
		// Position: Destination
		var destinationPosition = (
			BitConverter.GetUInt16(workingData, 12),
			BitConverter.GetUInt16(workingData, 14)
		);
		return new Portal(originPlanet, originPosition, destinationPlanet, destinationPosition);
	}
	/* Properties */
	public (ushort X, ushort Y) OriginPosition;
	public (ushort X, ushort Y) DestinationPosition;
	public Planet OriginPlanet;
	public Planet DestinationPlanet;
	/* Class Properties */
	private const byte SIZE = 16;
}
