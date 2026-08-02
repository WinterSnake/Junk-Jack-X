/*
	Junk Jack X: Protocol
	- [Packet::Player]Position

	Written By: Ryan Smith
*/

using System;
using JJx.Core;
using JJx.Core.Serialization;

namespace JJx.Protocol.Packets;

[PacketOpcode(Opcode=JJxPacketOpcode.PlayerCreativeFlags)]
public sealed class PlayerCreativeFlagsPacket : JJxPacket
{
	/* Constructor */
	public PlayerCreativeFlagsPacket(byte id, bool isFlying = true)
	{
		this.Id = id;
		this.IsFlying = isFlying;
	}
	/* Static Methods */
	internal static PlayerCreativeFlagsPacket Deserialize(ref JJxReader reader) => new(
		reader.ReadUInt8(),
		reader.ReadBool()
	);
	internal static void Serializate(PlayerCreativeFlagsPacket packet, JJxWriter writer)
	{
		writer.Write(packet.Id);
		writer.Write(packet.IsFlying);
	}
	/* Properties */
	public readonly byte Id;
	public readonly bool IsFlying;
}
