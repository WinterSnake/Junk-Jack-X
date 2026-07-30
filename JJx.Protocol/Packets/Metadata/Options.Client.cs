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
	}
	/* Class Properties */
	internal static readonly JJxPacketRegistry Registry;
}
