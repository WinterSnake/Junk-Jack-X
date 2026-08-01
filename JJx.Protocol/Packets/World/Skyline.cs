/*
	Junk Jack X: Protocol
	- [Packet::World]Skyline

	Written By: Ryan Smith
*/

using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using JJx.Core.Serialization;

namespace JJx.Protocol.Packets;

[PacketOpcode(Opcode=JJxPacketOpcode.WorldSkyline)]
public sealed class WorldSkylinePacket : JJxPacket
{
	/* Constructor */
	private WorldSkylinePacket(byte[] compressedData) => this._CompressedData = compressedData;
	/* Static Methods */
	public static WorldSkylinePacket Compress(ReadOnlySpan<ushort> skyline)
	{
		// Raw
		using var decompressedStream = new MemoryStream(new byte[skyline.Length * 2]);
		decompressedStream.Write(MemoryMarshal.Cast<ushort, byte>(skyline));
		decompressedStream.Seek(0, SeekOrigin.Begin);
		// Compression
		using var compressedStream = new MemoryStream();
		using (var compressionStream = new GZipStream(compressedStream, CompressionMode.Compress, true))
			decompressedStream.CopyTo(compressionStream);
		return new(compressedStream.ToArray());
	}
	internal static WorldSkylinePacket Deserialize(ref JJxReader reader)
	{
		var buffer = new byte[reader.Remaining];
		reader.CopyTo(buffer);
		return new(buffer);
	}
	internal static void Serialize(WorldSkylinePacket packet, JJxWriter writer)
	{
		writer.Write(packet._CompressedData.AsSpan());
	}
    /* Properties */
	public ReadOnlyMemory<byte> CompressedData => this._CompressedData;
	private readonly byte[] _CompressedData;
}
