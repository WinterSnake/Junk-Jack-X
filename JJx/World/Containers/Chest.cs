/*
	Junk Jack X: World
	- Container: Chest

	Segment Breakdown:
	----------------------------------------------------------------
	Segment[0x0 : 0x3] = X Position | Length: 4 (0x4) | Type: uint32
	Segment[0x4 : 0x7] = Y Position | Length: 4 (0x4) | Type: uint32
	Segment[0x8 : 0xB] = Item Count | Length: 4 (0x4) | Type: uint32
	----------------------------------------------------------------
	Size: 12 (0xC) + ItemCount * 12 (Item[itemCount])

	Written By: Ryan Smith
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Chest
{
	/* Constructors */
	public Chest((uint, uint) position)
	{
		this.Position = position;
		this.Items = new List<Item>();
	}
	public Chest((uint, uint) position, uint itemCount)
	{
		this.Position = position;
		this.Items = new List<Item>((int)itemCount);
		for (var i = 0; i < itemCount; ++i)
			this.Items.Add(new Item(0xFFFF, 0));
	}
	private Chest((uint, uint) position, Item[] items)
	{
		this.Position = position;
		this.Items = new List<Item>(items);
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var workingData = new byte[SIZE];
		BitConverter.Write(workingData, this.Position.X,        0);
		BitConverter.Write(workingData, this.Position.Y,        4);
		BitConverter.Write(workingData, (uint)this.Items.Count, 8);
		await stream.WriteAsync(workingData, 0, workingData.Length);
		foreach (var item in this.Items)
			await item.ToStream(stream);
	}
	/* Static Methods */
	public static async Task<Chest> FromStream(Stream stream)
	{
		int bytesRead = 0;
		var workingData = new byte[SIZE];
		while (bytesRead < workingData.Length)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, workingData.Length - bytesRead);
		// Position
		var position = (
			BitConverter.GetUInt32(workingData, 0),
			BitConverter.GetUInt32(workingData, 4)
		);
		// Items
		var itemCount = BitConverter.GetUInt32(workingData, 8);
		var items = new Item[itemCount];
		for (var i = 0; i < items.Length; ++i)
			items[i] = await Item.FromStream(stream);
		return new Chest(position, items);
	}
	/* Properties */
	public (uint X, uint Y) Position;
	public readonly List<Item> Items;
	/* Class Properties */
	public const uint ITEMCOUNT_WOOD       = 12;
	public const uint ITEMCOUNT_COPPER     = 24;
	public const uint ITEMCOUNT_IRON       = 24;
	public const uint ITEMCOUNT_SILVER     = 36;
	public const uint ITEMCOUNT_GOLD       = 36;
	public const uint ITEMCOUNT_MITHRIL    = 48;
	public const uint ITEMCOUNT_ANTANIUM   = 60;
	public const uint ITEMCOUNT_GALVANIUM  = 72;
	public const uint ITEMCOUNT_TITANIUM   = 72;
	public const uint ITEMCOUNT_BOTTOMLESS = 120;
	private const byte SIZE = 12;
}
