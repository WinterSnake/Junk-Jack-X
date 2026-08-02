/*
	Junk Jack X: Protocol
	- [Packet::Management]World

	Written By: Ryan Smith
*/

using System;
using JJx.Core.Serialization;

namespace JJx.Protocol.Packets;

[PacketOpcode(Opcode=JJxPacketOpcode.ManagementWorldInfoRequest)]
public sealed class WorldInfoRequestPacket : JJxPacket
{
	/* Constructor */
	public WorldInfoRequestPacket(ushort status = 0x0000) => this.Status = status;
	/* Static Methods */
	internal static WorldInfoRequestPacket Deserialize(ref JJxReader reader) => new(
		reader.ReadUInt16()
	);
	internal static void Serialize(WorldInfoRequestPacket packet, JJxWriter writer)
	{
		writer.Write(packet.Status);
	}
	/* Properties */
	public readonly ushort Status;
}

[PacketOpcode(Opcode=JJxPacketOpcode.ManagementWorldProgress)]
public sealed class WorldProgressPacket : JJxPacket
{
	/* Constructor */
	public WorldProgressPacket(ushort progress) => this.Progress = progress;
	public WorldProgressPacket(float progress) => this.Progress = (ushort)(progress * 100.0f);
	/* Static Methods */
	internal static WorldProgressPacket Deserialize(ref JJxReader reader) => new(
		reader.ReadUInt16()
	);
	internal static void Serialize(WorldProgressPacket packet, JJxWriter writer)
	{
		writer.Write(packet.Progress);
	}
	/* Properties */
	public readonly ushort Progress;
}
