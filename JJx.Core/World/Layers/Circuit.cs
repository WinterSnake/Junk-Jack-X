/*
	Junk Jack X: Core
	- [World: Layers]Circuit

	Segment Breakdown:
	------------------------------------------------------------------
	------------------------------------------------------------------

	Written By: Ryan Smith
*/
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class CircuitLayer
{
	/* Constructors */
	public CircuitLayer() { }
	private CircuitLayer(byte[] data) { this.Data = data; }
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		await stream.WriteAsync(this.Data, 0, this.Data.Length);
	}
	/* Static Methods */
	public static async Task<CircuitLayer> FromStream(ArchiverStream stream)
	{
		var layerSize = stream.GetChunkSize(ArchiverChunkType.WorldCircuitry);
		var bytesRead = 0;
		var buffer = new byte[layerSize];
		while (bytesRead < buffer.Length)
			bytesRead += await stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
		return new CircuitLayer(buffer);
	}
	/* Properties */
	public readonly byte[] Data = new byte[SIZE];
	/* Class Properties */
	private const byte SIZE = 16;
}
