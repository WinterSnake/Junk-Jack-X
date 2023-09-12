/*
	Junk Jack X: World
	- Lock

	Segment Breakdown:
	-----------------------------------------------------------------------
	Segment[0x0 : 0x1] = X Position | Length: 2  (0x02) | Type: uint16
	Segment[0x2 : 0x3] = Y Position | Length: 2  (0x02) | Type: uint16
	Segment[0x4]       = Type       | Length: 1  (0x01) | Type: enum[uint8]
	-----------------------------------------------------------------------

	Written By: Ryan Smith
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public enum LockType : byte
{
	Wooden   = 0x0A,
	Iron     = 0x14,
	Gold     = 0x28,
	Titanium = 0x50
}

public sealed class Lock
{
	/* Constructors */
	public Lock(LockType type, (ushort, ushort) position)
	{
		this.Type = type;
		this.Position = position;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var workingData = new byte[SIZE];
		// Position
		Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Position.X, 0);
		Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Position.Y, 2);
		// Type
		Utilities.ByteConverter.Write(new Span<byte>(workingData), (byte)this.Type, 4);
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
		// Type
		var type = (LockType)Utilities.ByteConverter.GetUInt8(new Span<byte>(workingData), 4);
		return new Lock(type, position);
	}
	/* Properties */
	public LockType Type;
	public (ushort X, ushort Y) Position;
	/* Class Properties */
	private const byte SIZE = 5;
}
