/*
	Junk Jack X: Protocol
	- Packet

	Written By: Ryan Smith
*/

using System;

namespace JJx.Protocol.Packets;

public enum JJxPacketOpcode : ushort
{
	// Primary
	Management = (0x00 << 8),
	WorldData  = (0x03 << 8),
	Entity     = (0x05 << 8),
	// Sub-type[Management]
	LoginRequest      = Management | 0x02,
	LoginSuccess      = Management | 0x03,
	LoginFail         = Management | 0x0C,
	WorldInfoRequest  = Management | 0x09,
	// Sub-type[WorldData]
	WorldInfoResponse = WorldData  | 0x43,
	WorldSkyline      = WorldData  | 0x4C,
	// Sub-type[Entity]
}

[AttributeUsage(AttributeTargets.Class)]
internal sealed class PacketOpcodeAttribute : Attribute
{
	/* Properties */
	public JJxPacketOpcode Opcode { get; init; }
}

public abstract class JJxPacket { }
