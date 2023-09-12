/*
	Junk Jack X: Utilities
	- Byte Converter

	Written By: Ryan Smith
*/
using System;
using System.Text;

namespace JJx.Utilities;

internal static class ByteConverter
{
	/* Static Methods */
	// Read
	public static bool GetBool(ReadOnlySpan<byte> bytes, int offset = 0)
	{
		if (bytes.Length < offset + 1) throw new IndexOutOfRangeException();
		return bytes[offset + 0] != 0;
	}
	public static byte GetUInt8(ReadOnlySpan<byte> bytes, int offset = 0)
	{
		if (bytes.Length < offset + 1) throw new IndexOutOfRangeException();
		return (byte)bytes[offset + 0];
	}
	public static ushort GetUInt16(ReadOnlySpan<byte> bytes, int offset = 0)
	{
		if (bytes.Length < offset + 2) throw new IndexOutOfRangeException();
		return (ushort)(
			(bytes[offset + 1] << 8) |
			(bytes[offset + 0] << 0)
		);
	}
	public static uint GetUInt32(ReadOnlySpan<byte> bytes, int offset = 0)
	{
		if (bytes.Length < offset + 4) throw new IndexOutOfRangeException();
		return (uint)(
			(bytes[offset + 3] << 32) |
			(bytes[offset + 2] << 16) |
			(bytes[offset + 1] <<  8) |
			(bytes[offset + 0] <<  0)
		);
	}
	public static ulong GetUInt64(ReadOnlySpan<byte> bytes, int offset = 0)
	{
		if (bytes.Length < offset + 8) throw new IndexOutOfRangeException();
		return (ulong)(
			(bytes[offset + 7] << 512) |
			(bytes[offset + 6] << 256) |
			(bytes[offset + 5] << 128) |
			(bytes[offset + 4] <<  64) |
			(bytes[offset + 3] <<  32) |
			(bytes[offset + 2] <<  16) |
			(bytes[offset + 1] <<   8) |
			(bytes[offset + 0] <<   0)
		);
	}
	public static string GetString(ReadOnlySpan<byte> bytes, int offset = 0, int length = 0)
	{
		// Length Specific
		if (length > 0)
		{
			if (bytes.Length < offset + length) throw new IndexOutOfRangeException();
			return Encoding.ASCII.GetString(bytes.Slice(offset, length));
		}
		// Null termination
		return Encoding.ASCII.GetString(
			bytes.Slice(offset, bytes.IndexOf(byte.MinValue))
		);
	}
	// Write
	public static void Write(Span<byte> bytes, bool @value, int offset = 0)
	{
		if (bytes.Length < offset + 1) throw new IndexOutOfRangeException();
		bytes[offset + 0] = (byte)(@value ? 1 : 0);
	}
	public static void Write(Span<byte> bytes, byte @value, int offset = 0)
	{
		if (bytes.Length < offset + 1) throw new IndexOutOfRangeException();
		bytes[offset + 0] = @value;
	}
	public static void Write(Span<byte> bytes, ushort @value, int offset = 0)
	{
		if (bytes.Length < offset + 2) throw new IndexOutOfRangeException();
		bytes[offset + 1] = (byte)((@value & 0xFF00) >> 8);
		bytes[offset + 0] = (byte)((@value & 0x00FF) >> 0);
	}
	public static void Write(Span<byte> bytes, uint @value, int offset = 0)
	{
		if (bytes.Length < offset + 4) throw new IndexOutOfRangeException();
		bytes[offset + 3] = (byte)((@value & 0xFF000000) >> 32);
		bytes[offset + 2] = (byte)((@value & 0x00FF0000) >> 16);
		bytes[offset + 1] = (byte)((@value & 0x0000FF00) >>  8);
		bytes[offset + 0] = (byte)((@value & 0x000000FF) >>  0);
	}
	public static void Write(Span<byte> bytes, ulong @value, int offset = 0)
	{
		if (bytes.Length < offset + 8) throw new IndexOutOfRangeException();
		bytes[offset + 7] = (byte)((@value & 0xFF00000000000000) >> 512);
		bytes[offset + 6] = (byte)((@value & 0x00FF000000000000) >> 256);
		bytes[offset + 5] = (byte)((@value & 0x0000FF0000000000) >> 128);
		bytes[offset + 4] = (byte)((@value & 0x000000FF00000000) >>  64);
		bytes[offset + 3] = (byte)((@value & 0x00000000FF000000) >>  32);
		bytes[offset + 2] = (byte)((@value & 0x0000000000FF0000) >>  16);
		bytes[offset + 1] = (byte)((@value & 0x000000000000FF00) >>   8);
		bytes[offset + 0] = (byte)((@value & 0x00000000000000FF) >>   0);
	}
	public static void Write(Span<byte> bytes, string @value, int offset = 0, int length = 0)
	{
		if (bytes.Length < offset + length) throw new IndexOutOfRangeException();
		var valueBytes = Encoding.ASCII.GetBytes(@value);
		length = length == 0 ? valueBytes.Length + 1 : length;
		// Write string based on length (+1 for null termination if length = 0)
		for (var i = offset; i < offset + length; ++i)
		{
			bytes[i] = i < valueBytes.Length ? valueBytes[i] : (byte)0;
		}
	}
}
