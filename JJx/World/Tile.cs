/*
	Junk Jack X: World
	- Tile

	Segment Breakdown:
	--------------------------------------------------------------------
	Segment[0x0 : 0x1] = Foreground  | Length: 2 (0x2) | Type: uint16
	Segment[0x2 : 0x3] = Background  | Length: 2 (0x2) | Type: uint16
	Segment[0x4 : 0xB] = Decorations | Length: 8 (0x8) | Type: uint16[4]
	Segment[0xC : 0xD] = UNKNOWN     | Length: 2 (0x2) | Type: uint16
	Segment[0xE : 0xF] = UNKNOWN     | Length: 2 (0x2) | Type: uint16
	--------------------------------------------------------------------
	Size: 16 (0x10)

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Tile
{
	/* Constructors */
	public Tile(ushort foregroundId, ushort backgroundId)
	{
		this.ForegroundId = foregroundId;
		this.BackgroundId = backgroundId;
		for (var i = 0; i < this.DecorationIds.Length; ++i)
			this.DecorationIds[i] = 0x0000;
		this.CircuitryId  = 0x0000;
		this.FluidId      = 0x0000;
	}
	private Tile(
		ushort foregroundId, ushort backgroundId, ushort[] decorationIds,
		ushort circuitryId, ushort fluidId
	)
	{
		this.ForegroundId = foregroundId;
		this.BackgroundId = backgroundId;
		this.DecorationIds = decorationIds;
		this.CircuitryId = circuitryId;
		this.FluidId = fluidId;
	}
	/* Instance Methods */
	public override string ToString()
	{
		return $"Foreground: 0x{this.ForegroundId:X4} | Background: 0x{this.BackgroundId:X4} | Circuitry: 0x{this.CircuitryId:X4} | Fluid: 0x{this.FluidId:X4}";
	}
	public async Task ToStream(Stream stream, bool compressed = true)
	{
		var workingData = new byte[SIZE];
		BitConverter.Write(workingData, this.ForegroundId, 0);
		BitConverter.Write(workingData, this.BackgroundId, 2);
		for (var i = 0; i < this.DecorationIds.Length; ++i)
			BitConverter.Write(workingData, this.DecorationIds[i], 4 + i * 2);
		BitConverter.Write(workingData, this.CircuitryId, 12);
		BitConverter.Write(workingData, this.FluidId, 14);
		await stream.WriteAsync(workingData, 0, workingData.Length);
	}
	/* Static Methods */
	public static async Task<Tile> FromStream(Stream stream)
	{
		int bytesRead = 0;
		var workingData = new byte[SIZE];
		while (bytesRead < SIZE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZE - bytesRead);
		var foregroundId = BitConverter.GetUInt16(workingData, 0);
		var backgroundId = BitConverter.GetUInt16(workingData, 2);
		var decorationIds = new ushort[COUNTOF_DECORATIONS];
		for (var i = 0; i < decorationIds.Length; ++i)
			decorationIds[i] = BitConverter.GetUInt16(workingData, 4 + i * 2);
		var circuitId = BitConverter.GetUInt16(workingData, 12);
		var fluidId   = BitConverter.GetUInt16(workingData, 14);
		return new Tile(foregroundId, backgroundId, decorationIds, circuitId, fluidId);
	}
	/* Properties */
	public ushort BackgroundId;
	public ushort ForegroundId;
	public readonly ushort[] DecorationIds = new ushort[COUNTOF_DECORATIONS];
	public ushort CircuitryId;
	public ushort FluidId;
	/* Class Properties */
	internal const byte SIZE = 16;
	private const byte COUNTOF_DECORATIONS = 4;
}
