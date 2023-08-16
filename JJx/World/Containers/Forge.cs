/*
	Junk Jack X: World
	- [Container]Forge

	Segment Breakdown:
	------------------------------------------------------------------------------------------------------------------------
	Segment[0x0 : 0x1] = X Position      | Length: 2  (0x2) | Type: uint16
	Segment[0x2 : 0x3] = Y Position      | Length: 2  (0x2) | Type: uint16
		<Item[3]>
	------------------------------------------------------------------------------------------------------------------------
	Length: 7 (0x7)

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace JJx;

public sealed class ForgeContainer
{
	/* Constructors */
	public ForgeContainer((ushort x, ushort y) position)
	{
		this.Position = position;
		for (var i = 0; i < this.Items.Length; ++i)
			this.Items[i] = new Item(0, 0);
	}
	private ForgeContainer((ushort x, ushort y) position, Item[] items)
	{
		this.Position = position;
		this.Items = items;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{

	}
	/* Static Methods */
	public static async Task<ForgeContainer> FromStream(Stream stream)
	{
		var items = new Item[COUNT_ITEMS];
		return new ForgeContainer((0, 0), items);
	}
	/* Properties */
	public (ushort X, ushort Y) Position;
	public readonly Item[] Items = new Item[COUNT_ITEMS];
	/* Class Properties */
	private const byte SIZE            = 4;
	private const byte SIZEOF_POSITION = 4;
	private const byte COUNT_ITEMS     = 5;
}
