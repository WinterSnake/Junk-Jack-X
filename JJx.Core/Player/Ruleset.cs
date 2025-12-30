/*
	Junk Jack X: Core
	- [Player]Gameplay

	Written By: Ryan Smith
*/

using System;

namespace JJx.Core;

public enum Difficulty : byte
{
	Peaceful = 0,
	Easy,
	Normal,
	Hard,
	VeryHard
}

public sealed record class Ruleset
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
