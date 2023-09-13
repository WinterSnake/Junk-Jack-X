/*
	Junk Jack X: World
	- Plant

	Segment Breakdown:
	-----------------------------------------------------------------------
	-----------------------------------------------------------------------

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Plant
{
	/* Constructors */
	public Plant()
	{
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
	}
	/* Static Methods */
	public static async Task<Plant> FromStream(Stream stream)
	{
		return new Plant();
	}
	/* Properties */
	/* Class Properties */
}
