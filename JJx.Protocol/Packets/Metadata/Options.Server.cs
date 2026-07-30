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
		Registry.RegisterDeserializer<LoginRequestPacket>(LoginRequestPacket.Deserialize);
	}
	/* Class Properties */
	internal static readonly JJxPacketRegistry Registry;
}
