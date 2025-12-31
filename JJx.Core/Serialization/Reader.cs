/*
	Junk Jack X: Core
	- [Archiver]Reader

	Written By: Ryan Smith
*/

using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace JJx.Core.Serialization;

public ref struct JJxReader
{
	/* Constructor */
	public JJxReader(Stream stream) => this._Stream = stream;
	/* Instance Methods */
	public void Skip(int count)
	{
		if (count <= 0) return;
		if (!this._Stream.CanSeek)
			throw new InvalidOperationException("Skipping not supported on non-seekable streams");
		this._Stream.Seek(count, SeekOrigin.Current);
	}
	public bool ReadBool()
	{
		Span<byte> buffer = stackalloc byte[sizeof(byte)];
		this._Stream.ReadExactly(buffer);
		return Convert.ToBoolean(buffer[0]);
	}
	public sbyte ReadInt8()
	{
		Span<byte> buffer = stackalloc byte[sizeof(sbyte)];
		this._Stream.ReadExactly(buffer);
		return (sbyte)buffer[0];
	}
	public byte ReadUInt8()
	{
		Span<byte> buffer = stackalloc byte[sizeof(byte)];
		this._Stream.ReadExactly(buffer);
		return buffer[0];
	}
	public short ReadInt16()
	{
		Span<byte> buffer = stackalloc byte[sizeof(short)];
		this._Stream.ReadExactly(buffer);
		return BinaryPrimitives.ReadInt16LittleEndian(buffer);
	}
	public ushort ReadUInt16()
	{
		Span<byte> buffer = stackalloc byte[sizeof(ushort)];
		this._Stream.ReadExactly(buffer);
		return BinaryPrimitives.ReadUInt16LittleEndian(buffer);
	}
	public int ReadInt32()
	{
		Span<byte> buffer = stackalloc byte[sizeof(int)];
		this._Stream.ReadExactly(buffer);
		return BinaryPrimitives.ReadInt32LittleEndian(buffer);
	}
	public uint ReadUInt32()
	{
		Span<byte> buffer = stackalloc byte[sizeof(uint)];
		this._Stream.ReadExactly(buffer);
		return BinaryPrimitives.ReadUInt32LittleEndian(buffer);
	}
	public long ReadInt64()
	{
		Span<byte> buffer = stackalloc byte[sizeof(long)];
		this._Stream.ReadExactly(buffer);
		return BinaryPrimitives.ReadInt64LittleEndian(buffer);
	}
	public ulong ReadUInt64()
	{
		Span<byte> buffer = stackalloc byte[sizeof(ulong)];
		this._Stream.ReadExactly(buffer);
		return BinaryPrimitives.ReadUInt64LittleEndian(buffer);
	}
	public float ReadFloat32()
	{
		Span<byte> buffer = stackalloc byte[sizeof(float)];
		this._Stream.ReadExactly(buffer);
		return BinaryPrimitives.ReadSingleLittleEndian(buffer);
	}
	public double ReadFloat64()
	{
		Span<byte> buffer = stackalloc byte[sizeof(double)];
		this._Stream.ReadExactly(buffer);
		return BinaryPrimitives.ReadDoubleLittleEndian(buffer);
	}
	public string ReadString(int length = 0)
	{
		if (length == 0)
			length = this.ReadInt32();
		byte[]? storage = null;
		Span<byte> buffer = length > 128 ? (storage = ArrayPool<byte>.Shared.Rent(length)).AsSpan(0, length) : stackalloc byte[length];
		try {
			this._Stream.ReadExactly(buffer);
			var terminator = buffer.IndexOf((byte)0);
			buffer = terminator != -1 ? buffer.Slice(0, terminator) : buffer;
			return Encoding.UTF8.GetString(buffer);
		} finally {
			if (storage is not null)
				ArrayPool<byte>.Shared.Return(storage);
		}
	}
	public void ReadSpan(scoped Span<byte> buffer) => this._Stream.ReadExactly(buffer);
	public T ReadObject<T>() => JJxSerializationOptions.Default.GetConverter<T>().Read(ref this);
	/* Properties */
	private readonly Stream _Stream;
}
