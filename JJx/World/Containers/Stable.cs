/*
	Junk Jack X: World Container
	- Stable

	Segment Breakdown:
	--------------------------------
	--------------------------------

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
		for (var i = 0; i < this.Animals.Length; ++i)
			this.Animals[i] = new Animal(0xFFFF, 0, 0);
		this.Feed = new Item(0xFFFF, 0);
	}
	private Stable((ushort, ushort) position, Animal[] animals, Item item)
	{
		this.Position = position;
		this.Animals = animals;
		this.Feed = item;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var workingData = new byte[SIZE];
		// Position
		Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Position.X, 0);
		Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Position.Y, 2);
		await stream.WriteAsync(workingData, 0, workingData.Length);
		// Animals
		foreach (var animal in this.Animals)
			await animal.ToStream(stream);
		// Padding
		Utilities.ByteConverter.Write(new Span<byte>(workingData), (ushort)0, 0);
		await stream.WriteAsync(workingData, 0, SIZEOF_PADDING);
		// Feed Item
		await this.Feed.ToStream(stream);
	}
	/* Static Methods */
	public static async Task<Stable> FromStream(Stream stream)
	{
		var bytesRead = 0;
		var workingData = new byte[SIZE];
		while (bytesRead < SIZE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZE - bytesRead);
		// Position
		var position = (
			Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData), 0),
			Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData), 2)
		);
		// Animals
		var animals = new Animal[COUNT_ANIMALS];
		for (var i = 0; i < animals.Length; ++i)
			animals[i] = await Animal.FromStream(stream);
		// Padding
		bytesRead = 0;
		while (bytesRead < SIZEOF_PADDING)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_PADDING - bytesRead);
		// Feed Item
		var item = await Item.FromStream(stream);
		return new Stable(position, animals, item);
	}
	/* Properties */
	public (ushort X, ushort Y) Position;
	public readonly Animal[] Animals = new Animal[COUNT_ANIMALS];
	public Item Feed;
	//public readonly Pet[]
	/* Class Properties */
	private const byte SIZE = 4;
	private const byte COUNT_ANIMALS = 3;
	private const byte SIZEOF_PADDING = 2;
	/* Sub-Classes */
	public sealed class Animal
	{
		/* Constructors */
		public Animal(ushort id, ushort data = 0, ushort feed = 0)
		{
			this.Id   = id;
			this.Data = data;
			this.Feed = feed;
		}
		/* Instance Methods */
		public async Task ToStream(Stream stream)
		{
			var workingData = new byte[SIZE];
			Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Id,   0);
			Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Data, 2);
			Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Feed, 4);
			await stream.WriteAsync(workingData, 0, workingData.Length);
		}
		/* Static Methods */
		public static async Task<Animal> FromStream(Stream stream)
		{
			var bytesRead = 0;
			var workingData = new byte[SIZE];
			while (bytesRead < SIZE)
				bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZE - bytesRead);
			// Id
			var type  = Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData), 0);
			var data = Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData),  2);
			var feed = Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData),  4);
			return new Animal(type, data, feed);
		}
		/* Properties */
		public ushort Id;
		public ushort Data;
		public ushort Feed;
		/* Class Properties */
		internal const byte SIZE = 6;
	}
}
