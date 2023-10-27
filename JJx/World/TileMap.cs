/*
	Junk Jack X: World
	- Tile Map

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
	public TileMap((ushort width, ushort height) size)
	{
		this.Tiles = new Tile[size.width, size.height];
	}
	private TileMap(Tile[,] tiles)
	{
		this.Tiles = tiles;
	}
	/* Instance Methods */
	public async Task Save(string filePath, bool compressed = true)
	{
		await Task.Delay(1000);
	}
	public async Task ToStream(Stream stream, bool compressed = true)
	{
		var blocksDecompressedStream = new MemoryStream(this.Height * this.Width * Tile.SIZE);
		for (var x = 0; x < this.Width; ++x)
			for (var y = 0; y < this.Height; ++y)
				await this.Tiles[x, y].ToStream(blocksDecompressedStream);
		blocksDecompressedStream.Position = 0;
		if (compressed)
		{
			using var blockCompressedStream = new GZipStream(stream, CompressionLevel.Optimal, true);
			await blocksDecompressedStream.CopyToAsync(blockCompressedStream);
		}
		else
			await blocksDecompressedStream.CopyToAsync(stream);
	}
	/* Static Methods */
	public static async Task<TileMap> Load(string filePath)
	{
		await Task.Delay(1000);
		return new TileMap((1, 1));
	}
	public static async Task<TileMap> FromStream(
		Stream stream, (ushort width, ushort height) size, bool compressed = true
	)
	{
		Tile[,] tiles = new Tile[size.width, size.height];
		if (compressed)
			stream = new GZipStream(stream, CompressionMode.Decompress);
		for (var x = 0; x < size.width; ++x)
			for (var y = 0; y < size.height; ++y)
				tiles[x, y] = await Tile.FromStream(stream);
		return new TileMap(tiles);
	}
	/* Properties */
	public Tile[,] Tiles { get; internal set; }
	public ushort Height { get { return (ushort)this.Tiles.GetLength(1); }}
	public ushort Width  { get { return (ushort)this.Tiles.GetLength(0); }}
}
