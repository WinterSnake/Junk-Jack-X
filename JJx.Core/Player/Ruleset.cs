/*
	Junk Jack X: Core
	- [Player]Ruleset

	Written By: Ryan Smith
*/

using System;

namespace JJx.Core;

public enum Difficulty : byte
{
	Peaceful = 0,
	Easy     = 1,
	Normal   = 2,
	Hard     = 3,
	VeryHard = 4,
}

public sealed class Ruleset
{
	/* Constructor */
	public Ruleset(Difficulty difficulty, GameplayOptions flags)
	{
		this.Difficulty = difficulty;
		this.Flags = flags;
	}
	/* Properties */
	public Difficulty Difficulty;
	public GameplayOptions Flags;
	/* Sub-Classes */
	[Flags]
	public enum GameplayOptions : uint
	{
		None           = 0,
		Hardcore       = 1 << 0,
		SimpleCraft    = 1 << 1,
		ContinuousTime = 1 << 2,
	}
}
