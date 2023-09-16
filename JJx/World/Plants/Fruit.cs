/*
	Junk Jack X: World Plants
	- Fruit

	Segment Breakdown:
	----------------------------------------------------------------
	Segment[0x0 : 0x1] = X Position | Length: 2 (0x2) | Type: uint16
	Segment[0x2 : 0x3] = Y Position | Length: 2 (0x2) | Type: uint16
	----------------------------------------------------------------
	Size: 4 (0x4)

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Fruit
{
	/* Constructors */
	public Fruit((ushort, ushort) position)
	{
		this.Position = position;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var workingData = new byte[SIZE];
		// Position
		Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Position.X, 0);
		Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Position.Y, 2);
		await stream.WriteAsync(workingData, 0, workingData.Length);
	}
	/* Static Methods */
	public static async Task<Fruit> FromStream(Stream stream)
	{
		int bytesRead = 0;
		var workingData = new byte[SIZE];
		while (bytesRead < SIZE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZE - bytesRead);
		// Position
		var position = (
			Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData), 0),
			Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData), 2)
		);
		return new Fruit(position);
	}
	/* Properties */
	public (ushort X, ushort Y) Position;
	/* Class Properties */
	private const byte SIZE        = 4;
}
