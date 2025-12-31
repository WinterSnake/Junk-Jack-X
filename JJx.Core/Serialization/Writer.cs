/*
	Junk Jack X: Core
	- [Archiver]Writer

	Written By: Ryan Smith
*/

using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace JJx.Core.Serialization;

public sealed class JJxWriter
{
	/* Constructor */
	public JJxWriter(Stream stream) => this._Stream = stream;
	/* Instance Methods */
	public void Skip(int count)
	{
		if (count <= 0) return;
		if (!this._Stream.CanSeek)
			throw new InvalidOperationException("Skipping not supported on non-seekable streams");
		this._Stream.Seek(count, SeekOrigin.Current);
	}
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
	public void Write(string @value, int length = 0)
	{
		if (length == 0)
		{
			length = Encoding.UTF8.GetByteCount(@value);
			this.Write(length);
		}
		byte[]? storage = null;
		Span<byte> buffer = length > 128 ? (storage = ArrayPool<byte>.Shared.Rent(length)).AsSpan(0, length) : stackalloc byte[length];
		try {
			buffer.Clear();
			Encoding.UTF8.GetBytes(@value, buffer);
			this._Stream.Write(buffer);
		} finally {
			if (storage is not null)
				ArrayPool<byte>.Shared.Return(storage);
		}
	}
	public void Write(ReadOnlySpan<byte> @value) => this._Stream.Write(@value);
	public void Write<T>(T @value) => JJxSerializationOptions.Default.GetConverter<T>().Write(@value, this);
	/* Properties */
	private readonly Stream _Stream;
}
