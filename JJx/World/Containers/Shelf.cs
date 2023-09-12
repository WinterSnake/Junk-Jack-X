/*
	Junk Jack X: World Container
	- Shelf

	Segment Breakdown:
	----------------------------------------------------------------
	Segment[0x0 : 0x1] = X Position | Length: 2 (0x2) | Type: uint16
	Segment[0x2 : 0x3] = Y Position | Length: 2 (0x2) | Type: uint16
	----------------------------------------------------------------
	Size: 4 (0x4)

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Shelf
{
	/* Constructors */
	public Shelf((ushort, ushort) position)
	{
		this.Position = position;
		for (var i = 0; i < this.Items.Length; ++i)
			this.Items[i] = new Item(0xFFFF, 0);
	}
	private Shelf((ushort, ushort) position, Item[] items)
	{
		this.Position = position;
		this.Items = items;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var workingData = new byte[SIZE];
		// Position
		Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Position.X, 0);
		Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Position.Y, 2);
		await stream.WriteAsync(workingData, 0, workingData.Length);
		// Items
		foreach (var item in this.Items)
			await item.ToStream(stream);
	}
	/* Static Methods */
	public static async Task<Shelf> FromStream(Stream stream)
	{
		int bytesRead = 0;
		var workingData = new byte[SIZE];
		while (bytesRead < SIZE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZE - bytesRead);
		// Position
		var position = (
			Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData), 0),
			Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData), 2)
		);
		// Items
		var items = new Item[COUNT_ITEMS];
		for (var i = 0; i < items.Length; ++i)
			items[i] = await Item.FromStream(stream);
		return new Shelf(position, items);
	}
	/* Properties */
	public (ushort X, ushort Y) Position;
	public readonly Item[] Items = new Item[COUNT_ITEMS];
	/* Class Properties */
	private const byte SIZE        = 4;
	private const byte COUNT_ITEMS = 4;
}
