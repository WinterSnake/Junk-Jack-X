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
		await stream.WriteAsync(this.Data, 0, this.Data.Length);
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
