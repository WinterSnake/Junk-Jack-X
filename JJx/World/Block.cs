/*
	Junk Jack X: World
	- Block

	Segment Breakdown:
	------------------------------------------------------------------------------------------------------------------------
	Segment[0x0 : 0x10] = UNKNOWN FOR NOW | Length: 16  (0x10) | Type: ???
	------------------------------------------------------------------------------------------------------------------------
	Length: 16 (0x10)

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Block
{
	/* Constructors */
	public Block(byte[] data)
	{
		this.Data = data;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		await stream.WriteAsync(this.Data, 0, SIZE);
	}
	/* Static Methods */
	public static async Task<Block> FromStream(Stream stream)
	{
		int bytesRead = 0;
		var workingData = new byte[SIZE];
		while (bytesRead < SIZE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZE - bytesRead);
		//----Unknown----\\
		return new Block(workingData);
	}
	/* Properties */
	public readonly byte[] Data;
	/* Class Properties */
	private const byte SIZE = 16;
}
