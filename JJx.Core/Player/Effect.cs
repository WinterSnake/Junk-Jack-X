/*
	Junk Jack X: Core
	- [Player]Effect

	Segment Breakdown:
	-----------------------------------------------------------
	Segment[0x0 : 0x1] = Id    | Length: 2 (0x2) | Type: uint16
	Segment[0x2 : 0x3] = Ticks | Length: 2 (0x2) | Type: uint16
	-----------------------------------------------------------
	Size: 4 (0x4)

	Written By: Ryan Smith
*/
using JJx.Serialization;

namespace JJx;

[JJxObject]
public sealed class Effect
{
	/* Constructor */
	public Effect(ushort id, ushort ticks)
	{
		this.Id = id;
		this.Ticks = ticks;
	}
	/* Properties */
	[JJxData(0)]
	public ushort Id;
	[JJxData(1)]
	public ushort Ticks;
}
