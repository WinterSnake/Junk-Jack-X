/*
	Junk Jack X: World
	- Block

	Segment Breakdown:
	------------------------------------------------------------------
	------------------------------------------------------------------
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
		return new Block(workingData);
	}
	/* Properties */
	public byte[] _Block;
	/* Class Properties */
	internal const byte SIZE = 16;
}
