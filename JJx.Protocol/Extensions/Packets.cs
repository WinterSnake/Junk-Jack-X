/*
	Junk Jack X: Protocol
	- [Extensions]Packets

	Written By: Ryan Smith
*/

using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using CommunityToolkit.HighPerformance;
using JJx.Protocol.Packets;

namespace JJx.Protocol.Extensions;

internal static class PacketExtensions
{
	/* Static Methods */
	// Skyline Packet
	public static ushort[] Decompress(this WorldSkylinePacket packet, ushort width)
	{
		var skyline = new ushort[width];
		using var decompressionStream = packet.GetDecompressionStream();
		decompressionStream.ReadExactly(MemoryMarshal.Cast<ushort, byte>(skyline.AsSpan()));
		return skyline;
	}
	internal static Stream GetDecompressionStream(this WorldSkylinePacket packet)
	{
		var compressedStream = packet.CompressedData.AsStream();
		return new GZipStream(compressedStream, CompressionMode.Decompress);
	}
}
