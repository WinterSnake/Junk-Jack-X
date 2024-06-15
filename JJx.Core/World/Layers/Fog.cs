/*
	Junk Jack X: Core
	- [World: Layers]Fog

	Segment Breakdown:
	------------------------------------------------------------------
	------------------------------------------------------------------

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace JJx;

public sealed class FogLayer
{
	/* Constructors */
	private FogLayer(byte[] data) { this.Data = data; }
	/* Instance Methods */
	public async Task ToStream(Stream stream, bool compressed = true)
	{
		using var fogStream = new MemoryStream(this.Data);
		if (compressed)
			using (var compressedStream = new GZipStream(stream, CompressionLevel.Optimal, true))
				await fogStream.CopyToAsync(compressedStream);
		else
			await fogStream.CopyToAsync(stream);
	}
	/* Static Methods */
	public static async Task<FogLayer> FromStream(Stream stream, bool compressed = true)
	{
		if (compressed)
			stream = new GZipStream(stream, CompressionMode.Decompress, false);
		using (var bufferStream = new MemoryStream())
		{
			await stream.CopyToAsync(bufferStream);
			return new FogLayer(bufferStream.GetBuffer());
		}
	}
	/* Properties */
	public readonly byte[] Data;
}
