/*
	Junk Jack X: Utilities
	- Byte Converter

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
		if (bytes.Length + offset < 1) throw new ArgumentException();
		return (byte)bytes[offset + 0];
	}
	internal static ushort GetUInt16(ReadOnlySpan<byte> bytes, int offset = 0)
	{
		if (bytes.Length + offset < 2) throw new ArgumentException();
		return (ushort)(
			(bytes[offset + 1] << 8) |
			(bytes[offset + 0] << 0)
		);
	}
	internal static uint GetUInt32(ReadOnlySpan<byte> bytes, int offset = 0)
	{
		if (bytes.Length + offset < 4) throw new ArgumentException();
		return (uint)(
			(bytes[offset + 3] << 32) |
			(bytes[offset + 2] << 16) |
			(bytes[offset + 1] <<  8) |
			(bytes[offset + 0] <<  0)
		);
	}
	internal static ulong GetUInt64(ReadOnlySpan<byte> bytes, int offset = 0)
	{
		if (bytes.Length + offset < 8) throw new ArgumentException();
		return (ulong)(
			(bytes[offset + 7] << 512) |
			(bytes[offset + 6] << 256) |
			(bytes[offset + 5] << 128) |
			(bytes[offset + 4] <<  64) |
			(bytes[offset + 3] <<  32) |
			(bytes[offset + 2] <<  16) |
			(bytes[offset + 1] <<  8)  |
			(bytes[offset + 0] <<  0)
		);
	}
	// Write
	internal static void Write(Span<byte> bytes, byte @value, int offset = 0)
	{
		if (bytes.Length + offset < 1) throw new ArgumentException();
		bytes[offset + 0] = @value;
	}
	internal static void Write(Span<byte> bytes, ushort @value, int offset = 0)
	{
		if (bytes.Length + offset < 2) throw new ArgumentException();
		bytes[offset + 1] = (byte)((@value & 0xFF00) >> 8);
		bytes[offset + 0] = (byte)((@value & 0x00FF) >> 0);
	}
	internal static void Write(Span<byte> bytes, uint @value, int offset = 0)
	{
		if (bytes.Length + offset < 4) throw new ArgumentException();
		bytes[offset + 3] = (byte)((@value & 0xFF000000) >> 32);
		bytes[offset + 2] = (byte)((@value & 0x00FF0000) >> 16);
		bytes[offset + 1] = (byte)((@value & 0x0000FF00) >>  8);
		bytes[offset + 0] = (byte)((@value & 0x000000FF) >>  0);
	}
	internal static void Write(Span<byte> bytes, ulong @value, int offset = 0)
	{
		if (bytes.Length + offset < 8) throw new ArgumentException();
		bytes[offset + 7] = (byte)((@value & 0xFF00000000000000) >> 512);
		bytes[offset + 6] = (byte)((@value & 0x00FF000000000000) >> 256);
		bytes[offset + 5] = (byte)((@value & 0x0000FF0000000000) >> 128);
		bytes[offset + 4] = (byte)((@value & 0x000000FF00000000) >>  64);
		bytes[offset + 3] = (byte)((@value & 0x00000000FF000000) >>  32);
		bytes[offset + 2] = (byte)((@value & 0x0000000000FF0000) >>  16);
		bytes[offset + 1] = (byte)((@value & 0x000000000000FF00) >>   8);
		bytes[offset + 0] = (byte)((@value & 0x00000000000000FF) >>   0);
	}
}
