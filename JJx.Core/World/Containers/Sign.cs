/*
	Junk Jack X: Core
	- [World: Containers]Sign

	Segment Breakdown:
	------------------------------------------------------------------
	Segment[0x0 : 0x1] = X Position  | Length:  2 (0x2) | Type: uint16
	Segment[0x2 : 0x3] = Y Position  | Length:  2 (0x2) | Type: uint16
	Segment[0x4 : 0x5] = String Size | Length:  2 (0x2) | Type: uint16
	------------------------------------------------------------------
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
	public Sign(ushort x, ushort y, string text)
	{
		this.Position = (x, y);
		this.Text = text;
	}
	public Sign((ushort, ushort) position, string text)
	{
		this.Position = position;
		this.Text = text;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var buffer = new byte[SIZE + this.Text.Length + 1];
		BitConverter.LittleEndian.Write(this.Position.X, buffer, OFFSET_POSITION);
		BitConverter.LittleEndian.Write(this.Position.Y, buffer, OFFSET_POSITION + sizeof(ushort));
		BitConverter.LittleEndian.Write((ushort)(this.Text.Length + 1), buffer, OFFSET_SIZE);
		if (!String.IsNullOrEmpty(this.Text))
			BitConverter.Write(this.Text, buffer, OFFSET_TEXT);
		await stream.WriteAsync(buffer, 0, buffer.Length);
	}
	/* Static Methods */
	public static async Task<Sign> FromStream(Stream stream)
	{
		var bytesRead = 0;
		var buffer = new byte[SIZE];
		while (bytesRead < buffer.Length)
			bytesRead += await stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
		var position = (
			BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_POSITION),
			BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_POSITION + sizeof(ushort))
		);
		// Text
		bytesRead = 0;
		var textLength = BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_SIZE);
		if (textLength == 1)
		{
			stream.ReadByte();
			return new Sign(position, "");
		}
		buffer = new byte[textLength];
		while (bytesRead < buffer.Length)
			bytesRead += await stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
		var text = BitConverter.GetString(buffer);
		return new Sign(position, text);
	}
	/* Properties */
	public (ushort X, ushort Y) Position;
	public string Text;
	/* Class Properties */
	private const byte SIZE            = 6;
	private const byte OFFSET_POSITION = 0;
	private const byte OFFSET_SIZE     = 4;
	private const byte OFFSET_TEXT     = 6;
}
