/*
	Junk Jack X: Core
	- Item

	Segment Breakdown:
	----------------------------------------------------------------
	Segment[0x0 : 0x3] = Modifier   | Length: 4 (0x4) | Type: uint32
	Segment[0x4 : 0x5] = Id         | Length: 2 (0x2) | Type: uint16
	Segment[0x6 : 0x7] = Count      | Length: 2 (0x2) | Type: uint16
	Segment[0x8 : 0x9] = Durabiltiy | Length: 2 (0x2) | Type: uint16
	Segment[0xA : 0xB] = Icon       | Length: 2 (0x2) | Type: uint16
	----------------------------------------------------------------
	Length: 12 (0xC)

	Written By: Ryan Smith
*/
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Item
{
	/* Constructor */
	public Item(ushort id, ushort count, ushort durability = 0, uint modifier = 0, ushort icon = 0)
	{
		this.Id = id;
		this.Count = count;
		this.Durability = durability;
		this.Modifier = modifier;
		this.Icon = icon;
	}
	/* Instance Methods */
	public override string ToString() => $"Item(Id:{this.Id}, Count:{this.Count}, Modifier:0x{this.Modifier:X4})";
	public async Task ToStream(Stream stream)
	{
		var buffer = new byte[Item.SIZE];
		JJx.BitConverter.LittleEndian.Write(this.Modifier, buffer, Item.OFFSET_MODIFIER);
		JJx.BitConverter.LittleEndian.Write(this.Id, buffer, Item.OFFSET_ID);
		JJx.BitConverter.LittleEndian.Write(this.Count, buffer, Item.OFFSET_COUNT);
		JJx.BitConverter.LittleEndian.Write(this.Durability, buffer, Item.OFFSET_DURABILITY);
		JJx.BitConverter.LittleEndian.Write(this.Icon, buffer, Item.OFFSET_ICON);
		await stream.WriteAsync(buffer, 0, buffer.Length);
	}
	/* Static Methods */
	public static async Task<Item> FromStream(Stream stream)
	{
		var bytesRead = 0;
		var buffer = new byte[Item.SIZE];
		while (bytesRead < buffer.Length)
			bytesRead += await stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
		var modifier   = JJx.BitConverter.LittleEndian.GetUInt32(buffer, Item.OFFSET_MODIFIER);
		var id         = JJx.BitConverter.LittleEndian.GetUInt16(buffer, Item.OFFSET_ID);
		var count      = JJx.BitConverter.LittleEndian.GetUInt16(buffer, Item.OFFSET_COUNT);
		var durability = JJx.BitConverter.LittleEndian.GetUInt16(buffer, Item.OFFSET_DURABILITY);
		var icon       = JJx.BitConverter.LittleEndian.GetUInt16(buffer, Item.OFFSET_ICON);
		return new Item(id, count, durability, modifier, icon);
	}
	/* Properties */
	public ushort Id;
	public ushort Count;
	public ushort Durability;
	public uint Modifier;
	public ushort Icon;
	/* Class Properties */
	private const byte SIZE              = 12;
	private const byte OFFSET_MODIFIER   =  0;
	private const byte OFFSET_ID         =  4;
	private const byte OFFSET_COUNT      =  6;
	private const byte OFFSET_DURABILITY =  8;
	private const byte OFFSET_ICON       = 10;
}
