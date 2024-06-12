/*
	Junk Jack X: Core
	- Item

	Segment Breakdown:
	----------------------------------------------------------------
	Segment[0x0 : 0x3] = Data       | Length: 4 (0x4) | Type: uint32
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
	public Item(ushort id, ushort count, ushort durability = 0, uint data = 0, ushort icon = 0)
	{
		this.Id = id;
		this.Count = count;
		this.Durability = durability;
		this.Data = data;
		this.Icon = icon;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var buffer = new byte[Item.SIZE];
		BitConverter.LittleEndian.Write(this.Data, buffer, OFFSET_DATA);
		BitConverter.LittleEndian.Write(this.Id, buffer, OFFSET_ID);
		BitConverter.LittleEndian.Write(this.Count, buffer, OFFSET_COUNT);
		BitConverter.LittleEndian.Write(this.Durability, buffer, OFFSET_DURABILITY);
		BitConverter.LittleEndian.Write(this.Icon, buffer, OFFSET_ICON);
		await stream.WriteAsync(buffer, 0, buffer.Length);
	}
	/* Static Methods */
	public static async Task<Item> FromStream(Stream stream)
	{
		var bytesRead = 0;
		var buffer = new byte[SIZE];
		while (bytesRead < buffer.Length)
			bytesRead += await stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
		var data       = BitConverter.LittleEndian.GetUInt32(buffer, OFFSET_DATA);
		var id         = BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_ID);
		var count      = BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_COUNT);
		var durability = BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_DURABILITY);
		var icon       = BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_ICON);
		return new Item(id, count, durability, data, icon);
	}
	/* Properties */
	public ushort Id;
	public ushort Count;
	public ushort Durability;
	public uint Data;
	public ushort Icon;
	/* Class Properties */
	private const byte SIZE              = 12;
	private const byte OFFSET_DATA       =  0;
	private const byte OFFSET_ID         =  4;
	private const byte OFFSET_COUNT      =  6;
	private const byte OFFSET_DURABILITY =  8;
	private const byte OFFSET_ICON       = 10;
}
