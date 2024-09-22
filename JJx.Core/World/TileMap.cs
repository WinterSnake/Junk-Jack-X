/*
	Junk Jack X: Core
	- [World]TileMap

	Written By: Ryan Smith
*/
using System.IO;

namespace JJx;

public sealed class TileMap
{
	/* Constructors */
	public TileMap(ushort width, ushort height)
	{
		this.Tiles = new Tile[width, height];
		for (var x = 0; x < width; ++x)
			for (var y = 0; y < height; ++y)
				this.Tiles[x,y] = new Tile(0x0000, 0x0000);
	}
	internal TileMap(Tile[,] tiles) { this.Tiles = tiles; }
	/* Instance Methods */
	public void Save(string filePath, bool compressed = true)
	{

	}
	public void ToStream(Stream stream, bool compressed = true)
	{

	}
	/* Static Methods */
	public TileMap Load(string filePath)
	{
		return default;
	}
	public TileMap FromStream(Stream stream)
	{
		return default;
	}
	/* Properties */
	public Tile[,] Tiles { get; internal set; }
}
