/*
	Junk Jack X: Core
	- [Serialization]Converter - Item

	Written By: Ryan Smith
*/

using System;
using System.Runtime.InteropServices;

namespace JJx.Core.Serialization;

internal sealed class TileConverter : JJxConverter<Tile>
{
	/* Instance Methods */
	public override Tile Read(ref JJxReader reader)
	{
		var foreground = reader.ReadUInt16();
		var background = reader.ReadUInt16();
		Span<ushort> decorations = stackalloc ushort[4];
		reader.ReadSpan(MemoryMarshal.Cast<ushort, byte>(decorations));
		var tile = new Tile(foreground, background, decorations);
		reader.ReadSpan(tile.Unknown);
		return tile;
	}
	public override void Write(in Tile @value, JJxWriter writer)
	{
		writer.Write(@value.Foreground);
		writer.Write(@value.Background);
		writer.Write(MemoryMarshal.Cast<ushort, byte>(@value.Decorations));
		writer.Write(@value.Unknown);
	}
}
