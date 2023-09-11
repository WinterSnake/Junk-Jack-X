/*
	Junk Jack X: Player
	- Potion Book

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

[Flags]
public enum Potions
/*
	Segment[0x00]
		- 0x00 - 0xFF: Giant
			NOTE: odd only (?)
	Segment[0x01]
		NONE
	Segment[0x02]
		NONE
	Segment[0x03]
		- 0x00 - 0x3F: NONE
		- 0x40 - 0x7F: Tools Expertise
		- 0x80 - 0xBF: Climb/Jump Movement
		- 0xC0 - 0xFF: Climb/Jump Movement | Tools Expertise
	Segment[0x04]
		- 0xFF: Dwarves | Pierce Damage
*/
{
	ClimbJumpMovement,
	Dwarves,
	Giant,
	PierceDamage,
	ToolsExpertise,
}

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
