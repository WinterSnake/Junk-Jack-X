/*
	Junk Jack X: Core
	- [World]Entity

	Segment Breakdown:
	--------------------------------------------------------------
	Segment[0x0 : 0x1] = X        | Length: 2 (0x2) | Type: uint16
	Segment[0x2 : 0x3] = Y        | Length: 2 (0x2) | Type: uint16
	Segment[0x4]       = UNKNOWN  | Length: 1 (0x1) | Type: uint8
	Segment[0x5 : 0x6] = Id       | Length: 2 (0x2) | Type: uint16
	--------------------------------------------------------------
	Size: 7 (0x7)

	Written By: Ryan Smith
*/

namespace JJx.Core;

public sealed class Entity
{
	/* Constructor */
	public Entity(ushort x, ushort y, byte unknown, ushort id) :this((x, y), unknown, id) { }
	public Entity((ushort, ushort) position, byte unknown, ushort id)
	{
		this.Id = id;
		this.Position = position;
		this.Unknown = unknown;
	}
	/* Properties */
	public ushort Id;
	public (ushort X, ushort Y) Position;
	public byte Unknown;
}
