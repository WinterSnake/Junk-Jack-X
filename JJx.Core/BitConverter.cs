/*
	Junk Jack X: Core
	- Bit Converter

	Written By: Ryan Smith
*/
using System;
using System.Text;

namespace JJx;

public class BitConverter
{
	/* Constructor */
	public BitConverter(bool littleEndian) { this._LittleEndian = littleEndian; }
	/* Instance Methods */
	// Reading
	public ushort GetUInt16(ReadOnlySpan<byte> buffer, int offset = 0)
	{
		if (buffer.Length < offset + 2) throw new IndexOutOfRangeException();
		if (this._LittleEndian)
		{
			return (ushort)(
				(buffer[offset + 1] << 8) |
				(buffer[offset + 0] << 0)
			);
		}
		else
		{
			return (ushort)(
				(buffer[offset + 0] << 8) |
				(buffer[offset + 1] << 0)
			);
		}
	}
	public uint GetUInt32(ReadOnlySpan<byte> buffer, int offset = 0)
	{
		if (buffer.Length < offset + 4) throw new IndexOutOfRangeException();
		if (this._LittleEndian)
		{
			return (uint)(
				(buffer[offset + 3] << 24) |
				(buffer[offset + 2] << 16) |
				(buffer[offset + 1] <<  8) |
				(buffer[offset + 0] <<  0)
			);
		}
		else
		{
			return (uint)(
				(buffer[offset + 0] << 24) |
				(buffer[offset + 1] << 16) |
				(buffer[offset + 3] <<  8) |
				(buffer[offset + 3] <<  0)
			);
		}
	}
	public float GetFloat(ReadOnlySpan<byte> buffer, int offset = 0)
	{
		if (buffer.Length < offset + 4) throw new IndexOutOfRangeException();
		int flt;
		if (this._LittleEndian)
		{
			flt = (int)(
				(buffer[offset + 3] << 24) |
				(buffer[offset + 2] << 16) |
				(buffer[offset + 1] <<  8) |
				(buffer[offset + 0] <<  0)
			);
		}
		else
		{
			flt = (int)(
				(buffer[offset + 0] << 24) |
				(buffer[offset + 1] << 16) |
				(buffer[offset + 2] <<  8) |
				(buffer[offset + 3] <<  0)
			);
		}
		unsafe { return *(float*)&flt; }
	}
	// Writing
	public void Write(ushort @value, Span<byte> buffer, int offset = 0)
	{
		if (buffer.Length < offset + 2) throw new IndexOutOfRangeException();
		if (this._LittleEndian)
		{
			buffer[offset + 1] = (byte)((@value & 0xFF00) >> 8);
			buffer[offset + 0] = (byte)((@value & 0x00FF) >> 0);
		}
		else
		{
			buffer[offset + 0] = (byte)((@value & 0xFF00) >> 8);
			buffer[offset + 1] = (byte)((@value & 0x00FF) >> 0);
		}
	}
	public void Write(uint @value, Span<byte> buffer, int offset = 0)
	{
		if (buffer.Length < offset + 4) throw new IndexOutOfRangeException();
		if (this._LittleEndian)
		{
			buffer[offset + 3] = (byte)((@value & 0xFF000000) >> 24);
			buffer[offset + 2] = (byte)((@value & 0x00FF0000) >> 16);
			buffer[offset + 1] = (byte)((@value & 0x0000FF00) >>  8);
			buffer[offset + 0] = (byte)((@value & 0x000000FF) >>  0);
		}
		else
		{
			buffer[offset + 0] = (byte)((@value & 0xFF000000) >> 24);
			buffer[offset + 1] = (byte)((@value & 0x00FF0000) >> 16);
			buffer[offset + 2] = (byte)((@value & 0x0000FF00) >>  8);
			buffer[offset + 3] = (byte)((@value & 0x000000FF) >>  0);
		}
	}
	public void Write(float @value, Span<byte> buffer, int offset = 0)
	{
		if (buffer.Length < offset + 4) throw new IndexOutOfRangeException();
		int @int;
		unsafe { @int = *(int*)&@value; }
		if (this._LittleEndian)
		{
			buffer[offset + 3] = (byte)((@int & 0xFF000000) >> 24);
			buffer[offset + 2] = (byte)((@int & 0x00FF0000) >> 16);
			buffer[offset + 1] = (byte)((@int & 0x0000FF00) >>  8);
			buffer[offset + 0] = (byte)((@int & 0x000000FF) >>  0);
		}
		else
		{
			buffer[offset + 0] = (byte)((@int & 0xFF000000) >> 24);
			buffer[offset + 1] = (byte)((@int & 0x00FF0000) >> 16);
			buffer[offset + 2] = (byte)((@int & 0x0000FF00) >>  8);
			buffer[offset + 3] = (byte)((@int & 0x000000FF) >>  0);
		}
	}
	/* Static Methods */
	// Reading
	public static bool GetBool(ReadOnlySpan<byte> buffer, int offset = 0)
	{
		if (buffer.Length < offset + 1) throw new IndexOutOfRangeException();
		return buffer[offset] != 0;
	}
	public static string GetString(ReadOnlySpan<byte> buffer, int offset = 0, int length = 0)
	{
		if (offset > 0) buffer = buffer.Slice(offset);
		if (length > 0)
		{
			if (buffer.Length < length) throw new IndexOutOfRangeException();
			return Encoding.ASCII.GetString(buffer.Slice(0, length));
		}
		return Encoding.ASCII.GetString(buffer.Slice(0, buffer.IndexOf(byte.MinValue)));
	}
	// Writing
	public static void Write(bool @value, Span<byte> buffer, int offset = 0)
	{
		if (buffer.Length < offset + 1) throw new IndexOutOfRangeException();
		buffer[offset] = (byte)(@value ? 1 : 0);
	}
	public static void Write(string @value, Span<byte> buffer, int offset = 0, int length = 0)
	{
		length = length == 0 ? @value.Length + 1 : length;
		if (buffer.Length < offset + length) throw new IndexOutOfRangeException();
		var valueBytes = Encoding.ASCII.GetBytes(@value);
		for (var i = 0; i < length; ++i)
			buffer[offset + i] = i < valueBytes.Length ? valueBytes[i] : (byte)0;
	}
	/* Properties */
	private readonly bool _LittleEndian;
	/* Class Properties */
	public static readonly JJx.BitConverter BigEndian = new BitConverter(false);
	public static readonly JJx.BitConverter LittleEndian = new BitConverter(true);
}
