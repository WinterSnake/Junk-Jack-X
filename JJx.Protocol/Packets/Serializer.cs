/*
	Junk Jack X: Protocol
	- [Packet] Serializer

	Written By: Ryan Smith
*/

using System;
using System.Buffers;
using JJx.Core.Serialization;
using JJx.Protocol.Extensions;

namespace JJx.Protocol.Packets;

public static class JJxPacketSerializer
{
	/* Static Methods */
	public static void Serialize(JJxPacket packet, IBufferWriter<byte> writer, JJxPacketRegistry options)
	{
		var packetType = packet.GetType();
		if (!options.TryGetSerializer(packet.GetType(), out var packetInfo))
			throw new InvalidOperationException($"No packet serializer found for {packet.GetType().Name}. Packet not registered.");
		var packetWriter = new JJxWriter(writer);
		packetWriter.WriteOpcode(packetInfo.Opcode);
		packetInfo.SerializeFunc(packet, packetWriter);
	}
	public static JJxPacket Deserialize(in ReadOnlySpan<byte> rawPacket, JJxPacketRegistry options)
	{
		var reader = new JJxReader(rawPacket);
		var opcode = reader.ReadOpcode();
		if (!options.TryGetDeserializer(opcode, out var deserializeFunc))
			throw new InvalidOperationException($"No packet deserializer found for 0x{(ushort)opcode:X4}. Packet not registered.");
		return deserializeFunc(ref reader);
	}
}
