/*
	Junk Jack X: Containers
	- Chest, Forge, Lab, Sign, Stable

*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Chest
/*
	Segment Breakdown:
	--------------------------------
	Segment[0x0 : 0x3] = X Position | Length: 4  (0x4) | Type: uint32
	Segment[0x4 : 0x7] = Y Position | Length: 4  (0x4) | Type: uint32
	Segment[0x8 : 0xB] = Item Count | Length: 4  (0x4) | Type: uint32
	--------------------------------
*/
{
	/* Constructors */
	public Chest((uint, uint) position, uint itemCount)
	{
		this.Position = position;
		for (var i = 0; i < itemCount; ++i)
			this.Items.Add(new Item(0xFFFF, 0));
	}
	public Chest((uint, uint) position, Item[] items)
	{
		this.Position = position;
		this.Items.AddRange(items);
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{

	}
	/* Static Methods */
	public static async Task<Chest> FromStream(Stream stream)
	{
		var bytesRead = 0;
		var workingData = new byte[SIZE];
		while (bytesRead < SIZE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZE - bytesRead);
		// Position
		var position = (
			BitConverter.ToUInt32(new Span<byte>(workingData).Slice(0, 4)),
			BitConverter.ToUInt32(new Span<byte>(workingData).Slice(4, 4))
		);
		// Item Count
		var size = BitConverter.ToUInt32(new Span<byte>(workingData).Slice(8, 4));
		// Items
		var items = new Item[size];
		for (var i = 0; i < size; ++i)
			items[i] = await Item.FromStream(stream);
		return new Chest(position, items);
	}
	/* Properties */
	public (uint X, uint Y) Position;
	public readonly List<Item> Items = new List<Item>();
	/* Class Properties */
	public  const uint ITEMCOUNT_WOOD       = 12;
	public  const uint ITEMCOUNT_COPPER     = 24;
	public  const uint ITEMCOUNT_IRON       = 24;
	public  const uint ITEMCOUNT_SILVER     = 36;
	public  const uint ITEMCOUNT_GOLD       = 36;
	public  const uint ITEMCOUNT_MITHRIL    = 48;
	public  const uint ITEMCOUNT_ANTANIUM   = 60;
	public  const uint ITEMCOUNT_GALVANIUM  = 72;
	public  const uint ITEMCOUNT_TITANIUM   = 72;
	public  const uint ITEMCOUNT_BOTTOMLESS = 120;
	private const byte SIZE = 12;
}

public sealed class Forge
/*
	Segment Breakdown:
	--------------------------------
	--------------------------------
*/
{
	/* Constructors */
	/* Instance Methods */
	/* Static Methods */
	/* Properties */
	/* Class Properties */
}

public sealed class Lab
/*
	Segment Breakdown:
	--------------------------------
	--------------------------------
*/
{
	/* Constructors */
	/* Instance Methods */
	/* Static Methods */
	/* Properties */
	/* Class Properties */
}

public sealed class Sign
/*
	Segment Breakdown:
	--------------------------------
	--------------------------------
*/
{
	/* Constructors */
	/* Instance Methods */
	/* Static Methods */
	/* Properties */
	/* Class Properties */
}

public sealed class Stable
/*
	Segment Breakdown:
	--------------------------------
	--------------------------------
*/
{
	/* Constructors */
	/* Instance Methods */
	/* Static Methods */
	/* Properties */
	/* Class Properties */
}
