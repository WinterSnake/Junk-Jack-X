/*
	Junk Jack X: Protocol
	- [Extensions]JJxWorldBuilder

	Written By: Ryan Smith
*/

using System;
using System.Runtime.InteropServices;
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
	public static void ApplyCompressedSegment(this JJxWorldBuilder builder, WorldCompressedSegmentPacket packet)
	{
		var completion = builder.PushToCompressedBuffer(packet.CompressedData.Span);
	}
}
