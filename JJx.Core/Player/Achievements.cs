/*
	Junk Jack X: Core
	- [Player]Achievements

	Segment Breakdown:
	-----------------------------------------------------------
	-----------------------------------------------------------
	Size: 

	Written By: Ryan Smith
*/
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public class Achievements
{
	/* Constructor */
	public Achievements() { }
	private Achievements(byte[] data)
	{
		this.Data = data;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		await stream.WriteAsync(this.Data, 0, this.Data.Length);
	}
	/* Static Methods */
	public static async Task<Achievements> FromStream(Stream stream)
	{
		var bytesRead = 0;
		var buffer = new byte[SIZE];
		while (bytesRead < buffer.Length)
			bytesRead += await stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
		return new Achievements(buffer);
	}
	/* Properties */
	public readonly byte[] Data = new byte[SIZE];
	/* Class Properties */
	private const ushort SIZE = 0x20;
}
