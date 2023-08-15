/*
	Junk Jack X: Player
	- Gameplay Data

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
	public Gameplay(Difficulty difficulty, Flag flags = Flag.None)
	{
		this.Difficulty = difficulty;
		this.Flags = flags;
	}
	internal Gameplay(byte difficulty, byte flags)
	{
		this.Difficulty = (Difficulty)difficulty;
		this.Flags = (Flag)flags;
	}
	/* Properties */
	public Difficulty Difficulty;
	public Flag Flags;
	/* Sub-Classes */
	[Flags]
	public enum Flag : byte
	{
		None = 0x00,
		Hardcore = 0x01,
		SimpleCraft = 0x02,
		ContinuousTime = 0x4
	}
}
