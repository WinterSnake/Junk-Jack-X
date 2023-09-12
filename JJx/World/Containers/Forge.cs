/*
	Junk Jack X: World Container
	- Forge

	Segment Breakdown:
	------------------------------------------------------------------------
	Segment[0x0 : 0x1] = X Position | Length: 2  (0x2) | Type: uint16
	Segment[0x2 : 0x3] = Y Position | Length: 2  (0x2) | Type: uint16
	Segment[0x4 : 0xF] = UNKNOWN    | Length: 12 (0xC) | Type: Forge::Status
	------------------------------------------------------------------------

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
		Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Position.X, 0);
		Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Position.Y, 2);
		// =UNKNOWN=
		Utilities.ByteConverter.Write(new Span<byte>(workingData), (uint)0,  4);
		Utilities.ByteConverter.Write(new Span<byte>(workingData), (ulong)0, 8);
		// Items
		foreach (var item in this.Items)
			await item.ToStream(stream);
	}
	/* Static Methods */
	public static async Task<Forge> FromStream(Stream stream)
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
		// =UNKNOWN=
		// Items
		var items = new Item[COUNT_ITEMS];
		for (var i = 0; i < items.Length; ++i)
			items[i] = await Item.FromStream(stream);
		return new Forge(position, items);
	}
	/* Properties */
	public (ushort X, ushort Y) Position;
	public readonly Item[] Items = new Item[COUNT_ITEMS];
	/* Class Properties */
	private const byte SIZE        = 16;
	private const byte COUNT_ITEMS = 3;
}
