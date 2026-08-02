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
	Management = 0x00 << 8,
	Player     = 0x02 << 8,
	WorldData  = 0x03 << 8,
	Entity     = 0x05 << 8,
	// Sub-type[Management]
	ManagementLoginRequest      = Management | 0x02,
	ManagementLoginSuccess      = Management | 0x03,
	ManagementPlayerReady       = Management | 0x05,
	ManagementPlayerListRequest = Management | 0x06,
	ManagementPlayerListEntry   = Management | 0x07,
	ManagementWorldInfoRequest  = Management | 0x09,
	ManagementWorldProgress     = Management | 0x0A,
	ManagementLoginFail         = Management | 0x0C,
	// Sub-type[Player]
	PlayerSpawnLocation         = Player     | 0x11,
	PlayerUpdateItem            = Player     | 0x15,
	PlayerUpdateEquipment       = Player     | 0x16,
	PlayerUpdateModel           = Player     | 0x17,
	PlayerCreativeFlags         = Player     | 0x2E,
	// Sub-type[WorldData]
	WorldInfoResponse           = WorldData  | 0x43,
	WorldTime                   = WorldData  | 0x44,
	WorldCompressedSegment      = WorldData  | 0x47,
	WorldSkyline                = WorldData  | 0x4C,
	// Sub-type[Entity]
}

[AttributeUsage(AttributeTargets.Class)]
internal sealed class PacketOpcodeAttribute : Attribute
{
	/* Properties */
	public JJxPacketOpcode Opcode { get; init; }
}

public abstract class JJxPacket { }
