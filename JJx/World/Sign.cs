/*
	Junk Jack X: World
	- Sign

	Segment Breakdown:
	------------------------------------------------------------------------------------------------------------------------
	Segment[0x0 : 0x1] = X Position      | Length: 2  (0x02) | Type: uint16
	Segment[0x2 : 0x3] = Y Position      | Length: 2  (0x02) | Type: uint16
	Segment[0x4 : 0x5] = Text Size       | Length: 2  (0x02) | Type: uint16
	------------------------------------------------------------------------------------------------------------------------

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace JJx;

public sealed class Sign
{
	/* Contructors */
	public Sign((ushort x, ushort y) position)
	{
		this.Position = position;
	}
	private Sign((ushort x, ushort y) position, string text)
	{
		this.Position = position;
		this._Text = text;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
		// TODO: Use full buffer before WriteAynsc
		// TODO: Ensure BitConverter.GetBytes<T> forces little endian
	{
		byte[] bytes;
		var workingData = new byte[BUFFER_SIZE];
		// Position
		// -X
		bytes = BitConverter.GetBytes(this.Position.X);
		Array.Copy(bytes, 0, workingData, 0, bytes.Length);
		// -Y
		bytes = BitConverter.GetBytes(this.Position.Y);
		Array.Copy(bytes, 0, workingData, 2, bytes.Length);
		await stream.WriteAsync(workingData, 0, SIZEOF_POSITION);
		// -Text Length
		var textLength = this._Text.Length;
		bytes = BitConverter.GetBytes(textLength);
		Array.Copy(bytes, 0, workingData, 0, bytes.Length);
		await stream.WriteAsync(workingData, 0, SIZEOF_TEXTLENGTH);
		// Text
		var byteCount = Encoding.ASCII.GetBytes(this._Text, 0, this._Text.Length, workingData, 0);
		workingData[byteCount] = 0;
		await stream.WriteAsync(workingData, 0, textLength + 1);
	}
	/* Static Methods */
	public static async Task<Sign> FromStream(Stream stream)
	{
		var bytesRead = 0;
		var workingData = new byte[BUFFER_SIZE];
		// Position
		while (bytesRead < SIZEOF_POSITION)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_POSITION - bytesRead);
		var position = (
			BitConverter.ToUInt16(new Span<byte>(workingData).Slice(0, 2)),
			BitConverter.ToUInt16(new Span<byte>(workingData).Slice(2, 2))
		);
		// -Text Length
		bytesRead = 0;
		while (bytesRead < SIZEOF_TEXTLENGTH)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TEXTLENGTH - bytesRead);
		var textLength = BitConverter.ToUInt16(new Span<byte>(workingData).Slice(0, 2));
		// Text
		bytesRead = 0;
		while (bytesRead < textLength)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, textLength - bytesRead);
		var text = Encoding.ASCII.GetString(
			new Span<byte>(workingData).Slice(0, Array.IndexOf(workingData, byte.MinValue))
		);
		return new Sign(position, text);
	}
	/* Properties */
	public (ushort X, ushort Y) Position;
	private string _Text = "";
	public string Text {
		get { return this._Text; }
		set {
			if (value.Length < SIZEOF_TEXT) this._Text = value;
			else this._Text = value.Substring(0, SIZEOF_TEXT - 1);
		}
	}
	/* Class Properties */
	public  const byte MAXLENGTH_TEXT    = SIZEOF_TEXT - 1;
	private const byte BUFFER_SIZE       = 74;
	private const byte SIZEOF_POSITION   = 4;
	private const byte SIZEOF_TEXTLENGTH = 2;
	private const byte SIZEOF_TEXT       = 74;
}
