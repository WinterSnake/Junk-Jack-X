/*
	Junk Jack X: Protocol
	- [Packet::Registry]Server

	Written By: Ryan Smith
*/

using JJx.Protocol.Packets;

namespace JJx.Protocol.Metadata;

internal static class JJxServerRegistry
{
	/* Constructor */
	static JJxServerRegistry()
	{
		Registry = new();
		Registry.RegisterDeserializer(LoginRequestPacket.Deserialize);
		Registry.RegisterSerializer<LoginSuccessPacket>(LoginSuccessPacket.Serialize);
		Registry.RegisterSerializer<LoginFailPacket>(LoginFailPacket.Serialize);
		Registry.RegisterDeserializer(WorldInfoRequestPacket.Deserialize);
		Registry.RegisterSerializer<WorldInfoResponsePacket>(WorldInfoResponsePacket.Serialize);
		Registry.RegisterSerializer<WorldSkylinePacket>(WorldSkylinePacket.Serialize);
		Registry.RegisterSerializer<WorldCompressedSegmentPacket>(WorldCompressedSegmentPacket.Serialize);
		Registry.RegisterDeserializer(WorldProgressPacket.Deserialize);
	}
	/* Class Properties */
	internal static readonly JJxPacketRegistry Registry;
}
