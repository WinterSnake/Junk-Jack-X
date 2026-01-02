/*
	Junk Jack X: Core
	- [World]Stable

	Segment Breakdown:
	------------------------------------------------------------------
	Segment[0x00 : 0x01] = X        | Length:  2 (0x02) | Type: uint16
	Segment[0x02 : 0x03] = Y        | Length:  2 (0x02) | Type: uint16
	Segment[0x04 : 0x17] = UNKNOWN  | Length: 20 (0x14) | Type: ???
	------------------------------------------------------------------
	Size: 24 (0x18)

	Written By: Ryan Smith
*/

using System;

namespace JJx.Core;

public sealed class Stable
{
	/* Constructor */
	public Stable(ushort x, ushort y) :this((x, y)) { }
	public Stable((ushort, ushort) position) => this.Position = position;
	/* Properties */
	public (ushort X, ushort Y) Position;
	private readonly byte[] _Unknown = new byte[SIZEOF_UNKNOWN];
	public Span<byte> Unknown => this._Unknown.AsSpan();
	private Item _Feed;
	public ref Item Feed => ref this._Feed;
	/* Class Properties */
	private const int SIZEOF_UNKNOWN = 20;
}
