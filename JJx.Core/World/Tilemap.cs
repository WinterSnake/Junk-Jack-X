/*
	Junk Jack X: Core
	- [World]Tilemap

	Written By: Ryan Smith
*/

using System;

namespace JJx.Core;

public sealed class Tilemap
{
	/* Constructor */
	public Tilemap(Tile[] tiles, (ushort, ushort) size)
	{
		this.Size = size;
		this._Tiles = tiles;
	}
	/* Instance Methods */
	public ref Tile this[int index] => ref this._Tiles[index];
	public ref Tile this[ushort x, ushort y] => ref this._Tiles[x * this.Size.Height + y];
	public Span<Tile> GetColumn(ushort x) => this._Tiles.AsSpan(x * this.Size.Height, this.Size.Height);
	/* Properties */
	public readonly (ushort Width, ushort Height) Size;
	public int Length => this._Tiles.Length;
	private readonly Tile[] _Tiles;
	public Span<Tile> Tiles => this._Tiles.AsSpan();
}
