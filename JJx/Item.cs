/*
	Junk Jack X: Item

	Segment Breakdown:
	----------------------------------------------------------------
	:<Item>
	Segment[0x0 : 0x3] = Modifier   | Length: 4 (0x4) | Type: uint32
	Segment[0x4 : 0x5] = Id         | Length: 2 (0x2) | Type: uint16
	Segment[0x6 : 0x7] = Count      | Length: 2 (0x2) | Type: uint16
	Segment[0x8 : 0x9] = Durabiltiy | Length: 2 (0x2) | Type: uint16
	Segment[0xA : 0xB] = Icon       | Length: 2 (0x2) | Type: uint16
	----------------------------------------------------------------
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
	public override string ToString()
	{
		return $"Id={this.Id} | Count={this.Count} | Durability={this.Durability} | Properties={this.Modifier} | Icon={this.Icon}";
	}
	public async Task ToStream(Stream stream)
	{
		var workingData = new byte[SIZE];
		BitConverter.Write(workingData, this.Modifier,   0);
		BitConverter.Write(workingData, this.Id,         4);
		BitConverter.Write(workingData, this.Count,      6);
		BitConverter.Write(workingData, this.Durability, 8);
		BitConverter.Write(workingData, this.Icon,      10);
		await stream.WriteAsync(workingData, 0, workingData.Length);
	}
	/* Static Methods */
	public static async Task<Item> FromStream(Stream stream)
	{
		var bytesRead = 0;
		var workingData = new byte[SIZE];
		while (bytesRead < SIZE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZE - bytesRead);
		var modifier   = BitConverter.GetUInt32(workingData,  0);
		var id         = BitConverter.GetUInt16(workingData,  4);
		var count      = BitConverter.GetUInt16(workingData,  6);
		var durability = BitConverter.GetUInt16(workingData,  8);
		var icon       = BitConverter.GetUInt16(workingData, 10);
		return new Item(id, count, durability, modifier, icon);
	}
	/* Properties */
	public ushort Id;
	public ushort Count;
	public ushort Durability;
	public uint Modifier;
	public ushort Icon;
	/* Class Properties */
	internal const byte SIZE = 12;
}
