/*
	Junk Jack X: Core
	- [World]Tile

	Segment Breakdown:
	--------------------------------------------------------------------
	Segment[0x0 : 0xF] = UNKNOWN | Length: 16 (0x10) | Type: ???
	--------------------------------------------------------------------
	Size: 16 (0x10)

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Tile
{
	/* Constructors */
	public Tile()
	{

	}
	private Tile(byte[] data)
	{
		this.Data = data;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		await stream.WriteAsync(this.Data, 0, this.Data.Length);
	}
	/* Static Methods */
	public static async Task<Tile> FromStream(Stream stream)
	{
		var bytesRead = 0;
		var buffer = new byte[SIZE];
		while (bytesRead < buffer.Length)
			bytesRead += await stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
		return new Tile(buffer);
	}
	/* Properties */
	public readonly byte[] Data = new byte[SIZE];
	/* Class Properties */
	internal const byte SIZE = 16;
}
