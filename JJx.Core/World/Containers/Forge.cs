/*
	Junk Jack X: Core
	- [World: Containers]Forge

	Segment Breakdown:
	-----------------------------------------------------------------
	Segment[0x0 : 0x1] = X Position | Length:  2 (0x2) | Type: uint16
	Segment[0x2 : 0x3] = Y Position | Length:  2 (0x2) | Type: uint16
	Segment[0x4 : 0xF] = UNKNOWN    | Length: 10 (0xA) | Type: ???
	-----------------------------------------------------------------
	Size: 16 (0x10) + {Item[3]}

	Written By: Ryan Smith
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Forge
{
	/* Constructors */
	public Forge(ushort x, ushort y)
	{
		this.Position = (x, y);
	}
	private Forge((ushort, ushort) position, Item[] items)
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
		// UNKNOWN(10)
		await stream.WriteAsync(buffer, 0, buffer.Length);
		foreach (var item in this.Items)
			await item.ToStream(stream);
	}
	/* Static Methods */
	public static async Task<Forge> FromStream(Stream stream)
	{
		var bytesRead = 0;
		var buffer = new byte[SIZE];
		while (bytesRead < buffer.Length)
			bytesRead += await stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
		var position = (
			BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_POSITION),
			BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_POSITION + sizeof(ushort))
		);
		// UNKNOWN(10)
		// Items
		var items = new Item[SIZEOF_ITEMS];
		for (var i = 0; i < items.Length; ++i)
			items[i] = await Item.FromStream(stream);
		return new Forge(position, items);
	}
	/* Properties */
	public (ushort X, ushort Y) Position;
	public readonly Item[] Items = new Item[SIZEOF_ITEMS];
	/* Class Properties */
	private const byte SIZE            = 16;
	private const byte OFFSET_POSITION =  0;
	private const byte SIZEOF_ITEMS    =  3;
}