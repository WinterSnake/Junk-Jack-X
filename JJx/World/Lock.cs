/*
	Junk Jack X: World
	- Lock

	Segment Breakdown:
	------------------------------------------------------------------
	Segment[0x0 : 0x1] = X Position | Length: 2  (0x02) | Type: uint16
	Segment[0x2 : 0x3] = Y Position | Length: 2  (0x02) | Type: uint16
	Segment[0x4]       = Radius     | Length: 1  (0x01) | Type: uint8
	------------------------------------------------------------------
	Size: 5 (0x5)

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Lock
{
	/* Constructors */
	public Lock(byte radius, (ushort, ushort) position)
	{
		this.Radius = radius;
		this.Position = position;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var workingData = new byte[SIZE];
		// Position
		Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Position.X, 0);
		Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Position.Y, 2);
		// Radius
		Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Radius, 4);
		await stream.WriteAsync(workingData, 0, workingData.Length);
	}
	/* Static Methods */
	public static async Task<Lock> FromStream(Stream stream)
	{
		var bytesRead = 0;
		var workingData = new byte[SIZE];
		while (bytesRead < SIZE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZE - bytesRead);
		// Position
		var position = (
			Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData), 0),
			Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData), 2)
		);
		// Radius
		var radius = Utilities.ByteConverter.GetUInt8(new Span<byte>(workingData), 4);
		return new Lock(radius, position);
	}
	/* Properties */
	public byte Radius;
	public (ushort X, ushort Y) Position;
	/* Class Properties */
	public const uint RADIUS_WOOD     = 0x0A;
	public const uint RADIUS_IRON     = 0x14;
	public const uint RADIUS_GOLD     = 0x28;
	public const uint RADIUS_TITANIUM = 0x50;
	private const byte SIZE = 5;
}
