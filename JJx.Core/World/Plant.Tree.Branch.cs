/*
	Junk Jack X: Core
	- [World]Plant - Tree Branch

	Segment Breakdown:
	--------------------------------------------------------------
	Segment[0x0 : 0x3] = UNKNOWN  | Length: 4 (0x4) | Type: ???
	Segment[0x4 : 0x5] = X        | Length: 2 (0x2) | Type: uint16
	Segment[0x6 : 0x7] = Y        | Length: 2 (0x2) | Type: uint16
	--------------------------------------------------------------
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
		public (ushort X, ushort Y) Position;
		private readonly byte[] _Unknown = new byte[SIZE];
		public Span<byte> Unknown => this._Unknown.AsSpan();
		/* Class Properties */
		private const int SIZE = 4;
	}
}
