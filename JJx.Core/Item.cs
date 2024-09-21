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
using JJx.Serialization;

namespace JJx;

[JJxObject(0xC)]
public sealed class Item
{
	/* Constructor */
	public Item(ushort id, ushort count, ushort durability = 0, uint data = 0, byte icon = 0)
	{
		this.Id = id;
		this.Count = count;
		this.Durability = durability;
		this.Data = data;
		this.Icon = icon;
	}
	/* Properties */
	[JJxData(1)]
	public ushort Id;
	[JJxData(2)]
	public ushort Count;
	[JJxData(3)]
	public ushort Durability;
	[JJxData(0)]
	public uint Data;
	[JJxData(4)]
	public byte Icon;
}
