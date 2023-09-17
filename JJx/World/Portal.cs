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
	public Portal(
		Planet originPlanet, Planet destinationPlanet, (ushort, ushort)originPosition,
		(ushort, ushort) destinationPlanet = (0xFFFF, 0xFFFF)
	)
	{
		this.OriginPlanet = originPlanet;
		this.DestinationPlanet = destinationPlanet;
		this.OriginPosition = originPosition;
		this.DestinationPosition = (0xFFFF, 0xFFFF);
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var workingData = new byte[SIZE];
		// Origin Planet
		Utilities.ByteConverter.Write(new Span<byte>(workingData), (uint)this.OriginPlanet, 0);
		// Destination Planet
		Utilities.ByteConverter.Write(new Span<byte>(workingData), (uint)this.DestinationPlanet, 4);
		// Origin Position
		Utilities.ByteConverter.Write(new Span<byte>(workingData), this.OriginPosition.X,  8);
		Utilities.ByteConverter.Write(new Span<byte>(workingData), this.OriginPosition.Y, 10);
		// Destination Position
		Utilities.ByteConverter.Write(new Span<byte>(workingData), this.DestinationPosition.X, 12);
		Utilities.ByteConverter.Write(new Span<byte>(workingData), this.DestinationPosition.Y, 14);
		await stream.WriteAsync(workingData, 0, workingData.Length);
	}
	/* Static Methods */
	public static async Task<Portal> FromStream(Stream stream)
	{
		int bytesRead = 0;
		var workingData = new byte[SIZE];
		while (bytesRead < SIZE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZE - bytesRead);
		// Origin Planet
		var originplanet      = (Planet)Utilities.ByteConverter.GetUInt32(new Span<byte>(workingData), 0);
		// Destination Planet
		var destinationplanet = (Planet)Utilities.ByteConverter.GetUInt32(new Span<byte>(workingData), 4);
		// Origin Position
		var originPosition = (
			Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData),  8),
			Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData), 10)
		);
		// Destination Position
		var destinationPosition = (
			Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData), 12),
			Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData), 14)
		);
		return new Portal();
	}
	/* Properties */
	public Planet OriginPlanet;
	public Planet DestinationPlanet;
	public (ushort X, ushort Y) OriginPosition;
	public (ushort X, ushort Y) DestinationPosition;
	/* Class Properties */
	private const byte SIZE = 16;
}
