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
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Tile
{
	/* Constructors */
	public Tile(ushort foregroundId = 0x0000, ushort backgroundId = 0x0000)
	{
		this.ForegroundId = foregroundId;
		this.BackgroundId = backgroundId;
		for (var i = 0; i < this.DecorationIds.Length; ++i)
			this.DecorationIds[i] = 0x0000;
	}
	private Tile(ushort foregroundId, ushort backgroundId, ushort[] decorationIds, ushort unknownId0, ushort unknownId1)
	{
		this._ForegroundId = foregroundId;
		this.BackgroundId = backgroundId;
		this.DecorationIds = decorationIds;
		this.UnknownTilePart0 = unknownId0;
		this.UnknownTilePart1 = unknownId1;
	}
	/* Instance Methods */
	public (ushort id, bool background) GetDecoration(byte index)
	{
		var id = this.DecorationIds[index];
		var background = Convert.ToBoolean(id & 0x8000);
		return (background ? (ushort)(id ^ 0x8000) : id, background);
	}
	public void SetDecoration(byte index, ushort id, bool background = false)
	{
		if (background)
			id ^= 0x8000;
		this.DecorationIds[index] = id;
	}
	public async Task ToStream(Stream stream)
	{
		var workingData = new byte[SIZE];
		Utilities.ByteConverter.Write(new Span<byte>(workingData), this._ForegroundId, 0);
		Utilities.ByteConverter.Write(new Span<byte>(workingData), this.BackgroundId,  2);
		for (var i = 0; i < this.DecorationIds.Length; ++i)
			Utilities.ByteConverter.Write(new Span<byte>(workingData), this.DecorationIds[i],  4 + i * 2);
		Utilities.ByteConverter.Write(new Span<byte>(workingData), this.UnknownTilePart0, 12);
		Utilities.ByteConverter.Write(new Span<byte>(workingData), this.UnknownTilePart1, 14);
		await stream.WriteAsync(workingData, 0, workingData.Length);
	}
	/* Static Methods */
	public static async Task<Tile> FromStream(Stream stream)
	{
		var bytesRead = 0;
		var workingData = new byte[SIZE];
		while (bytesRead < SIZE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZE - bytesRead);
		var foregroundId = Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData), 0);
		var backgroundId = Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData), 2);
		var decorationIds = new ushort[COUNT_DECORATIONS];
		for (var i = 0; i < decorationIds.Length; ++i)
			decorationIds[i] = Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData), 4 + i * 2);
		var unknownId0 = Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData), 12);
		var unknownId1 = Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData), 14);
		return new Tile(foregroundId, backgroundId, decorationIds, unknownId0, unknownId1);
	}
	/* Properties */
	private ushort _ForegroundId;
	public ushort ForegroundId {
		get { return (ushort)(this._ForegroundId & 0x7FFF); }
		set { this._ForegroundId = (ushort)(value | 0x8000); }
	}
	public ushort BackgroundId;
	public readonly ushort[] DecorationIds = new ushort[COUNT_DECORATIONS];
	public ushort UnknownTilePart0 = 0x0000;
	public ushort UnknownTilePart1 = 0x0000;
	/* Class Properties */
	internal const byte SIZE = 16;
	private const byte COUNT_DECORATIONS = 4;
}
