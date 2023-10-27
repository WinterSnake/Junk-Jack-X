/*
	Junk Jack X: World
	- Container: Sign

	Segment Breakdown:
	--------------------------------------------------------------------------
	Segment[0x0 :    0x1] = X Position      | Length: 2  (0x02) | Type: uint16
	Segment[0x2 :    0x3] = Y Position      | Length: 2  (0x02) | Type: uint16
	Segment[0x4 :    0x5] = Text Size       | Length: 2  (0x02) | Type: uint16
	Segment[0x6 : {size}] = Text            | Length:    {size} | Type: char*
	--------------------------------------------------------------------------
	Size: 6 (0x6) + char*

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Sign
{
	/* Constructors */
	public Sign((ushort, ushort) position, string text = "")
	{
		this.Position = position;
		this.Text = text;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var workingData = new byte[SIZE + this.Text.Length + 1];
		// Position
		BitConverter.Write(workingData, this.Position.X, 0);
		BitConverter.Write(workingData, this.Position.Y, 2);
		// Text
		BitConverter.Write(workingData, (ushort)(this.Text.Length + 1), 4);
		BitConverter.Write(workingData, this.Text, 6);
		await stream.WriteAsync(workingData, 0, workingData.Length);
	}
	/* Static Methods */
	public static async Task<Sign> FromStream(Stream stream)
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
		// Text: Length
		var textLength = BitConverter.GetUInt16(workingData, 4);
		// Text
		bytesRead = 0;
		workingData = new byte[textLength];
		while (bytesRead < workingData.Length)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, workingData.Length - bytesRead);
		var text = BitConverter.GetString(workingData);
		return new Sign(position, text);
	}
	/* Properties */
	public (ushort X, ushort Y) Position;
	public string Text;
	/* Class Properties */
	private const byte SIZE = 6;
}
