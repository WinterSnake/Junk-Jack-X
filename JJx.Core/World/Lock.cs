/*
	Junk Jack X: Core
	- [World]Lock

	Segment Breakdown:
	-------------------------------------------------------------
	Segment[0x0 : 0x1] = X       | Length: 2 (0x2) | Type: uint16
	Segment[0x2 : 0x3] = Y       | Length: 2 (0x2) | Type: uint16
	Segment[0x4]       = Radius  | Length: 1 (0x1) | Type: uint8
	-------------------------------------------------------------
	Size: 5 (0x5)

	Written By: Ryan Smith
*/

namespace JJx.Core;

public sealed class Lock
{
	/* Constructor */
	public Lock(ushort x, ushort y, byte radius) :this((x, y), radius) { }
	public Lock((ushort, ushort) position, byte radius)
	{
		this.Position = position;
		this.Radius = radius;
	}
	/* Properties */
	public (ushort X, ushort Y) Position;
	public byte Radius;
}
