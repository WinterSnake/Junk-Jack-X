/*
	Junk Jack X: World
	- Chest

	Segment Breakdown:
	------------------------------------------------------------------------------------------------------------------------
	Segment[0x0 : 0x3] = X Position | Length: 4  (0x4) | Type: uint32
	Segment[0x4 : 0x7] = Y Position | Length: 4  (0x4) | Type: uint32
	Segment[0x8 : 0xB] = Item Count | Length: 4  (0x4) | Type: uint32
		<Item[]>
	------------------------------------------------------------------------------------------------------------------------
	Length: 7 (0x7)

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class ChestContainer
{
	/* Constructors */
	public ChestContainer((uint x, uint y) position, uint itemCount)
	{
		this.Position = position;
		this.Items = new Item[itemCount];
		for (var i = 0; i < itemCount; ++i)
			this.Items[i] = new Item(0, 0);
	}
	private ChestContainer((uint x, uint y) position, Item[] items)
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
		Array.Copy(bytes, 0, workingData, 4, bytes.Length);
		await stream.WriteAsync(workingData, 0, SIZEOF_POSITION);
		// Item Count
		bytes = BitConverter.GetBytes((uint)this.Items.Length);
		Array.Copy(bytes, 0, workingData, 8, bytes.Length);
		await stream.WriteAsync(workingData, 0, SIZEOF_ITEMLENGTH);
		// Items
		foreach (var item in this.Items)
			await item.ToStream(stream);
	}
	/* Static Methods */
	public static async Task<ChestContainer> FromStream(Stream stream)
		// TODO: Ensure BitConverter.To<T> forces little endian
	{
		var bytesRead = 0;
		var workingData = new byte[SIZE];
		while (bytesRead < SIZE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZE - bytesRead);
		// Position
		var position = (
			BitConverter.ToUInt32(new Span<byte>(workingData).Slice(0, 4)),
			BitConverter.ToUInt32(new Span<byte>(workingData).Slice(4, 4))
		);
		// Item Count
		var size = BitConverter.ToUInt32(new Span<byte>(workingData).Slice(8, 4));
		// Items
		var items = new Item[size];
		for (var i = 0; i < size; ++i)
			items[i] = await Item.FromStream(stream);
		return new ChestContainer(position, items);
	}
	/* Properties */
	public (uint X, uint Y) Position;
	public Item[] Items;
	/* Class Properties */
	public  const uint ITEMCOUNT_WOOD       = 12;
	public  const uint ITEMCOUNT_COPPER     = 24;
	public  const uint ITEMCOUNT_IRON       = 24;
	public  const uint ITEMCOUNT_SILVER     = 36;
	public  const uint ITEMCOUNT_GOLD       = 36;
	public  const uint ITEMCOUNT_MITHRIL    = 48;
	public  const uint ITEMCOUNT_ANTANIUM   = 60;
	public  const uint ITEMCOUNT_GALVANIUM  = 72;
	public  const uint ITEMCOUNT_TITANIUM   = 72;
	public  const uint ITEMCOUNT_BOTTOMLESS = 120;
	private const byte SIZE                 = 12;
	private const byte SIZEOF_POSITION      = 8;
	private const byte SIZEOF_ITEMLENGTH    = 8;
}
