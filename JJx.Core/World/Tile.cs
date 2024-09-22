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
using System.Diagnostics;
using JJx.Serialization;

namespace JJx;

public sealed partial class Tile
{
	/* Constructors */
	public Tile(ushort foregroundId, ushort backgroundId)
	{
		this.Foreground = new Tile.Block(foregroundId);
		this.Background = new Tile.Block(backgroundId);
		for (var i = 0; i < this.Decorations.Length; ++i)
			this.Decorations[i] = new Decoration(0x0000);
	}
	internal Tile(Tile.Block foreground, Tile.Block background, Tile.Decoration[] decorations)
	{
		Debug.Assert(decorations.Length == SIZEOF_DECORATIONS, $"Tile() expects {SIZEOF_DECORATIONS} decorations, received: {decorations.Length}");
		this.Foreground = foreground;
		this.Background = background;
		this.Decorations = decorations;
	}
	/* Instance Methods */
	public override string ToString()
	{
		var decorations = new string[this.Decorations.Length];
		for (var i = 0; i < decorations.Length; ++i)
			decorations[i] = $"0x{this.Decorations[i].Packed:X4}";
		var output = $"Foreground: 0x{this.Foreground.Packed:X4} ; Background: 0x{this.Background.Packed:X4} ; ";
		output += '[' + String.Join(',', decorations) + ']';
		return output;
	}
	/* Properties */
	public Tile.Block Foreground;
	public Tile.Block Background;
	public Tile.Decoration[] Decorations = new Tile.Decoration[SIZEOF_DECORATIONS];
	/* Class Properties */
	internal const byte SIZEOF_DECORATIONS = 4;
}

// Converters
internal sealed class TileConverter : JJxConverter<Tile>
{
	/* Instance Methods */
	public override Tile Read(JJxReader reader)
	{
		var foregroundId = (ushort)(reader.GetUInt16() ^ FOREGROUND_FLAG);
		var backgroundId = reader.GetUInt16();
		var decorations = new Tile.Decoration[Tile.SIZEOF_DECORATIONS];
		for (var i = 0; i < decorations.Length; ++i)
		{
			var decorationId = reader.GetUInt16();
			decorations[i] = new Tile.Decoration(decorationId);
		}
		// -UNKNOWN(4)- \\
		reader.GetBytes(4);
		return new Tile(new Tile.Block(foregroundId), new Tile.Block(backgroundId), decorations);
	}
	/* Properties */
	/* Class Properties */
	private const ushort FOREGROUND_FLAG = 0x8000;
}
