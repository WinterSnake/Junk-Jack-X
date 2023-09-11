/*
	Junk Jack X: Player
	- Item Book

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class ItemBook
{
	/* Constructors */
	public ItemBook()
	{

	}
	/* Instance Methods */
	/* Static Methods */
	internal static async Task<ItemBook> FromStream(Stream stream)
	{
		int bytesRead = 0;
		var workingData = new byte[SIZE];
		while (bytesRead < SIZE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZE - bytesRead);
		return new ItemBook();
	}
	/* Properties */
	/* Class Properties */
	private const byte SIZE = 44;
}
