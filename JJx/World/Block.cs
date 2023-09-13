/*
	Junk Jack X: World
	- Block

	Segment Breakdown:
	----------------------------------------------------------------------
	Segment[0x0 : 0x1] = Foreground Block | Length: 2 (0x2) | Type: uint16
	Segment[0x2 : 0x3] = Background Block | Length: 2 (0x2) | Type: uint16
	----------------------------------------------------------------------
	Size: 16 (0x10)

	Written By: Ryan Smith
*/
using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Block
{
	/* Constructors */
	private Block(byte[] buffer)
	{
		this._Block = buffer;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		await stream.WriteAsync(this._Block, 0, this._Block.Length);
	}
	/* Static Methods */
	public static async Task<Block> FromStream(Stream stream)
	{
		var bytesRead = 0;
		var workingData = new byte[SIZE];
		while (bytesRead < SIZE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZE - bytesRead);
		var foregroundId = Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData), 0);
		var backgroundId = Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData), 2);
		return new Block(workingData);
	}
	/* Properties */
	public ushort ForegroundId;
	public ushort BackgroundId;
	public byte[] _Block;
	/* Class Properties */
	internal const byte SIZE = 16;
}
