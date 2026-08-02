/*
	Junk Jack X: Protocol
	- [Packet::World]Time

	Written By: Ryan Smith
*/

using System;
using JJx.Core;
using JJx.Core.Serialization;

namespace JJx.Protocol.Packets;

[PacketOpcode(Opcode=JJxPacketOpcode.WorldTime)]
public sealed class WorldTimePacket : JJxPacket
{
	/* Constructor */
	public WorldTimePacket(DayPhase dayPhase, ushort ticks)
	{
		this.DayPhase = dayPhase;
		this.Ticks = ticks;
	}
	/* Static Methods */
	internal static WorldTimePacket Deserialize(ref JJxReader reader) => new(
		reader.ReadObject<DayPhase>(JJxPacketRegistry.Default),
		reader.ReadUInt16()
	);
	internal static void Serialize(WorldTimePacket packet, JJxWriter writer)
	{
		writer.Write(packet.DayPhase, JJxPacketRegistry.Default);
		writer.Write(packet.Ticks);
	}
    /* Properties */
	public readonly DayPhase DayPhase;
	public readonly ushort Ticks;
}
