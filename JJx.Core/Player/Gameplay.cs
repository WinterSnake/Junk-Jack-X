/*
	Junk Jack X: Core
	- [Player]Gameplay

	Written By: Ryan Smith
*/
using System;

namespace JJx;

public enum Difficulty : byte
{
	Peaceful = 0x0,
	Easy,
	Normal,
	Hard,
	VeryHard
}

public sealed class Gameplay
{
	/* Constructors */
	public Gameplay(Difficulty difficulty, Flag flags)
	{
		this.Difficulty = difficulty;
		this.Flags = flags;
	}
	/* Properties */
	public Flag Flags;
	public Difficulty Difficulty;
	/* Sub-Classes */
	[Flags]
	public enum Flag : uint
	{
		None           = 0x00000000,
		Hardcore       = 0x00000001,
		SimpleCraft    = 0x00000002,
		ContinuousTime = 0x00000004,
	}
}
