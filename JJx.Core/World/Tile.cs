/*
	Junk Jack X: Core
	- [World]Tile

	Segment Breakdown:
	-----------------------------------------------------------------------
	Segment[0x0 : 0x1] = Foreground Id  | Length: 2 (0x2) | Type: uint16
	Segment[0x2 : 0x3] = Background Id  | Length: 2 (0x2) | Type: uint16
	Segment[0x4 : 0xB] = Decoration Ids | Length: 8 (0x8) | Type: uint16[4]
	Segment[0xC : 0xF] = UNKNOWN        | Length: 4 (0x4) | Type: ???
	-----------------------------------------------------------------------
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
	}
	private Tile(ushort foregroundId, ushort backgroundId, ushort[] decorationIds, byte[] data)
	{
		this.ForegroundId = foregroundId;
		this.BackgroundId = backgroundId;
		this._DecorationIds = decorationIds;
		this.Data = data;
	}
	/* Instance Methods */
	public (ushort Id, bool IsBackground) GetDecoration(int index)
	{
		if (index > this._DecorationIds.Length - 1) throw new ArgumentOutOfRangeException();
		var id = this._DecorationIds[index];
		var isBackground = id > FOREGROUND_FLAG ? true : false;
		return (isBackground ? (ushort)(id & FOREGROUND_FLAG) : id, isBackground);
	}
	public void SetDecoration(int index, ushort id, bool background)
	{
		if (index > this._DecorationIds.Length - 1) throw new ArgumentOutOfRangeException();
		if (background)
			id |= (FOREGROUND_FLAG + 1);
		this._DecorationIds[index] = id;
	}
	public async Task ToStream(Stream stream)
	{
		var buffer = new byte[SIZE];
		BitConverter.LittleEndian.Write((ushort)(this.ForegroundId | (FOREGROUND_FLAG + 1)), buffer, OFFSET_FOREGROUND);
		BitConverter.LittleEndian.Write(this.BackgroundId, buffer, OFFSET_BACKGROUND);
		for (var i = 0; i < this._DecorationIds.Length; ++i)
			BitConverter.LittleEndian.Write(this._DecorationIds[i], buffer, OFFSET_DECORATION + i * sizeof(ushort));
		// UNKNOWN(4)
		Array.Copy(this.Data, 0xC, buffer, 0xC, 4);
		await stream.WriteAsync(buffer, 0, buffer.Length);
	}
	/* Static Methods */
	public static async Task<Tile> FromStream(Stream stream)
	{
		var bytesRead = 0;
		var buffer = new byte[SIZE];
		while (bytesRead < buffer.Length)
			bytesRead += await stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
		var foregroundId = (ushort)(BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_FOREGROUND) & FOREGROUND_FLAG);
		var backgroundId = BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_BACKGROUND);
		var decorationIds = new ushort[DECORATION_COUNT];
		for (var i = 0; i < decorationIds.Length; ++i)
			decorationIds[i] = BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_DECORATION + i * sizeof(ushort));
		// UNKNOWN(4)
		return new Tile(foregroundId, backgroundId, decorationIds, buffer);
	}
	/* Properties */
	public ushort ForegroundId;
	public ushort BackgroundId;
	private readonly ushort[] _DecorationIds = new ushort[DECORATION_COUNT];
	public readonly byte[] Data;
	/* Class Properties */
	public const byte DECORATION_COUNT   =  4;
	internal const byte SIZE             = 16;
	private const byte OFFSET_FOREGROUND =  0;
	private const byte OFFSET_BACKGROUND =  2;
	private const byte OFFSET_DECORATION =  4;
	private const ushort FOREGROUND_FLAG = 0x7FFF;
}
