/*
	Junk Jack X: Core
	- [World: Containers]Stable

	Segment Breakdown:
	------------------------------------------------------------------
	Segment[0x0  :  0x1] = X Position | Length: 2 (0x2) | Type: uint16
	Segment[0x2  :  0x3] = Y Position | Length: 2 (0x2) | Type: uint16
	Segment[0x16 : 0x17] = UNKNOWN    | Length: 2 (0x2) | Type: ???
	------------------------------------------------------------------
	Size: 6 (0x6) + {Animal[3]} + Item

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Stable
{
	/* Constructors */
	public Stable(ushort x, ushort y)
	{
		this.Position = (x, y);
	}
	private Stable((ushort, ushort) position, Animal[] animals, Item feed)
	{
		this.Position = position;
		this.Animals = animals;
		this.Feed = feed;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var buffer = new byte[SIZE];
		BitConverter.LittleEndian.Write(this.Position.X, buffer, OFFSET_POSITION);
		BitConverter.LittleEndian.Write(this.Position.Y, buffer, OFFSET_POSITION + sizeof(ushort));
		await stream.WriteAsync(buffer, 0, SIZEOF_POSITION);
		foreach (var animal in this.Animals)
			await animal.ToStream(stream);
		await stream.WriteAsync(buffer, OFFSET_UNKNOWN, SIZEOF_UNKNOWN);
		await this.Feed.ToStream(stream);
	}
	/* Static Methods */
	public static async Task<Stable> FromStream(Stream stream)
	{
		var bytesRead = 0;
		var buffer = new byte[SIZE];
		while (bytesRead < buffer.Length)
			bytesRead += await stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
		// Position
		var position = (
			BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_POSITION),
			BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_POSITION + sizeof(ushort))
		);
		// Animals
		var animals = new Animal[SIZEOF_ANIMALS];
		for (var i = 0; i < animals.Length; ++i)
			animals[i] = await Animal.FromStream(stream);
		// -UNKNOWN(2)- \\
		//while (bytesRead < buffer.Length)
		//	bytesRead += await stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
		var feed = await Item.FromStream(stream);
		return new Stable(position, animals, feed);
	}
	/* Properties */
	public (ushort X, ushort Y) Position;
	public readonly Animal[] Animals = new Animal[SIZEOF_ANIMALS];
	public Item Feed = new Item(0xFFFF, 0);
	/* Class Properties */
	private const byte SIZE            = 6;
	private const byte SIZEOF_POSITION = 4;
	private const byte SIZEOF_ANIMALS  = 3;
	private const byte SIZEOF_UNKNOWN  = 2;
	private const byte OFFSET_POSITION = 0;
	private const byte OFFSET_UNKNOWN  = 5;
	/* Sub-Classes */
	public sealed class Animal
	{
		/* Constructors */
		public Animal(ushort id, ushort data = 0x0000, ushort feed = 0x0000)
		{
			this.Id = id;
			this.Data = data;
			this.Feed = feed;
		}
		/* Instance Methods */
		public async Task ToStream(Stream stream)
		{
			var buffer = new byte[SIZE];
			BitConverter.LittleEndian.Write(this.Id, buffer, OFFSET_ID);
			BitConverter.LittleEndian.Write(this.Data, buffer, OFFSET_DATA);
			BitConverter.LittleEndian.Write(this.Feed, buffer, OFFSET_FEED);
			await stream.WriteAsync(buffer, 0, buffer.Length);
		}
		/* Static Methods */
		public static async Task<Animal> FromStream(Stream stream)
		{
			var bytesRead = 0;
			var buffer = new byte[SIZE];
			while (bytesRead < buffer.Length)
				bytesRead += await stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
			var id = BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_ID);
			var data = BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_DATA);
			var feed = BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_FEED);
			return new Animal(id, data, feed);
		}
		/* Properties */
		public ushort Id;
		public ushort Data;
		public ushort Feed;
		/* Class Properties */
		private const byte SIZE        = 6;
		private const byte OFFSET_ID   = 0;
		private const byte OFFSET_DATA = 2;
		private const byte OFFSET_FEED = 4;
	}
}
