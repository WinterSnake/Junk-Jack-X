/*
	Junk Jack X: Protocol
	- [Packet::Management]Login

	Written By: Ryan Smith
*/

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
	internal static void Serialize(LoginRequestPacket packet, JJxWriter writer)
	{
		writer.Write(packet.Id);
		writer.Write(packet.Name, length: SIZEOF_NAME);
		writer.Write(packet.Version);
	}
	internal static LoginRequestPacket Deserialize(ref JJxReader reader) => new(
		reader.ReadUInt8(),
		reader.ReadString(length: SIZEOF_NAME),
		reader.ReadObject<JJxVersion>()
	);
    /* Properties */
	public readonly byte Id;
	public readonly string Name;
	public readonly JJxVersion Version;
	/* Class Properties */
	public const int SIZEOF_NAME = 32;
}
