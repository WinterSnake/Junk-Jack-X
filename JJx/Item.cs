/*
	Junk Jack X: Item

	Segment Breakdown:    | Length: 12 (0xC)
	------------------------------------------------------------------------------------------------------------------------
	Segment[0x0]  - Segment[0x3]  = Modifier    | Length: 4  (0x04) | Type: uint16/uint32 (?)
	Segment[0x4]  - Segment[0x5]  = Id          | Length: 2  (0x02) | Type: uint16
	Segment[0x0]  - Segment[0x3]  = Count       | Length: 2  (0x02) | Type: uint16
	Segment[0x0]  - Segment[0x3]  = Durabiltiy  | Length: 2  (0x02) | Type: uint16 / float16 (?)
	Segment[0x0]  - Segment[0x3]  = Render Icon | Length: 2  (0x02) | Type: uint16
	------------------------------------------------------------------------------------------------------------------------

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
		var workingData = new byte[12];
		workingData[11] = (byte)((this.Icon       & 0xFF00)     >>  8);
		workingData[10] = (byte)((this.Icon       & 0x00FF)     >>  0);
		workingData[9]  = (byte)((this.Durability & 0xFF00)     >>  8);
		workingData[8]  = (byte)((this.Durability & 0x00FF)     >>  0);
		workingData[7]  = (byte)((this.Count      & 0xFF00)     >>  8);
		workingData[6]  = (byte)((this.Count      & 0x00FF)     >>  0);
		workingData[5]  = (byte)((this.Id         & 0xFF00)     >>  8);
		workingData[4]  = (byte)((this.Id         & 0x00FF)     >>  0);
		workingData[3]  = (byte)((this.Modifier   & 0xFF000000) >> 32);
		workingData[2]  = (byte)((this.Modifier   & 0x00FF0000) >> 16);
		workingData[1]  = (byte)((this.Modifier   & 0x0000FF00) >>  8);
		workingData[0]  = (byte)((this.Modifier   & 0x000000FF) >>  0);
		await stream.WriteAsync(workingData, 0, workingData.Length);
	}
	/* Static Methods */
	public static async Task<Item> FromStream(Stream stream)
	{
		var workingData = new byte[12];
		await stream.ReadAsync(workingData, 0, 0xC);
		return new Item(
			(ushort)((workingData[5] <<  8) | workingData[4]),                                                  // Id
			(ushort)((workingData[7] <<  8) | workingData[6]),                                                  // Count
			(ushort)((workingData[9] <<  8) | workingData[8]),                                                  // Durability
			(uint)  ((workingData[3] << 32) | (workingData[2] << 16) | (workingData[1] << 8) | workingData[0]), // Modifier
			(ushort)((workingData[11] << 8) | workingData[10])                                                  // Icon
		);
	}
	/* Properties */
	public ushort Id;
	public ushort Count;
	public ushort Durability;
	public uint Modifier;
	public ushort Icon;
}
