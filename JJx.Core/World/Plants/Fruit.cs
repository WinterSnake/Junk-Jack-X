/*
	Junk Jack X: Core
	- [World: Plants]Fruit

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
	public Fruit(ushort x, ushort y) { this.Position = (x, y); }
	public Fruit((ushort, ushort) position) { this.Position = position; }
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var buffer = new byte[SIZE];
		BitConverter.LittleEndian.Write(this.Position.X, buffer, OFFSET_POSITION);
		BitConverter.LittleEndian.Write(this.Position.Y, buffer, OFFSET_POSITION + sizeof(ushort));
		await stream.WriteAsync(buffer, 0, buffer.Length);
	}
	/* Static Methods */
	public static async Task<Fruit> FromStream(Stream stream)
	{
		var bytesRead = 0;
		var buffer = new byte[SIZE];
		while (bytesRead < buffer.Length)
			bytesRead += await stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
		// Position
		var position = (
			BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_POSITION),
			BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_POSITION + sizeof(ushort))
		);
		return new Fruit(position);
	}
	/* Properties */
	public (ushort X, ushort Y) Position;
	/* Class Properties */
	private const byte SIZE            = 4;
	private const byte OFFSET_POSITION = 0;
}
