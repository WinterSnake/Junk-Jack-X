/*
	Junk Jack X: Protocol
	- [Packet::World]Blocks

	Written By: Ryan Smith
*/

using System;
using CommunityToolkit.HighPerformance;
using JJx.Core.Serialization;

namespace JJx.Protocol.Packets;

[PacketOpcode(Opcode=JJxPacketOpcode.WorldCompressedSegment)]
public sealed class WorldCompressedSegmentPacket : JJxPacket
{
	/* Constructor */
	internal WorldCompressedSegmentPacket(byte[] compressedData) => this._CompressedData = compressedData;
	/* Static Methods */
	internal static WorldCompressedSegmentPacket Deserialize(ref JJxReader reader)
	{
		var buffer = new byte[reader.ReadUInt32()];
		reader.CopyTo(buffer);
		return new(buffer);
	}
	internal static void Serialize(WorldCompressedSegmentPacket packet, JJxWriter writer)
	{
		writer.Write(packet._CompressedData.Length);
		writer.Write(packet._CompressedData.AsSpan());
	}
    /* Properties */
	public ReadOnlyMemory<byte> CompressedData => this._CompressedData;
	private readonly byte[] _CompressedData;
}
