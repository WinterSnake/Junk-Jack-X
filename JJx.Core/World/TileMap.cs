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
	public TileMap(Tile[,] tiles) { this.Tiles = tiles; }
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
	/* Instance Methods */
	public async Task ToStream(Stream stream, bool compressed = true)
	{
		var width = this.Tiles.GetLength(0);
		var height = this.Tiles.GetLength(1);
		var buffer = new byte[width * height * Tile.SIZE];
		using (var blockStream = new MemoryStream(buffer))
		{
			// Copy blocks to memory stream
			for (var x = 0; x < width; ++x)
				for (var y = 0; y < height; ++y)
					await this.Tiles[x, y].ToStream(blockStream);
			// Reset position
			blockStream.Position = 0;
			// Copy to output (compress if flagged)
			if (compressed)
				using (var compressedStream = new GZipStream(stream, CompressionLevel.Optimal, true))
					await blockStream.CopyToAsync(compressedStream);
			else
				await blockStream.CopyToAsync(stream);
		}
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
