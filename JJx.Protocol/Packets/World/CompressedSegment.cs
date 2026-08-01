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
	internal WorldCompressedSegmentPacket(ReadOnlyMemory<byte> compressedData) => this.CompressedData = compressedData;
	/* Static Methods */
	internal static WorldCompressedSegmentPacket Deserialize(ref JJxReader reader)
	{
		var buffer = new byte[reader.ReadUInt32()];
		reader.CopyTo(buffer);
		return new(buffer.AsMemory());
	}
	internal static void Serialize(WorldCompressedSegmentPacket packet, JJxWriter writer)
	{
		writer.Write(packet.CompressedData.Length);
		writer.Write(packet.CompressedData.Span);
	}
    /* Properties */
	public ReadOnlyMemory<byte> CompressedData;
}
