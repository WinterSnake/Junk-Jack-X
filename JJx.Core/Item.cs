/*
	Junk Jack X: Core
	- Item

	Segment Breakdown:
	----------------------------------------------------------------
	Segment[0x0 : 0x3] = Data       | Length: 4 (0x4) | Type: uint32
	Segment[0x4 : 0x5] = Id         | Length: 2 (0x2) | Type: uint16
	Segment[0x6 : 0x7] = Count      | Length: 2 (0x2) | Type: uint16
	Segment[0x8 : 0x9] = Durabiltiy | Length: 2 (0x2) | Type: uint16
	Segment[0xA]       = Icon       | Length: 1 (0x1) | Type: uint8
	Segment[0xB]       = UNKNOWN    | Length: 1 (0x1) | Type: uint8
	----------------------------------------------------------------
	Length: 12 (0xC)

	Written By: Ryan Smith
*/

namespace JJx.Core;

public record struct Item
{
	/* Constructor */
	public Item(ushort id, ushort count, ushort durability, uint data, byte variant, byte unknown)
	{
		this.Id = id;
		this.Count = count;
		this.Durability = durability;
		this.Data = data;
		this.Variant = variant;
		this.Unknown = unknown;
	}
	/* Properties */
	public ushort Id;
	public ushort Count;
	public ushort Durability;
	public uint Data;
	public byte Variant;
	public readonly byte Unknown;
}
