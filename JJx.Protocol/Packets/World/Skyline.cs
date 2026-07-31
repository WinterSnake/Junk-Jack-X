/*
	Junk Jack X: Protocol
	- [Packet::World]Skyline

	Written By: Ryan Smith
*/

using System;
using System.Buffers;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using JJx.Core;
using JJx.Core.Serialization;

namespace JJx.Protocol.Packets;

[PacketOpcode(Opcode=JJxPacketOpcode.WorldSkyline)]
public sealed class WorldSkylinePacket : JJxPacket
{
	/* Constructor */
	private WorldSkylinePacket(byte[] compressedData) => this._CompressedData = compressedData;
	/* Instance Methods */
	public ushort[] Decompress(ushort width)
	{
		var skyline = new ushort[width];
		using var compressedStream = new MemoryStream(this._CompressedData);
		using var decompressionStream = new GZipStream(compressedStream, CompressionMode.Decompress);
		decompressionStream.ReadExactly(MemoryMarshal.Cast<ushort, byte>(skyline.AsSpan()));
		return skyline;
	}
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
	/* Class Properties */
	public readonly byte[] _CompressedData;
}
