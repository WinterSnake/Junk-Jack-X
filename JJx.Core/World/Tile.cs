/*
	Junk Jack X: Core
	- [World]Tile

	Segment Breakdown:
	-----------------------------------------------------------------------
	Segment[0x0 : 0x1] = Foreground Id  | Length: 2 (0x2) | Type: uint16
	Segment[0x2 : 0x3] = Background Id  | Length: 2 (0x2) | Type: uint16
	Segment[0x4 : 0xB] = Decoration Ids | Length: 8 (0x8) | Type: uint16[4]
	Segment[0xC : 0xF] = UNKNOWN        | Length: 4 (0x4) | Type: ???
	-----------------------------------------------------------------------
	Size: 16 (0x10)

	Written By: Ryan Smith
*/

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace JJx.Core;

public partial record struct Tile
{
	/* Constructor */
	internal Tile(ushort foreground, ushort background, ReadOnlySpan<ushort> decorations)
	{
		this.Foreground = foreground;
		this.Background = background;
		decorations.CopyTo(this._Decorations);
	}
	/* Properties */
	public ushort Foreground;
	public ushort Background;
	private DecorationArray _Decorations;
	[UnscopedRef]
	public Span<ushort> Decorations => this._Decorations;
	private readonly byte[] _Unknown = new byte[SIZEOF_UNKNOWN];
	public Span<byte> Unknown => this._Unknown.AsSpan();
	/* Class Properties */
	private const int SIZEOF_UNKNOWN = 4;
	/* Sub-Classes */
	[InlineArray(4)]
	private struct DecorationArray { private ushort _Decoration; }
}
