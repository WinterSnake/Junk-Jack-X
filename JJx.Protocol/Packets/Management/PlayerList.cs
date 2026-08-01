/*
	Junk Jack X: Protocol
	- [Packet::Management]Player List

	Written By: Ryan Smith
*/

using System;
using JJx.Core;
using JJx.Core.Serialization;

namespace JJx.Protocol.Packets;

[PacketOpcode(Opcode=JJxPacketOpcode.PlayerListRequest)]
public sealed class PlayerListRequestPacket : JJxPacket
{
	/* Constructor */
	public PlayerListRequestPacket(ushort status = 0x0000) => this.Status = status;
	/* Static Methods */
	internal static PlayerListRequestPacket Deserialize(ref JJxReader reader) => new(
		reader.ReadUInt16()
	);
	internal static void Serialize(PlayerListRequestPacket packet, JJxWriter writer)
	{
		writer.Write(packet.Status);
	}
    /* Properties */
	public readonly ushort Status;
}

[PacketOpcode(Opcode=JJxPacketOpcode.PlayerListEntry)]
public sealed class PlayerListEntryPacket : JJxPacket
{
	/* Constructor */
	public PlayerListEntryPacket(byte id, bool isSelf, string name)
	{
		this.Id = id;
		this.IsSelf = isSelf;
		this.Name = name;
	}
	/* Static Methods */
	internal static PlayerListEntryPacket Deserialize(ref JJxReader reader) => new(
		reader.ReadUInt8(),
		reader.ReadBool(),
		reader.ReadString(SIZEOF_NAME)
	);
	internal static void Serialize(PlayerListEntryPacket packet, JJxWriter writer)
	{
		writer.Write(packet.Id);
		writer.Write(packet.IsSelf);
		writer.Write(packet.Name, SIZEOF_NAME);
	}
    /* Properties */
	public readonly byte Id;
	public readonly bool IsSelf;
	public readonly string Name;
	/* Class Properties */
	public const int SIZEOF_NAME = 32;
}
