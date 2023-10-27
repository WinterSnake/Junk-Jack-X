/*
	Junk Jack X: World
	- Lock

	Segment Breakdown:
	----------------------------------------------------------------
	Segment[0x0 : 0x1] = X Position | Length: 2 (0x2) | Type: uint16
	Segment[0x2 : 0x3] = Y Position | Length: 2 (0x2) | Type: uint16
	Segment[0x4]       = Radius     | Length: 1 (0x1) | Type: uint8
	----------------------------------------------------------------

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Lock
{
	/* Constructors */
	public Lock((ushort, ushort) position, byte radius)
	{
		this.Position = position;
		this.Radius = radius;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var workingData = new byte[SIZE];
		// Position
		BitConverter.Write(workingData, this.Position.X, 0);
		BitConverter.Write(workingData, this.Position.Y, 2);
		// Radius
		workingData[4] = this.Radius;
		await stream.WriteAsync(workingData, 0, workingData.Length);
	}
	/* Static Methods */
	public static async Task<Lock> FromStream(Stream stream)
	{
		int bytesRead = 0;
		var workingData = new byte[SIZE];
		while (bytesRead < workingData.Length)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, workingData.Length - bytesRead);
		// Position
		var position = (
			BitConverter.GetUInt16(workingData, 0),
			BitConverter.GetUInt16(workingData, 2)
		);
		// Radius
		var radius = workingData[4];
		return new Lock(position, radius);
	}
	/* Properties */
	public (ushort X, ushort Y) Position;
	public byte Radius;
	/* Class Properties */
	public const uint RADIUS_WOOD     = 0x0A;
	public const uint RADIUS_IRON     = 0x14;
	public const uint RADIUS_GOLD     = 0x28;
	public const uint RADIUS_TITANIUM = 0x50;
	private const byte SIZE = 5;
}
