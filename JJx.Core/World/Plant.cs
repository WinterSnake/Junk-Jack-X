/*
	Junk Jack X: Core
	- [World]Plant

	Segment Breakdown:
	-------------------------------------------------------------
	Segment[0x0 : 0x3] = Id      | Length: 4 (0x4) | Type: uint32
	Segment[0x4 : 0x5] = X       | Length: 4 (0x4) | Type: uint16
	Segment[0x6 : 0x7] = Y       | Length: 4 (0x4) | Type: uint16
	Segment[0x8 : 0xB] = IsTree  | Length: 4 (0x4) | Type: uint32
	Segment[0xC : 0xF] = IsCrop  | Length: 4 (0x4) | Type: uint32
	-------------------------------------------------------------
	Size: 16 (0x10)

	Written By: Ryan Smith
*/

public abstract class Plant
{
	/* Constructor */
	public Plant(ushort x, ushort y, uint id) :this((x, y), id) { }
	public Plant((ushort, ushort) position, uint id)
	{
		this.Id = id;
		this.Position = position;
	}
	/* Properties */
	public uint Id;
	public (ushort X, ushort Y) Position;
}
