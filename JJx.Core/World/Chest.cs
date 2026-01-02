/*
	Junk Jack X: Core
	- [World]Chest

	Segment Breakdown:
	---------------------------------------------------------------
	Segment[0x0 : 0x3] = X         | Length: 4 (0x4) | Type: uint32
	Segment[0x4 : 0x7] = Y         | Length: 4 (0x4) | Type: uint32
	Segment[0x8 : 0xB] = Capacity  | Length: 4 (0x4) | Type: int32
	---------------------------------------------------------------
	Size: 12 (0xC)

	Written By: Ryan Smith
*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace JJx.Core;

public sealed class Chest
{
	/* Constructor */
	public Chest(uint x, uint y) :this((x, y), 0) { }
	public Chest((uint, uint) position) :this(position, 0) { }
	internal Chest(uint x, uint y, int capacity) :this((x, y), capacity) { }
	internal Chest((uint, uint) position, int capacity)
	{
		this.Position = position;
		this._Items = new(capacity);
	}
	/* Instance Methods */
	public ref Item this[int index] => ref this.Items[index];
	public void Add(Item item)
	{
		var freeIndex = this._Items.FindIndex(slot => slot.Id == Item.Empty);
		if (freeIndex >= 0)
			this.Items[freeIndex] = item;
		else
			this._Items.Add(item);
	}
	public void Clear()
	{
		foreach (ref var slot in this.Items)
		{
			slot = default;
			slot.Id = Item.Empty;
			slot.Count = 1;
		}
	}
	public void Remove(int index)
	{
		ref var slot = ref this.Items[index];
		slot = default;
		slot.Id = Item.Empty;
		slot.Count = 1;
	}
	private void _SetCapacity(int length)
	{
		// Grow
		if (length > this._Items.Count)
		{
			this._Items.EnsureCapacity(length);
			while (this._Items.Count < length)
				this._Items.Add(new() { Id=Item.Empty, Count=1 });
		}
		// Shrink
		else this._Items.RemoveRange(length, this._Items.Count - length);
	}
	/* Properties */
	public (uint X, uint Y) Position;
	internal readonly List<Item> _Items;
	public Span<Item> Items => CollectionsMarshal.AsSpan(this._Items);
	public int Capacity { get => this._Items.Count; set => this._SetCapacity(value); }
}
