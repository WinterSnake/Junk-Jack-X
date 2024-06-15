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

public sealed partial class Tile
{
	/* Constructors */
	public Tile(ushort foregroundId, ushort backgroundId)
	{
		this.Foreground = Tile.Block.Unpack(foregroundId);
		this.Background = Tile.Block.Unpack(backgroundId);
		for (var i = 0; i < this.Decorations.Length; ++i)
			this.Decorations[i] = new(0x0000);
	}
	private Tile(Block foreground, Block background, Decoration[] decorations)
	{
		this.Foreground = foreground;
		this.Background = background;
		this.Decorations = decorations;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var buffer = new byte[SIZE];
		BitConverter.LittleEndian.Write((ushort)(this.Foreground.Pack() | FOREGROUND_FLAG), buffer, OFFSET_FOREGROUND);
		BitConverter.LittleEndian.Write(this.Background.Pack(), buffer, OFFSET_BACKGROUND);
		for (var i = 0; i < this.Decorations.Length; ++i)
			BitConverter.LittleEndian.Write(this.Decorations[i].Pack(), buffer, OFFSET_DECORATION + i * sizeof(ushort));
		// -UNKNOWN(4)- \\
		await stream.WriteAsync(buffer, 0, buffer.Length);
	}
	/* Static Methods */
	public static async Task<Tile> FromStream(Stream stream)
	{
		var bytesRead = 0;
		var buffer = new byte[SIZE];
		while (bytesRead < buffer.Length)
			bytesRead += await stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
		var foregroundId = (ushort)(BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_FOREGROUND) ^ FOREGROUND_FLAG);
		var foreground = Tile.Block.Unpack(foregroundId);
		var backgroundId = BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_BACKGROUND);
		var background = Tile.Block.Unpack(backgroundId);
		var decorations = new Tile.Decoration[SIZEOF_DECORATION];
		for (var i = 0; i < decorations.Length; ++i)
		{
			var decorationId = BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_DECORATION + i * sizeof(ushort));
			decorations[i] = Tile.Decoration.Unpack(decorationId);
		}
		// -UNKNOWN(4)- \\
		return new(foreground, background, decorations);
	}
	/* Properties */
	public Tile.Block Foreground;
	public Tile.Block Background;
	public Tile.Decoration[] Decorations = new Decoration[SIZEOF_DECORATION];
	/* Class Properties */
	internal const byte SIZE             = 16;
	private const byte SIZEOF_DECORATION =  4;
	private const byte OFFSET_FOREGROUND =  0;
	private const byte OFFSET_BACKGROUND =  2;
	private const byte OFFSET_DECORATION =  4;
	private const ushort FOREGROUND_FLAG = 0x8000;
}
