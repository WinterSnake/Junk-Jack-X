/*
	Junk Jack X: World
	- Container: Forge

	Segment Breakdown:
	------------------------------------------------------------------------
	Segment[0x0 : 0x1] = X Position | Length:  2 (0x2) | Type: uint16
	Segment[0x2 : 0x3] = Y Position | Length:  2 (0x2) | Type: uint16
	Segment[0x4 : 0xF] = UNKNOWN    | Length: 12 (0xC) | Type: Forge::Status
	------------------------------------------------------------------------
	Size: 16 (0x10) + 36 (0x24) {Item[3]}

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Forge
{
	/* Constructors */
	public Forge((ushort, ushort) position)
	{
		this.Position = position;
		this.Items = new Item[COUNTOF_ITEMS];
		for (var i = 0; i < this.Items.Length; ++i)
			this.Items[i] = new Item(0xFFFF, 0);
	}
	private Forge((ushort, ushort) position, Item[] items)
	{
		this.Position = position;
		this.Items = items;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var workingData = new byte[SIZE];
		// Position
		BitConverter.Write(workingData, this.Position.X, 0);
		BitConverter.Write(workingData, this.Position.Y, 2);
		// Status
		await stream.WriteAsync(workingData, 0, workingData.Length);
		// Items
		foreach (var item in this.Items)
			await item.ToStream(stream);
	}
	/* Static Methods */
	public static async Task<Forge> FromStream(Stream stream)
	{
		int bytesRead = 0;
		var workingData = new byte[SIZE];
		while (bytesRead < workingData.Length)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, workingData.Length - bytesRead);
		// Position
		var position = (
			BitConverter.GetUInt16(workingData, 0),
			BitConverter.GetUInt16(workingData, 2)
		);
		// Status
		// Items
		var items = new Item[COUNTOF_ITEMS];
		for (var i = 0; i < items.Length; ++i)
			items[i] = await Item.FromStream(stream);
		return new Forge(position, items);
	}
	/* Properties */
	public (ushort X, ushort Y) Position;
	public readonly Item[] Items;
	/* Class Properties */
	private const byte SIZE          = 16;
	private const byte COUNTOF_ITEMS =  3;
}
