/*
	Junk Jack X: Core
	- [World]Tilemap

	Written By: Ryan Smith
*/

using System;
using System.Buffers;
using System.Runtime.InteropServices;
using CommunityToolkit.HighPerformance.Buffers;
using JJx.Core.Serialization;

namespace JJx.Core;

public sealed class Tilemap
{
	/* Constructor */
	public Tilemap(Tile[,] tiles) => this._Tiles = tiles;
	/* Instance Methods */
	public ref Tile this[ushort x, ushort y] => ref this._Tiles[x, y];
	public Span<Tile> GetColumn(ushort x)
	{
		ref var tile = ref this._Tiles[x, 0];
		return MemoryMarshal.CreateSpan(ref tile, this.Size.Height);
	}
	/* Static Methods */
	public static Tilemap Deserialize(ReadOnlySpan<byte> buffer, (ushort width, ushort height) size)
	{
		var tiles = new Tile[size.width, size.height];
		var reader = new JJxReader(buffer);
		for (var x = 0; x < size.width; ++x)
			for (var y = 0; y < size.height; ++y)
				tiles[x, y] = reader.ReadObject<Tile>(JJxSerializationOptions.Default);
		return new(tiles);
	}
	public static IMemoryOwner<byte> Serialize(Tilemap tilemap, out ReadOnlyMemory<byte> buffer)
	{
		var owner = MemoryPool<byte>.Shared.Rent(tilemap.Size.Width * tilemap.Size.Height * Tile.SIZE);
		var memoryWriter = new MemoryBufferWriter<byte>(owner.Memory);
		var writer = new JJxWriter(memoryWriter);
		for (var x = 0; x < tilemap.Size.Width; ++x)
			for (var y = 0; y < tilemap.Size.Height; ++y)
				writer.Write(tilemap._Tiles[x, y], JJxSerializationOptions.Default);
		buffer = memoryWriter.WrittenMemory;
		return owner;
	}
	/* Properties */
	public (ushort Width, ushort Height) Size => ((ushort)this._Tiles.GetLength(0), (ushort)this._Tiles.GetLength(1));
	public int Length => this._Tiles.Length;
	private readonly Tile[,] _Tiles;
}
