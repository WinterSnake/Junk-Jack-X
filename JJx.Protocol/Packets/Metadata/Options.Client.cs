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
		Registry.RegisterSerializer<PlayerListRequestPacket>(PlayerListRequestPacket.Serialize);
		Registry.RegisterDeserializer(PlayerListEntryPacket.Deserialize);
		Registry.RegisterDeserializer(WorldTimePacket.Deserialize);
		Registry.RegisterSerializer<PlayerReadyPacket>(PlayerReadyPacket.Serialize);
		Registry.Register(PlayerUpdateModelPacket.Serialize, PlayerUpdateModelPacket.Deserialize);
		Registry.Register(PlayerUpdateItemPacket.Serialize, PlayerUpdateItemPacket.Deserialize);
		Registry.Register(PlayerUpdateEquipmentPacket.Serialize, PlayerUpdateEquipmentPacket.Deserialize);
	}
	/* Class Properties */
	internal static readonly JJxPacketRegistry Registry;
}
