/*
	Junk Jack X: Core
	- [World]Forge

	Segment Breakdown:
	---------------------------------------------------------------
	Segment[0x0 : 0x1] = X        | Length:  2 (0x2) | Type: uint16
	Segment[0x2 : 0x3] = Y        | Length:  2 (0x2) | Type: uint16
	Segment[0x4 : 0xF] = UNKNOWN  | Length: 12 (0x2) | Type: ???
	---------------------------------------------------------------
	Size: 16 (0x10)

	Written By: Ryan Smith
*/

using System;

namespace JJx.Core;

public sealed class Forge
{
	/* Constructor */
	public Forge(ushort x, ushort y) :this((x, y)) { }
	public Forge((ushort, ushort) position) => this.Position = position;
	/* Properties */
	public (ushort X, ushort Y) Position;
	private readonly byte[] _Unknown = new byte[SIZEOF_UNKNOWN];
	public Span<byte> Unknown => this._Unknown.AsSpan();
	private readonly Item[] _Items = new Item[COUNTOF_ITEMS];
	public Span<Item> Items => this._Items.AsSpan();
	/* Class Properties */
	private const int SIZEOF_UNKNOWN = 12;
	private const int COUNTOF_ITEMS  =  3;
}
