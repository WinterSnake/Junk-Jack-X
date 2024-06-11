/*
	Junk Jack X: Core
	- [World]TileMap

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace JJx;

public sealed class TileMap
{
	/* Constructors */
	public TileMap(ushort width, ushort height)
	{
		this.Tiles = new Tile[width, height];
		for (var x = 0; x < width; ++x)
		{
			for (var y = 0; y < height; ++y)
			{
				if (y == 0)
					this.Tiles[x, y] = new Tile();
				else
					this.Tiles[x, y] = new Tile();
			}
		}
	}
	public TileMap(Tile[,] tiles)
	{
		this.Tiles = tiles;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream, bool compressed = true)
	{

	}
	/* Static Methods */
	public static async Task<TileMap> FromStream(Stream stream, (ushort width, ushort height) size, bool compressed)
	{
		Tile[,] tiles = new Tile[size.width, size.height];
		// Decompress stream
		if (compressed)
			stream = new GZipStream(stream, CompressionMode.Decompress, true);
		// Iterate stream
		for (var x = 0; x < tiles.GetLength(0); ++x)
			for (var y = 0; y < tiles.GetLength(1); ++y)
				tiles[x, y] = await Tile.FromStream(stream);
		return new TileMap(tiles);
	}
	/* Properties */
	public readonly Tile[,] Tiles;  // TODO: Make resizeable
}
