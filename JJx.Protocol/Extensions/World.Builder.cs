/*
	Junk Jack X: Protocol
	- [Extensions]JJxWorldBuilder

	Written By: Ryan Smith
*/

using System;
using System.Buffers;
using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using CommunityToolkit.HighPerformance;
using JJx.Core;
using JJx.Protocol.Packets;

namespace JJx.Protocol.Extensions;

public static class JJxWorldBuilderExtensions
{
	/* Static Methods */
	public static void ApplySkyline(this JJxWorldBuilder builder, WorldSkylinePacket packet)
	{
		using var decompressionStream = packet.GetDecompressionStream();
		decompressionStream.ReadExactly(MemoryMarshal.Cast<ushort, byte>(builder.Skyline));
	}
	public static WorldProgressPacket ApplyCompressedSegment(this JJxWorldBuilder builder, WorldCompressedSegmentPacket packet)
	{
		var progress = builder.PushToCompressedBuffer(packet.CompressedData.Span);
		return new(progress);
	}
	public static Tilemap BuildTilemap(this JJxWorldBuilder builder)
	{
		Debug.Assert(builder.AreSegmentsCompleted);
		var length = builder.Size.Width * builder.Size.Height * Tile.SIZE;
		using var compressedStream = builder.CompressedMemory.AsStream();
		using var decompressionStream = new GZipStream(compressedStream, CompressionMode.Decompress);
		var buffer = ArrayPool<byte>.Shared.Rent(length);
		try {
			var slice = buffer.AsSpan(0, length);
			decompressionStream.ReadExactly(slice);
			return Tilemap.Deserialize(slice, builder.Size);
		} finally {
			ArrayPool<byte>.Shared.Return(buffer);
		}
	}
}
