/*
	Junk Jack X: Core
	- [World]Plant - Tree Branch

	Segment Breakdown:
	-------------------------------------------------------------
	-------------------------------------------------------------
	Size: 8 (0x8)

	Written By: Ryan Smith
*/

using System;

public sealed partial class Tree : Plant
{
	/* Sub-Classes */
	public sealed class Branch
	{
		/* Properties */
		private readonly byte[] _Unknown = new byte[SIZE];
		public Span<byte> Unknown => this._Unknown.AsSpan();
		/* Class Properties */
		private const int SIZE = 8;
	}
}
