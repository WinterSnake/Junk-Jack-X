/*
	Junk Jack X: Protocol
	- [Packet::World]Skyline

	Written By: Ryan Smith
*/

using System;
using JJx.Core.Serialization;

namespace JJx.Protocol.Packets;

[PacketOpcode(Opcode=JJxPacketOpcode.WorldSkyline)]
public sealed class WorldSkylinePacket : JJxPacket
{
	/* Constructor */
	private WorldSkylinePacket(byte[] compressedData) => this._CompressedData = compressedData;
	/* Static Methods */
	public static WorldSkylinePacket Compress(ushort[] skyline)
	{
		return new(Array.Empty<byte>());
	}
	internal static WorldSkylinePacket Deserialize(ref JJxReader reader)
	{
		var buffer = new byte[reader.Remaining];
		reader.CopyTo(buffer);
		return new(buffer);
	}
	internal static void Serialize(WorldSkylinePacket packet, JJxWriter writer)
	{
		writer.Write(packet._CompressedData);
	}
    /* Properties */
	public ReadOnlyMemory<byte> CompressedData => this._CompressedData;
	private readonly byte[] _CompressedData;
}
