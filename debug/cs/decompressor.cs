using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

internal static class Program
{
	/* Static Methods */
	private static async Task Main()
	{
		var bytesRead = 0;
		var compressedData = new byte[0x1AF1C];
		using var inputStream = File.Open("../debug.dat", FileMode.Open, FileAccess.Read);
		inputStream.Seek(0x5D0, SeekOrigin.Begin);
		bytesRead = await inputStream.ReadAsync(compressedData, 0, 0x1AF1C);
		Console.WriteLine($"Bytes read: {bytesRead.ToString("x6")}");
		using (var compressedFile = File.Open("debug.compressed", FileMode.Create, FileAccess.Write))
		{
			await compressedFile.WriteAsync(compressedData, 0, compressedData.Length);
		}
		Console.WriteLine($"Position: 0x{inputStream.Position.ToString("x8")} | Next Byte: {inputStream.ReadByte().ToString("x2")}");
		inputStream.Seek(0x5D0, SeekOrigin.Begin);
		var decompressStream = new GZipStream(inputStream, CompressionMode.Decompress);
		var blocks = new List<Block>();
		for (var i = 0; i < (0x100000 / 16); ++i)
		{
			var block = await Block.FromStream(decompressStream);
			blocks.Add(block);
		}
		using (var decompressedFile = File.Open("debug.decompressed", FileMode.Create, FileAccess.Write))
		{
			foreach (var block in blocks)
			{
				await block.ToStream(decompressedFile);
			}
		}
		Console.WriteLine($"Position: 0x{inputStream.Position.ToString("x8")} | Next Byte: {inputStream.ReadByte().ToString("x2")}");
		inputStream.Seek(0x5D0 + compressedData.Length, SeekOrigin.Begin);
		Console.WriteLine($"Position: 0x{inputStream.Position.ToString("x8")} | Next Byte: {inputStream.ReadByte().ToString("x2")}");

		var memStream = new MemoryStream();
		foreach (var block in blocks)
		{
			await memStream.WriteAsync(block.Data, 0, block.Data.Length);
		}
		memStream.Seek(0, SeekOrigin.Begin);
		using (var recompressedFile = File.Open("debug.recompressed", FileMode.Create, FileAccess.Write))
		{
			using var compressStream = new GZipStream(recompressedFile, CompressionMode.Compress);
			await memStream.CopyToAsync(compressStream);
		}
	}
}

public class Block
{
	/* Constructors */
	public Block(byte[] data)
	{
		this.Data = data;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		await stream.WriteAsync(this.Data, 0, this.Data.Length);
		await stream.FlushAsync();
	}
	/* Static Methods */
	public static async Task<Block> FromStream(Stream stream)
	{
		var bytesRead = 0;
		var workingData = new byte[SIZE];
		while (bytesRead <  SIZE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZE - bytesRead);
		return new Block(workingData);
	}
	/* Properties */
	public readonly byte[] Data;
	/* Class Properties */
	private const byte SIZE = 16;
}
