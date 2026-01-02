/*
	Junk Jack X: Core
	- [World]Plant - Tree

	Segment Breakdown[Tree]:
	-------------------------------------------------------------------
	Segment[0x0]       = UNKNOWN       | Length: 1 (0x1) | Type: uint8
	Segment[0x1]       = Stage         | Length: 1 (0x1) | Type: uint8
	Segment[0x2]       = Branch Count  | Length: 1 (0x1) | Type: uint8
	Segment[0x3]       = Growth Step   | Length: 1 (0x1) | Type: uint8
	Segment[0x4]       = UNKNOWN       | Length: 1 (0x1) | Type: uint8
	Segment[0x5]       = UNKNOWN       | Length: 1 (0x1) | Type: uint8
	Segment[0x6]       = UNKNOWN       | Length: 1 (0x1) | Type: uint8
	Segment[0x7]       = UNKNOWN       | Length: 1 (0x1) | Type: uint8
	Segment[0x8 : 0x9] = Height        | Length: 2 (0x2) | Type: uint16
	-------------------------------------------------------------------
	Size: 10 (0xA)

	Written By: Ryan Smith
*/

using System;
using System.Collections.Generic;

public sealed partial class Tree : Plant
{
	/* Constructor */
	public Tree(ushort x, ushort y, uint id) :this((x, y), id) { }
	public Tree((ushort, ushort) position, uint id) : base(position, id) { }
	/* Properties */
	private readonly byte[] _Unknown = new byte[SIZE];
	public Span<byte> Unknown => this._Unknown.AsSpan();
	public readonly List<Branch> Branches = new();
	/* Class Properties */
	private const int SIZE = 10;
}
