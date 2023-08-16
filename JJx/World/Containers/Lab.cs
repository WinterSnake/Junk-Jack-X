/*
	Junk Jack X: World
	- [Container]Lab

	Segment Breakdown:
	------------------------------------------------------------------------------------------------------------------------
	Segment[0x0 : 0x2] = X Position | Length: 2  (0x2) | Type: uint16
	Segment[0x2 : 0x3] = Y Position | Length: 2  (0x2) | Type: uint16
		<Item[5]>
	------------------------------------------------------------------------------------------------------------------------

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

public sealed class LabContainer
{
	/* Constructors */
	public LabContainer((ushort x, ushort y) position)
	{
		this.Position = position;
		for (var i = 0; i < this.Items.Length; ++i)
			this.Items[i] = new Item(0, 0);
	}
	private LabContainer((ushort x, ushort y) position, Item[] items)
	{
		this.Position = position;
		this.Items = items;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
		// TODO: Single WriteAsync | Use SIZE buffer
		// TODO: Ensure BitConverter.GetBytes<T> forces little endian
	{
		byte[] bytes;
		var workingData = new byte[SIZE];
		// Position
		// -X
		bytes = BitConverter.GetBytes(this.Position.X);
		Array.Copy(bytes, 0, workingData, 0, bytes.Length);
		// -Y
		bytes = BitConverter.GetBytes(this.Position.Y);
		Array.Copy(bytes, 0, workingData, 2, bytes.Length);
		await stream.WriteAsync(workingData, 0, SIZEOF_POSITION);
		// Items
		foreach (var item in this.Items)
			await item.ToStream(stream);
	}
	/* Static Methods */
	public static async Task<LabContainer> FromStream(Stream stream)
		// TODO: Ensure BitConverter.To<T> forces little endian
	{
		var bytesRead = 0;
		var workingData = new byte[SIZE];
		while (bytesRead < SIZE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZE - bytesRead);
		// Position
		var position = (
			BitConverter.ToUInt32(new Span<byte>(workingData).Slice(0, 2)),
			BitConverter.ToUInt32(new Span<byte>(workingData).Slice(2, 2))
		);
		// Items
		var items = new Item[COUNT_ITEMS];
		for (var i = 0; i < COUNT_ITEMS; ++i)
			items[i] = await Item.FromStream(stream);
		return new LabContainer(position, items);
	}
	/* Properties */
	public (ushort X, ushort Y) Position;
	public readonly Item[] Items = new Item[COUNT_ITEMS];
	/* Class Properties */
	private const byte SIZE            = 4;
	private const byte SIZEOF_POSITION = 4;
	private const byte COUNT_ITEMS     = 5;
}
