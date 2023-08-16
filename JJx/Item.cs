/*
	Junk Jack X: Item

	Segment Breakdown:
	------------------------------------------------------------------------------------------------------------------------
	Segment[0x0 : 0x3] = Modifier    | Length: 4  (0x04) | Type: uint16/uint32 (?)
	Segment[0x4 : 0x5] = Id          | Length: 2  (0x02) | Type: uint16
	Segment[0x6 : 0x7] = Count       | Length: 2  (0x02) | Type: uint16
	Segment[0x8 : 0x9] = Durabiltiy  | Length: 2  (0x02) | Type: uint16 / float16 (?)
	Segment[0xA : 0xB] = Render Icon | Length: 2  (0x02) | Type: uint16
	------------------------------------------------------------------------------------------------------------------------
	Length: 12 (0xC)

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Item
{
	/* Constructors */
	public Item(ushort id, ushort count, ushort durability = 0, uint modifier = 0, ushort icon = 0)
	{
		this.Id = id;
		this.Count = count;
		this.Durability = durability;
		this.Modifier = modifier;
		this.Icon = icon;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var workingData = new byte[SIZE];
		Utils.ByteConverter.Write(new Span<byte>(workingData), this.Modifier,   0);
		Utils.ByteConverter.Write(new Span<byte>(workingData), this.Id,         4);
		Utils.ByteConverter.Write(new Span<byte>(workingData), this.Count,      6);
		Utils.ByteConverter.Write(new Span<byte>(workingData), this.Durability, 8);
		Utils.ByteConverter.Write(new Span<byte>(workingData), this.Icon,      10);
		await stream.WriteAsync(workingData, 0, workingData.Length);
	}
	/* Static Methods */
	public static async Task<Item> FromStream(Stream stream)
	{
		var bytesRead = 0;
		var workingData = new byte[SIZE];
		while (bytesRead < SIZE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZE - bytesRead);
		var modifier   = Utils.ByteConverter.GetUInt32(new Span<byte>(workingData),  0);
		var id         = Utils.ByteConverter.GetUInt16(new Span<byte>(workingData),  4);
		var count      = Utils.ByteConverter.GetUInt16(new Span<byte>(workingData),  6);
		var durability = Utils.ByteConverter.GetUInt16(new Span<byte>(workingData),  8);
		var icon       = Utils.ByteConverter.GetUInt16(new Span<byte>(workingData), 10);
		return new Item(id, count, durability, modifier, icon);
	}
	/* Properties */
	public ushort Id;
	public ushort Count;
	public ushort Durability;
	public uint Modifier;
	public ushort Icon;
	/* Class Properties */
	private const byte SIZE = 12;
}
