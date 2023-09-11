/*
	Junk Jack X: Player
	- Potion Book

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class PotionBook
{
	/* Constructors */
	public PotionBook()
	{

	}
	/* Instance Methods */
	/* Static Methods */
	internal static async Task<PotionBook> FromStream(Stream stream)
	{
		int bytesRead = 0;
		var workingData = new byte[SIZE];
		while (bytesRead < SIZE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZE - bytesRead);
		return new PotionBook();
	}
	/* Properties */
	/* Class Properties */
	private const byte SIZE = 128;
}
