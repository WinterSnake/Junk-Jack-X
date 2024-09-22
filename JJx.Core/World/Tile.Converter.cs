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

namespace JJx.Serialization;

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
