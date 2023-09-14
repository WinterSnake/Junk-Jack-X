/*
	Junk Jack X: World
	- Tile

	Segment Breakdown:
	-----------------------------------------------------------------------------
	Segment[0x0 : 0x1] = Foreground  | Length: 2 (0x2) | Type: uint16
	Segment[0x2 : 0x3] = Background  | Length: 2 (0x2) | Type: uint16
	Segment[0x4 : 0xB] = Decorations | Length: 8 (0x8) | Type: uint16[4]
	Segment[0xC : 0xD] = UNKNOWN     | Length: 2 (0x2) | Type: uint16
	Segment[0xE : 0xF] = UNKNOWN     | Length: 2 (0x2) | Type: uint16
	-----------------------------------------------------------------------------
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
	public Tile(ushort foregroundId = 0x00, ushort backgroundId = 0x00)
	{
		this.ForegroundId = (ushort)(foregroundId ^ 0x8000);
		this.BackgroundId = backgroundId;
	}
	private Tile(ushort foregroundId, ushort backgroundId, ushort[] decorationIds, byte[] buffer)
	{
		this._ForegroundId = foregroundId;
		this.BackgroundId = backgroundId;
		this.DecorationIds = decorationIds;
		this._Block = buffer;
	}
	/* Instance Methods */
	public override string ToString()
	{
		//return $""
		var tile = $"Foreground: {this._ForegroundId:X4} | Background: {this.BackgroundId:X4} | Decorations: [";
		for (var i = 0; i < this.DecorationIds.Length; ++ i)
		{
			tile += $"{this.DecorationIds[i]:X4}";
			if (i < this.DecorationIds.Length - 1)
				tile += ", ";
			else
				tile += "]";
		}
		return tile;
	}
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
		return new Tile(foregroundId, backgroundId, decorationIds, workingData);
	}
	/* Properties */
	private ushort _ForegroundId;
	public ushort ForegroundId {
		get { return (ushort)(this._ForegroundId & 0x7FFF); }
		set { this._ForegroundId = (ushort)(value | 0x8000); }
	}
	public ushort BackgroundId;
	public readonly ushort[] DecorationIds = new ushort[COUNT_DECORATIONS];
	public byte[] _Block;
	/* Class Properties */
	internal const byte SIZE = 16;
	private const byte COUNT_DECORATIONS = 4;
}
