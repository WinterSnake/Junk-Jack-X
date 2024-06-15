/*
	Junk Jack X: Core
	- [World: Containers]Lab

	Segment Breakdown:
	-----------------------------------------------------------------
	Segment[0x0 : 0x1] = X Position | Length:  2 (0x2) | Type: uint16
	Segment[0x2 : 0x3] = Y Position | Length:  2 (0x2) | Type: uint16
	-----------------------------------------------------------------
	Size: 4 (0x4) + {Item[5]}

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Lab
{
	/* Constructors */
	public Lab(ushort x, ushort y)
	{
		this.Position = (x, y);
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
		var buffer = new byte[SIZE];
		BitConverter.LittleEndian.Write(this.Position.X, buffer, OFFSET_POSITION);
		BitConverter.LittleEndian.Write(this.Position.Y, buffer, OFFSET_POSITION + sizeof(ushort));
		await stream.WriteAsync(buffer, 0, buffer.Length);
		foreach (var item in this.Items)
			await item.ToStream(stream);
	}
	/* Static Methods */
	public static async Task<Lab> FromStream(Stream stream)
	{
		var bytesRead = 0;
		var buffer = new byte[SIZE];
		while (bytesRead < buffer.Length)
			bytesRead += await stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
		var position = (
			BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_POSITION),
			BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_POSITION + sizeof(ushort))
		);
		var items = new Item[SIZEOF_ITEMS];
		for (var i = 0; i < items.Length; ++i)
			items[i] = await Item.FromStream(stream);
		return new Lab(position, items);
	}
	/* Properties */
	public (ushort X, ushort Y) Position;
	public readonly Item[] Items = new Item[SIZEOF_ITEMS];
	/* Class Properties */
	private const byte SIZE            = 4;
	private const byte SIZEOF_ITEMS    = 5;
	private const byte OFFSET_POSITION = 0;
}
