/*
	Junk Jack X: Protocol
	- [Packet::Registry]Client

	Written By: Ryan Smith
*/

using JJx.Protocol.Packets;

namespace JJx.Protocol.Metadata;

internal static class JJxClientRegistry
{
	/* Constructor */
	static JJxClientRegistry()
	{
		Registry = new();
		Registry.RegisterSerializer<LoginRequestPacket>(LoginRequestPacket.Serialize);
		Registry.RegisterDeserializer(LoginSuccessPacket.Deserialize);
		Registry.RegisterDeserializer(LoginFailPacket.Deserialize);
		Registry.RegisterSerializer<WorldInfoRequestPacket>(WorldInfoRequestPacket.Serialize);
		Registry.RegisterDeserializer(WorldInfoResponsePacket.Deserialize);
		Registry.RegisterDeserializer(WorldSkylinePacket.Deserialize);
		Registry.RegisterDeserializer(WorldCompressedSegmentPacket.Deserialize);
		Registry.RegisterSerializer<WorldProgressPacket>(WorldProgressPacket.Serialize);
	}
	/* Class Properties */
	internal static readonly JJxPacketRegistry Registry;
}
