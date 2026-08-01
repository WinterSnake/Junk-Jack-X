/*
	Junk Jack X: Protocol
	- [Extensions]JJxWorld

	Written By: Ryan Smith
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using CommunityToolkit.HighPerformance;
using JJx.Core;
using JJx.Protocol.Packets;

namespace JJx.Protocol.Extensions;

public static partial class JJxWorldExtensions
{
	/* Static Methods */
	public static (uint sizeInBytes, IEnumerable<WorldCompressedSegmentPacket> compressedSegments) Compress(this Tilemap tilemap, int maxSegmentSize = 1024)
	{
		// Raw
		using var owner = Tilemap.Serialize(tilemap, out var buffer);
		using var decompressedStream = buffer.AsStream();
		// Compression
		using var compressedStream = new MemoryStream();
		using (var compressionStream = new GZipStream(compressedStream, new ZLibCompressionOptions() { CompressionLevel = 6 }, true))
			decompressedStream.CopyTo(compressionStream);
		// Get Iter
		var compressedSegment = compressedStream.GetBuffer().AsMemory(0, (int)compressedStream.Position);
		return ((uint)compressedSegment.Length, _GetCompressionSegmentIter(compressedSegment, maxSegmentSize));
	}
	private static IEnumerable<WorldCompressedSegmentPacket> _GetCompressionSegmentIter(ReadOnlyMemory<byte> compressedSegment, int maxSegmentSize)
	{
		while (compressedSegment.Length > 0)
		{
			var segmentSize = Math.Min(compressedSegment.Length, maxSegmentSize);
			var segment = compressedSegment[..segmentSize];
			yield return new(segment);
			compressedSegment = compressedSegment.Slice(segmentSize);
		}
	}
}
