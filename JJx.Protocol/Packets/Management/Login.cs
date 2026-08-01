/*
	Junk Jack X: Protocol
	- [Packet::Management]Login

	Written By: Ryan Smith
*/

using System;
using JJx.Core;
using JJx.Core.Serialization;

namespace JJx.Protocol.Packets;

[PacketOpcode(Opcode=JJxPacketOpcode.LoginRequest)]
public sealed class LoginRequestPacket : JJxPacket
{
	/* Constructor */
	public LoginRequestPacket(byte id, string name, JJxVersion version)
	{
		this.Id = id;
		this.Name = name;
		this.Version = version;
	}
	/* Static Methods */
	internal static LoginRequestPacket Deserialize(ref JJxReader reader) => new(
		reader.ReadUInt8(),
		reader.ReadString(length: SIZEOF_NAME),
		reader.ReadObject<JJxVersion>()
	);
	internal static void Serialize(LoginRequestPacket packet, JJxWriter writer)
	{
		writer.Write(packet.Id);
		writer.Write(packet.Name, length: SIZEOF_NAME);
		writer.Write(packet.Version);
	}
    /* Properties */
	public readonly byte Id;
	public readonly string Name;
	public readonly JJxVersion Version;
	/* Class Properties */
	public const int SIZEOF_NAME = 32;
}

[PacketOpcode(Opcode=JJxPacketOpcode.LoginSuccess)]
public sealed class LoginSuccessPacket : JJxPacket
{
	/* Constructor */
	public LoginSuccessPacket(ushort status = 0x0001) => this.Status = status;
	/* Static Methods */
	internal static LoginSuccessPacket Deserialize(ref JJxReader reader) => new(
		reader.ReadUInt16()
	);
	internal static void Serialize(LoginSuccessPacket packet, JJxWriter writer)
	{
		writer.Write(packet.Status);
	}
	/* Properties */
	public readonly ushort Status;
}

[PacketOpcode(Opcode=JJxPacketOpcode.LoginFail)]
public sealed class LoginFailPacket : JJxPacket
{
	/* Constructor */
	public LoginFailPacket(FailureCode code) => this.Code = code;
	/* Static Methods */
	internal static LoginFailPacket Deserialize(ref JJxReader reader) => new(
		reader.ReadObject<FailureCode>()
	);
	internal static void Serialize(LoginFailPacket packet, JJxWriter writer)
	{
		writer.Write(packet.Code);
	}
	/* Properties */
	public readonly FailureCode Code;
	/* Sub-Classes */
	public enum FailureCode : byte
	{
		ServerIsFull = 0x00,
		DifferentGameVersion = 0x01,
	}
}

[PacketOpcode(Opcode=JJxPacketOpcode.PlayerReady)]
public sealed class PlayerReadyPacket : JJxPacket
{
	/* Constructor */
	public PlayerReadyPacket(byte maxHealth, byte health)
	{
		this.Health = health;
		this.MaxHealth = maxHealth;
	}
	/* Static Methods */
	internal static PlayerReadyPacket Deserialize(ref JJxReader reader) => new(
		reader.ReadUInt8(),
		reader.ReadUInt8()
	);
	internal static void Serialize(PlayerReadyPacket packet, JJxWriter writer)
	{
		writer.Write(packet.MaxHealth);
		writer.Write(packet.Health);
	}
	/* Properties */
	public readonly byte Health;
	public readonly byte MaxHealth;
}
