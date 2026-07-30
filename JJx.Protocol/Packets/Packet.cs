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
	// Sub-type
	LoginRequest = Management | 0x02,
}

[AttributeUsage(AttributeTargets.Class)]
internal sealed class PacketOpcodeAttribute : Attribute
{
	/* Properties */
	public JJxPacketOpcode Opcode { get; init; }
}

public abstract class JJxPacket { }
