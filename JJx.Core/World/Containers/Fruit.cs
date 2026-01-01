/*
	Junk Jack X: Core
	- [World]Fruit

	Segment Breakdown:
	-------------------------------------------------------------
	Segment[0x0 : 0x1] = X       | Length: 2 (0x2) | Type: uint16
	Segment[0x2 : 0x3] = Y       | Length: 2 (0x2) | Type: uint16
	-------------------------------------------------------------
	Size: 4 (0x5)

	Written By: Ryan Smith
*/

namespace JJx.Core;

public sealed class Fruit
{
	/* Constructor */
	public Fruit(ushort x, ushort y) :this((x, y)) { }
	public Fruit((ushort, ushort) position) => this.Position = position;
	/* Properties */
	public (ushort X, ushort Y) Position;
}
