/*
	Junk Jack X
	- Utilities

	Written By: Ryan Smith
*/
using System;

namespace JJx.Utils;

internal static class ByteConverter
{
	/* Static Methods */
	// Read
	internal static byte GetUInt8(ReadOnlySpan<byte> bytes, int offset = 0)
	{
		if (bytes.Length < 1) throw new ArgumentException();
		return (byte)bytes[offset + 0];
	}
	internal static ushort GetUInt16(ReadOnlySpan<byte> bytes, int offset = 0)
	{
		if (bytes.Length < 2) throw new ArgumentException();
		return (ushort)(
			(bytes[offset + 1] << 8) |
			(bytes[offset + 0] << 0)
		);
	}
	internal static uint GetUInt32(ReadOnlySpan<byte> bytes, int offset = 0)
	{
		if (bytes.Length < 4) throw new ArgumentException();
		return (ushort)(
			(bytes[offset + 3] << 32) |
			(bytes[offset + 2] << 16) |
			(bytes[offset + 1] << 8)  |
			(bytes[offset + 0] << 0)
		);
	}
	// Write
	internal static void Write(Span<byte> bytes, byte @value, int offset = 0)
	{
		if (bytes.Length < 1) throw new ArgumentException();
		bytes[offset + 0] = @value;
	}
	internal static void Write(Span<byte> bytes, ushort @value, int offset = 0)
	{
		if (bytes.Length < 2) throw new ArgumentException();
		bytes[offset + 1] = (byte)((@value & 0xFF00) >> 8);
		bytes[offset + 0] = (byte)((@value & 0x00FF) >> 0);
	}
	internal static void Write(Span<byte> bytes, uint @value, int offset = 0)
	{
		if (bytes.Length < 4) throw new ArgumentException();
		bytes[offset + 3] = (byte)((@value & 0xFF000000) >> 32);
		bytes[offset + 2] = (byte)((@value & 0x00FF0000) >> 16);
		bytes[offset + 1] = (byte)((@value & 0x0000FF00) >> 8);
		bytes[offset + 0] = (byte)((@value & 0x000000FF) >> 0);
	}
}
