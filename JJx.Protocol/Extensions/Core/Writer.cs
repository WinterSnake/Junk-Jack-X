/*
	Junk Jack X: Protocol
	- [Extensions]JJxWriter

	Written By: Ryan Smith
*/

using System.Buffers.Binary;
using JJx.Core.Serialization;
using JJx.Protocol.Packets;

namespace JJx.Protocol.Extensions;

internal static class JJxWriterExtensions
{
	/* Static Methods */
	public static void WriteOpcode(this JJxWriter writer, JJxPacketOpcode @value)
	{
		var opcode = BinaryPrimitives.ReverseEndianness((ushort)@value);
		writer.Write(opcode);
	}
}
