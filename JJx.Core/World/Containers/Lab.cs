/*
	Junk Jack X: Core
	- [World]Lab

	Segment Breakdown:
	--------------------------------------------------------
	Segment[0x0 : 0x1] = X  | Length: 2 (0x2) | Type: uint16
	Segment[0x2 : 0x3] = Y  | Length: 2 (0x2) | Type: uint16
	--------------------------------------------------------
	Size: 4 (0x4)

	Written By: Ryan Smith
*/

using System;

namespace JJx.Core;

public sealed class Lab
{
	/* Constructor */
	public Lab(ushort x, ushort y) :this((x, y)) { }
	public Lab((ushort, ushort) position) => this.Position = position;
	/* Properties */
	public (ushort X, ushort Y) Position;
	private readonly Item[] _Items = new Item[COUNTOF_ITEMS];
	public Span<Item> Items => this._Items.AsSpan();
	/* Class Properties */
	private const int COUNTOF_ITEMS = 5;
}
