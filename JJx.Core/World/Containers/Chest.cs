/*
	Junk Jack X: Core
	- [World: Containers]Chest

	Segment Breakdown:
	----------------------------------------------------------------
	Segment[0x0 : 0x3] = X Position | Length: 4 (0x4) | Type: uint32
	Segment[0x4 : 0x7] = Y Position | Length: 4 (0x4) | Type: uint32
	Segment[0x8 : 0xB] = Item Count | Length: 4 (0x4) | Type: uint32
	----------------------------------------------------------------
	Size: 12 (0xC)

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
	}
	private Chest((uint, uint) position, Item[] items)
	{
		this.Position = position;
		this.Items = new List<Item>(items);
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var buffer = new byte[SIZE];
		BitConverter.LittleEndian.Write(this.Position.X, buffer, OFFSET_POSITION);
		BitConverter.LittleEndian.Write(this.Position.Y, buffer, OFFSET_POSITION + sizeof(uint));
		BitConverter.LittleEndian.Write((uint)this.Items.Count, buffer, OFFSET_ITEMS);
		await stream.WriteAsync(buffer, 0, buffer.Length);
		foreach (var item in this.Items)
			await item.ToStream(stream);
	}
	/* Static Methods */
	public static async Task<Chest> FromStream(Stream stream)
	{
		var bytesRead = 0;
		var buffer = new byte[SIZE];
		while (bytesRead < buffer.Length)
			bytesRead += await stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
		Console.WriteLine(BitConverter.ToString(buffer));
		var position = (
			BitConverter.LittleEndian.GetUInt32(buffer, OFFSET_POSITION),
			BitConverter.LittleEndian.GetUInt32(buffer, OFFSET_POSITION + sizeof(uint))
		);
		var itemCount = BitConverter.LittleEndian.GetUInt32(buffer, OFFSET_ITEMS);
		Console.WriteLine($"Position: {position}, Item Count: {itemCount}");
		var items = new Item[itemCount];
		for (var i = 0; i < items.Length; ++i)
			items[i] = await Item.FromStream(stream);
		return new Chest(position, items);
	}
	/* Properties */
	public (uint X, uint Y) Position;
	public readonly List<Item> Items = new List<Item>();
	/* Class Properties */
	private const byte SIZE            = 12;
	private const byte OFFSET_POSITION =  0;
	private const byte OFFSET_ITEMS    =  8;
}
