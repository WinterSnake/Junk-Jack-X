/*
	Junk Jack X: World Container
	- Lab

	Segment Breakdown:
	--------------------------------
	Segment[0x0 : 0x3] = X Position | Length: 4  (0x4) | Type: uint32
	Segment[0x4 : 0x7] = Y Position | Length: 4  (0x4) | Type: uint32
	Segment[0x8 : 0xB] = Item Count | Length: 4  (0x4) | Type: uint32
	--------------------------------

	Written By: Ryan Smith
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Lab
{
	/* Constructors */
	public Lab((ushort, ushort) position)
	{
		this.Position = position;
		for (var i = 0; i < this.Items.Length; ++i)
			this.Items[i] = new Item(0xFFFF, 0);
	}
	private Lab((ushort, ushort) position, Item[] items)
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
	public static async Task<Lab> FromStream(Stream stream)
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
		return new Lab(position, items);
	}
	/* Properties */
	public (ushort X, ushort Y) Position;
	public readonly Item[] Items = new Item[COUNT_ITEMS];
	/* Class Properties */
	private const byte SIZE        = 4;
	private const byte COUNT_ITEMS = 5;
}
