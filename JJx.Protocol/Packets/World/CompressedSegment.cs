/*
	Junk Jack X: Protocol
	- [Packet::World]Blocks

	Written By: Ryan Smith
*/

using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using JJx.Core;
using JJx.Core.Serialization;

namespace JJx.Protocol.Packets;

[PacketOpcode(Opcode=JJxPacketOpcode.WorldCompressedSegment)]
public sealed class WorldCompressedSegmentPacket : JJxPacket
{
	/* Constructor */
	private WorldCompressedSegmentPacket(byte[] compressedData) => this._CompressedData = compressedData;
	/* Static Methods */
	public static IEnumerable<WorldCompressedSegmentPacket> Compress(JJxWorld world, int maxChunkSize = 1024)
	{
		yield return new(Array.Empty<byte>());
	}
	internal static WorldCompressedSegmentPacket Deserialize(ref JJxReader reader)
	{
		var buffer = new byte[reader.ReadUInt32()];
		reader.CopyTo(buffer);
		return new(buffer);
	}
	internal static void Serialize(WorldCompressedSegmentPacket packet, JJxWriter writer)
	{
		writer.Write(packet._CompressedData.Length);
		writer.Write(packet._CompressedData);
	}
    /* Properties */
	public ReadOnlyMemory<byte> CompressedData => this._CompressedData;
	private readonly byte[] _CompressedData;
}
