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
	public TileMap(Tile[,] tiles)
	{
		this.Tiles = tiles;
	}
	public TileMap((ushort width, ushort height) size)
	{
		this.Tiles = new Tile[size.width, size.height];
		for (var x = 0; x < this.Width; ++x)
		{
			for (var y = 0; y < this.Height; ++y)
			{
				if (y == 0)
					this.Tiles[x, y] = new Tile(0x8054, 0x0054);
				else
					this.Tiles[x, y] = new Tile(0x8000, 0x0000);
			}
		}
	}
	/* Instance Methods */
	public async Task Save(string filePath, bool compressed = true)
	{
		var workingData = new byte[SIZE];
		using var writer = new FileStream(filePath, FileMode.Create);
		BitConverter.Write(workingData, this.Width,  0);
		BitConverter.Write(workingData, this.Height, 2);
		BitConverter.Write(workingData, compressed,  4);
		await writer.WriteAsync(workingData, 0, workingData.Length);
		await this.ToStream(writer, compressed);
	}
	public async Task ToStream(Stream stream, bool compressed = true)
	{
		var blocksDecompressedStream = new MemoryStream(this.Height * this.Width * Tile.SIZE);
		for (var x = 0; x < this.Width; ++x)
			for (var y = 0; y < this.Height; ++y)
				await this.Tiles[x, y].ToStream(blocksDecompressedStream);
		blocksDecompressedStream.Position = 0;
		if (compressed)
			using (var blockCompressedStream = new GZipStream(stream, CompressionLevel.Optimal, true))
				await blocksDecompressedStream.CopyToAsync(blockCompressedStream);
		else
			await blocksDecompressedStream.CopyToAsync(stream);
	}
	/* Static Methods */
	public static async Task<TileMap> Load(string filePath)
	{
		int bytesRead = 0;
		var workingData = new byte[SIZE];
		using var reader = new FileStream(filePath, FileMode.Open);
		while(bytesRead < workingData.Length)
			bytesRead += await reader.ReadAsync(workingData, bytesRead, workingData.Length - bytesRead);
		var size = (
			BitConverter.GetUInt16(workingData, 0),
			BitConverter.GetUInt16(workingData, 2)
		);
		var compressed = BitConverter.GetBool(workingData, 4);
		return await TileMap.FromStream(reader, size, compressed);
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
	/* Class Properties */
	private const byte SIZE = 5;
}
