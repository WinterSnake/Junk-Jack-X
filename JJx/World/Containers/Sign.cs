/*
	Junk Jack X: World Container
	- Sign

	Segment Breakdown:
	--------------------------------------------------------------------------
	Segment[0x0 :    0x1] = X Position      | Length: 2  (0x02) | Type: uint16
	Segment[0x2 :    0x3] = Y Position      | Length: 2  (0x02) | Type: uint16
	Segment[0x4 :    0x5] = Text Size       | Length: 2  (0x02) | Type: uint16
	Segment[0x6 : {size}] = Text            | Length: {size}    | Type: char*
	--------------------------------------------------------------------------
	Size: 6 + {size}

	Written By: Ryan Smith
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Sign
{
	/* Constructors */
	public Sign((ushort, ushort) position): this(position, String.Empty) { }
	public Sign((ushort, ushort) position, string text)
	{
		this.Position = position;
		this.Text = text;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var workingData = new byte[SIZE + this.Text.Length];
		// Position
		Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Position.X, 0);
		Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Position.Y, 2);
		// Text Length
		Utilities.ByteConverter.Write(new Span<byte>(workingData), (ushort)this.Text.Length, 4);
		// Text
		Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Text, 6);
	}
	/* Static Methods */
	public static async Task<Sign> FromStream(Stream stream)
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
		// Text Length
		var size = Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData), 4);
		// Text
		bytesRead = 0;
		workingData = new byte[size];
		while (bytesRead < workingData.Length)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, workingData.Length - bytesRead);
		var text = Utilities.ByteConverter.GetString(new Span<byte>(workingData));
		return new Sign(position, text);
	}
	/* Properties */
	public (ushort X, ushort Y) Position;
	public string Text;
	/* Class Properties */
	private const byte SIZE = 6;
}
