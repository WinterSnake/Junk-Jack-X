/*
	Junk Jack X: Protocol
	- [Extensions]JJxReader

	Written By: Ryan Smith
*/

using System.Buffers.Binary;
using JJx.Core.Serialization;
using JJx.Protocol.Packets;

internal static class JJxReaderExtensions
{
	/* Static Methods */
	public static JJxPacketOpcode ReadOpcode(ref this JJxReader reader)
	{
		var opcode = reader.ReadUInt16();
		opcode = BinaryPrimitives.ReverseEndianness(opcode);
		return (JJxPacketOpcode)opcode;
	}
}
