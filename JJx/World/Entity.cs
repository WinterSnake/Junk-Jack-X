/*
	Junk Jack X: World
	- Entity

	Segment Breakdown:
	--------------------------------
	--------------------------------

	Written By: Ryan Smith
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Entity
{
	/* Constructors */
	public Entity()
	{

	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{

	}
	/* Static Methods */
	public static async Task<Entity> FromStream(Stream stream)
	{
		return new Entity();
	}
	/* Properties */
	/* Class Properties */
}
