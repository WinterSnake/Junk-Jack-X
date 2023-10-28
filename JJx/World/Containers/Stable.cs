/*
	Junk Jack X: World
	- Container: Stable

	Segment Breakdown:
	-----------------------------------------------------------------------------------
	:<Stable>
	Segment[0x0  :  0x1] = X Position | Length: 2 (0x2) | Type: uint16
	Segment[0x2  :  0x3] = Y Position | Length: 2 (0x2) | Type: uint16
	Segment[0x16 : 0x17] = Padding    | Length: 2 (0x2) | Type: uint16 | 4 + 3 * Animal
	:<Animal>
	Segment[0x0 : 0x1] = Id           | Length: 2 (0x2) | Type: uint16
	Segment[0x2 : 0x3] = Data         | Length: 2 (0x2) | Type: uint16
	Segment[0x5 : 0x6] = Feed         | Length: 2 (0x2) | Type: uint16
	-----------------------------------------------------------------------------------

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Stable
{
	/* Constructors */
	public Stable((ushort, ushort) position)
	{
		this.Position = position;
		this.Feed = new Item(0xFFFF, 0);
	}
	private Stable((ushort, ushort) position, Animal[] animals, Item feed)
	{
		this.Position = position;
		this.Animals = animals;
		this.Feed = feed;
	}
	/* Instance Methods */
	public override string ToString() => $"Stable[{this.Position}]: Animals=[{String.Join<Animal>(", ", this.Animals)}], Feed={this.Feed}";
	public async Task ToStream(Stream stream)
	{
		var workingData = new byte[SIZE];
		// Padding
		BitConverter.Write(workingData, this.Position.X, 0);
		BitConverter.Write(workingData, this.Position.Y, 2);
		await stream.WriteAsync(workingData, 0, workingData.Length);
		// Animals
		foreach (var animal in this.Animals)
			await animal.ToStream(stream);
		// Padding
		BitConverter.Write(workingData, (ushort)0);
		await stream.WriteAsync(workingData, 0, SIZEOF_PADDING);
		// Item
		await this.Feed.ToStream(stream);
	}
	/* Static Methods */
	public static async Task<Stable> FromStream(Stream stream)
	{
		int bytesRead = 0;
		var workingData = new byte[SIZE];
		while (bytesRead < workingData.Length)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, workingData.Length - bytesRead);
		// Position
		var position = (
			BitConverter.GetUInt16(workingData, 0),
			BitConverter.GetUInt16(workingData, 2)
		);
		// Animals
		var animals = new Animal[COUNTOF_ANIMALS];
		for (var i = 0; i < animals.Length; ++i)
			animals[i] = await Animal.FromStream(stream);
		// Padding
		stream.Position += SIZEOF_PADDING;
		// Item
		var feed = await Item.FromStream(stream);
		return new Stable(position, animals, feed);
	}
	/* Properties */
	public (ushort X, ushort Y) Position;
	public readonly Animal[] Animals = new Animal[COUNTOF_ANIMALS];
	public Item Feed;
	/* Class Properties */
	private const byte SIZE = 4;
	private const byte SIZEOF_PADDING = 2;
	private const byte COUNTOF_ANIMALS = 3;
	/* Sub-Classes */
	public sealed class Animal
	{
		/* Constructors */
		public Animal(ushort id, ushort data = 0, ushort feed = 0)
		{
			this.Id = id;
			this.Data = data;
			this.Feed = feed;
		}
		/* Instance Methods */
		public override string ToString() => $"Id: {this.Id} | Data: 0x{this.Data:X4} | Feed: {this.Feed}";
		public async Task ToStream(Stream stream)
		{
			var workingData = new byte[SIZE];
			BitConverter.Write(workingData, this.Id,   0);
			BitConverter.Write(workingData, this.Data, 2);
			BitConverter.Write(workingData, this.Feed, 4);
			await stream.WriteAsync(workingData, 0, workingData.Length);
		}
		/* Static Methods */
		public static async Task<Animal> FromStream(Stream stream)
		{
			var bytesRead = 0;
			var workingData = new byte[SIZE];
			while (bytesRead < SIZE)
				bytesRead += await stream.ReadAsync(workingData, bytesRead, workingData.Length - bytesRead);
			var id   = BitConverter.GetUInt16(workingData,  0);
			var data = BitConverter.GetUInt16(workingData,  2);
			var feed = BitConverter.GetUInt16(workingData,  4);
			return new Animal(id, data, feed);
		}
		/* Properties */
		public ushort Id;
		public ushort Data;
		public ushort Feed;
		/* Class Properties */
		private const byte SIZE = 6;
	}
}
