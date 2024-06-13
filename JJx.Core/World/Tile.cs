/*
	Junk Jack X: Core
	- [World]Tile

	Segment Breakdown:
	--------------------------------------------------------------------
	Segment[0x0 : 0x1] = ForegroundId | Length: 2  (0x2) | Type: uint16
	Segment[0x2 : 0x3] = BackgroundId | Length: 2  (0x2) | Type: uint16
	Segment[0x4 : 0xF] = UNKNOWN      | Length: 12 (0xC) | Type: ???
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
	}
	private Tile(ushort foregroundId, ushort backgroundId, byte[] data)
	{
		this.ForegroundId = foregroundId;
		this.BackgroundId = backgroundId;
		this.Data = data;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var buffer = new byte[SIZE];
		BitConverter.LittleEndian.Write((ushort)(this.ForegroundId & FOREGROUND_FLAG), buffer, OFFSET_FOREGROUND);
		BitConverter.LittleEndian.Write(this.BackgroundId, buffer, OFFSET_BACKGROUND);
		Array.Copy(this.Data, OFFSET_BACKGROUND + sizeof(ushort), buffer, OFFSET_BACKGROUND + sizeof(ushort), SIZE - (sizeof(ushort) * 2));
		await stream.WriteAsync(buffer, 0, buffer.Length);
	}
	/* Static Methods */
	public static async Task<Tile> FromStream(Stream stream)
	{
		var bytesRead = 0;
		var buffer = new byte[SIZE];
		while (bytesRead < buffer.Length)
			bytesRead += await stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
		var foregroundId = (ushort)(BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_FOREGROUND) & ~FOREGROUND_FLAG);
		var backgroundId = BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_BACKGROUND);
		return new Tile(foregroundId, backgroundId, buffer);
	}
	/* Properties */
	public ushort ForegroundId;
	public ushort BackgroundId;
	public readonly byte[] Data = new byte[SIZE];
	/* Class Properties */
	internal const byte SIZE = 16;
	private const byte OFFSET_FOREGROUND = 0;
	private const byte OFFSET_BACKGROUND = 2;
	private const ushort FOREGROUND_FLAG  = 0x8000;
}
