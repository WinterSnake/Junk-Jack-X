/*
	Junk Jack X: Core
	- [Archiver]Writer

	Written By: Ryan Smith
*/

using System;
using System.Buffers.Binary;
using System.IO;

namespace JJx.Core.Serialization;

public sealed class JJxWriter
{
	/* Constructor */
	public JJxWriter(Stream stream) => this._Stream = stream;
	/* Instance Methods */
	public void Write(bool @value)
	{
		Span<byte> buffer = stackalloc byte[sizeof(byte)];
		buffer[0] = Convert.ToByte(value);
		this._Stream.Write(buffer);
	}
	public void Write(sbyte @value)
	{
		Span<byte> buffer = stackalloc byte[sizeof(sbyte)];
		buffer[0] = (byte)value;
		this._Stream.Write(buffer);
	}
	public void Write(byte @value)
	{
		Span<byte> buffer = stackalloc byte[sizeof(byte)];
		buffer[0] = value;
		this._Stream.Write(buffer);
	}
	public void Write(short @value)
	{
		Span<byte> buffer = stackalloc byte[sizeof(short)];
		BinaryPrimitives.WriteInt16LittleEndian(buffer, @value);
		this._Stream.Write(buffer);
	}
	public void Write(ushort @value)
	{
		Span<byte> buffer = stackalloc byte[sizeof(ushort)];
		BinaryPrimitives.WriteUInt16LittleEndian(buffer, @value);
		this._Stream.Write(buffer);
	}
	public void Write(int @value)
	{
		Span<byte> buffer = stackalloc byte[sizeof(int)];
		BinaryPrimitives.WriteInt32LittleEndian(buffer, @value);
		this._Stream.Write(buffer);
	}
	public void Write(uint @value)
	{
		Span<byte> buffer = stackalloc byte[sizeof(uint)];
		BinaryPrimitives.WriteUInt32LittleEndian(buffer, @value);
		this._Stream.Write(buffer);
	}
	public void Write(long @value)
	{
		Span<byte> buffer = stackalloc byte[sizeof(long)];
		BinaryPrimitives.WriteInt64LittleEndian(buffer, @value);
		this._Stream.Write(buffer);
	}
	public void Write(ulong @value)
	{
		Span<byte> buffer = stackalloc byte[sizeof(ulong)];
		BinaryPrimitives.WriteUInt64LittleEndian(buffer, @value);
		this._Stream.Write(buffer);
	}
	public void Write(float @value)
	{
		Span<byte> buffer = stackalloc byte[sizeof(float)];
		BinaryPrimitives.WriteSingleLittleEndian(buffer, @value);
		this._Stream.Write(buffer);
	}
	public void Write(double @value)
	{
		Span<byte> buffer = stackalloc byte[sizeof(double)];
		BinaryPrimitives.WriteDoubleLittleEndian(buffer, @value);
		this._Stream.Write(buffer);
	}
	public void Write(ReadOnlySpan<byte> @value) => this._Stream.Write(@value);
	public void Write<T>(T @value) => JJxSerializationOptions.Default.GetConverter<T>().Write(@value, this);
	/* Properties */
	private readonly Stream _Stream;
}
