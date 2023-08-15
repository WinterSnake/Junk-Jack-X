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
		// TODO: Write using single reverse
	{
		byte[] bytes;
		var workingData = new byte[SIZE];
		// Modifier
		bytes = BitConverter.GetBytes(this.Modifier);
		Array.Reverse(bytes);
		Array.Copy(bytes, 0, workingData, 0, bytes.Length);
		// Id
		bytes = BitConverter.GetBytes(this.Id);
		Array.Reverse(bytes);
		Array.Copy(bytes, 0, workingData, 4, bytes.Length);
		// Count
		bytes = BitConverter.GetBytes(this.Count);
		Array.Reverse(bytes);
		Array.Copy(bytes, 0, workingData, 6, bytes.Length);
		// Durability
		bytes = BitConverter.GetBytes(this.Durability);
		Array.Reverse(bytes);
		Array.Copy(bytes, 0, workingData, 8, bytes.Length);
		// Icon
		bytes = BitConverter.GetBytes(this.Icon);
		Array.Reverse(bytes);
		Array.Copy(bytes, 0, workingData, 10, bytes.Length);

		await stream.WriteAsync(workingData, 0, workingData.Length);
	}
	/* Static Methods */
	public static async Task<Item> FromStream(Stream stream)
		// TODO: Ensure BitConverter.To<T> forces little endian
	{
		var bytesRead = 0;
		var workingData = new byte[SIZE];
		while (bytesRead < SIZE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZE - bytesRead);
		var modifier   = BitConverter.ToUInt32(new Span<byte>(workingData).Slice(0, 4));
		var id         = BitConverter.ToUInt16(new Span<byte>(workingData).Slice(4, 2));
		var count      = BitConverter.ToUInt16(new Span<byte>(workingData).Slice(6, 2));
		var durability = BitConverter.ToUInt16(new Span<byte>(workingData).Slice(8, 2));
		var icon       = BitConverter.ToUInt16(new Span<byte>(workingData).Slice(10, 2));
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
