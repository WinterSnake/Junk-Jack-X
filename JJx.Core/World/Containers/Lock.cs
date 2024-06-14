/*
	Junk Jack X: Core
	- [World: Containers]Lock

	Segment Breakdown:
	----------------------------------------------------------------
	Segment[0x0 : 0x1] = X Position | Length: 2 (0x2) | Type: uint16
	Segment[0x2 : 0x3] = Y Position | Length: 2 (0x2) | Type: uint16
	Segment[0x4]       = Radius     | Length: 1 (0x1) | Type: uint8
	----------------------------------------------------------------
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
	public Lock(ushort x, ushort y, byte radius)
	{
		this.Position = (x, y);
		this.Radius = radius;
	}
	public Lock((ushort, ushort) position, byte radius)
	{
		this.Position = position;
		this.Radius = radius;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var buffer = new byte[SIZE];
		BitConverter.LittleEndian.Write(this.Position.X, buffer, OFFSET_POSITION);
		BitConverter.LittleEndian.Write(this.Position.Y, buffer, OFFSET_POSITION + sizeof(ushort));
		buffer[OFFSET_RADIUS] = this.Radius;
		await stream.WriteAsync(buffer, 0, buffer.Length);
	}
	/* Static Methods */
	public static async Task<Lock> FromStream(Stream stream)
	{
		var bytesRead = 0;
		var buffer = new byte[SIZE];
		while (bytesRead < buffer.Length)
			bytesRead += await stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
		var position = (
			BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_POSITION),
			BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_POSITION + sizeof(ushort))
		);
		var radius = buffer[OFFSET_RADIUS];
		return new Lock(position, radius);
	}
	/* Properties */
	public (ushort X, ushort Y) Position;
	public byte Radius;
	/* Class Properties */
	private const byte SIZE            = 5;
	private const byte OFFSET_POSITION = 0;
	private const byte OFFSET_RADIUS   = 4;
}
