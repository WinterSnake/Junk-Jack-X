/*
	Junk Jack X: Core
	- [World]TileMap

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Diagnostics;
using JJx.Serialization;

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

// Converters
internal sealed class TileMapConverterFactory : JJxConverterFactory
{
	/* Instance Methods */
	#nullable enable
	public override JJxConverter Build(Type type, params object?[]? properties)
	{
		Debug.Assert(
			properties != null && properties!.Length == 2,
			"TileMapConverterFactory.Build expects width and height to be passed in"
		);
		ushort width = (ushort)properties![0]!;
		ushort height = (ushort)properties![1]!;
		#if DEBUG
			Console.WriteLine($"Creating new TileMapConverter with size ({width}, {height})");
		#endif
		return new TileMapConverter(width, height);
	}
	#nullable disable
	public override bool CanConvert(Type type) => type == typeof(TileMap);
	/* Properties */
	public override bool Cache => false;
}

internal sealed class TileMapConverter : JJxConverter<TileMap>
{
	/* Constructor */
	public TileMapConverter(ushort width, ushort height) { this.Size = (width, height); }
	/* Instance Methods */
	public override TileMap Read(JJxReader reader)
	{
		var tiles = new Tile[this.Size.Width, this.Size.Height];
		for (var x = 0; x < this.Size.Width; ++x)
			for (var y = 0; y < this.Size.Height; ++y)
				tiles[x,y] = reader.Get<Tile>();
		return new TileMap(tiles);
	}
	public override void Write(JJxWriter writer, object @value) => throw new NotImplementedException($"TileMapConverter does not implement a Write");
	/* Properties */
	private (ushort Width, ushort Height) Size;
}
