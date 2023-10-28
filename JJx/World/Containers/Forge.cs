/*
	Junk Jack X: World
	- Container: Forge

	Segment Breakdown:
	---------------------------------------------------------------------------
	Segment[0x0 : 0x1] = X Position            | Length: 2 (0x2) | Type: uint16
	Segment[0x2 : 0x3] = Y Position            | Length: 2 (0x2) | Type: uint16
	Segment[0x4 : 0x5] = Fuel Ticks Remaining  | Length: 2 (0x2) | Type: uint16
	Segment[0x6 : 0x7] = Smelt Ticks Remaining | Length: 2 (0x2) | Type: uint16
	Segment[0x8 : 0x9] = Fuel Ticks Init       | Length: 2 (0x2) | Type: uint16
	Segment[0xA : 0xB] = Smelt Ticks Init      | Length: 2 (0x2) | Type: uint16
	Segment[0xC]       = Ticking               | Length: 2 (0x2) | Type: bool
	Segment[0xD : 0xF] = UNKNOWN               | Length: 3 (0xv) | Type: ???
	---------------------------------------------------------------------------
	Size: 16 (0x10) + 36 (0x24) {Item[3]}

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JJx;

public sealed class Forge
{
	/* Constructors */
	public Forge((ushort, ushort) position)
	{
		this.Position = position;
		for (var i = 0; i < this.Items.Length; ++i)
			this.Items[i] = new Item(0xFFFF, 0);
	}
	private Forge(
		(ushort, ushort) position, ushort ticksInitFuel, ushort ticksInitSmelt,
		ushort ticksRemainingFuel, ushort ticksRemainingSmelt,bool ticking,
		Item[] items
	)
	{
		this.Position = position;
		this.TicksInitFuel = ticksInitFuel;
		this.TicksRemainingFuel = ticksRemainingFuel;
		this.TicksInitSmelt = ticksInitSmelt;
		this.TicksRemainingSmelt = ticksRemainingSmelt;
		this.Ticking = ticking;
		this.Items = items;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var workingData = new byte[SIZE];
		// Position
		BitConverter.Write(workingData, this.Position.X, 0);
		BitConverter.Write(workingData, this.Position.Y, 2);
		// Status
		BitConverter.Write(workingData, this.TicksRemainingFuel,  4);
		BitConverter.Write(workingData, this.TicksRemainingSmelt, 6);
		BitConverter.Write(workingData, this.TicksInitFuel,       8);
		BitConverter.Write(workingData, this.TicksInitSmelt,     10);
		BitConverter.Write(workingData, this.Ticking,            12);
		await stream.WriteAsync(workingData, 0, workingData.Length);
		// Items
		foreach (var item in this.Items)
			await item.ToStream(stream);
	}
	/* Static Methods */
	public static async Task<Forge> FromStream(Stream stream)
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
		// Status
		var ticksRemainingFuel  = BitConverter.GetUInt16(workingData,  4);
		var ticksRemainingSmelt = BitConverter.GetUInt16(workingData,  6);
		var ticksInitFuel       = BitConverter.GetUInt16(workingData,  8);
		var ticksInitSmelt      = BitConverter.GetUInt16(workingData, 10);
		var ticking             = BitConverter.GetBool(workingData,   12);
		// -UNKNOWN
		//var statusData = String.Join(", ", workingData.TakeLast(3).Select(x => $"0x{x:X2}"));
		//Console.WriteLine($"Status: {statusData}");
		// Items
		var items = new Item[COUNTOF_ITEMS];
		for (var i = 0; i < items.Length; ++i)
			items[i] = await Item.FromStream(stream);
		return new Forge(
			position, ticksInitFuel, ticksInitSmelt, ticksRemainingFuel, ticksRemainingSmelt,
			ticking, items
		);
	}
	/* Properties */
	public (ushort X, ushort Y) Position;
	public bool   Ticking = false;
	public ushort TicksInitFuel = 0;
	public ushort TicksInitSmelt = 0;
	public ushort TicksRemainingFuel = 0;
	public ushort TicksRemainingSmelt = 0;
	public readonly Item[] Items = new Item[COUNTOF_ITEMS];
	/* Class Properties */
	private const byte SIZE          = 16;
	private const byte COUNTOF_ITEMS =  3;
}
