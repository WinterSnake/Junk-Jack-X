/*
	Junk Jack X: Core
	- [Serialization]Reader

	Written By: Ryan Smith
*/

using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace JJx.Core.Serialization;

internal ref struct JJxReader
{
	/* Constructor */
	public JJxReader(ReadOnlySpan<byte> buffer) => this._Buffer = buffer;
	/* Instance Methods */
	public bool ReadBool()
	{
		var @value = Convert.ToBoolean(this._Buffer[0]);
		this._Buffer = this._Buffer.Slice(sizeof(bool));
		return @value;
	}
	public byte ReadUInt8()
	{
		var @value = this._Buffer[0];
		this._Buffer = this._Buffer.Slice(sizeof(byte));
		return @value;
	}
	public ushort ReadUInt16()
	{
		var @value = BinaryPrimitives.ReadUInt16LittleEndian(this._Buffer);
		this._Buffer = this._Buffer.Slice(sizeof(ushort));
		return @value;
	}
	public ushort ReadUInt16BE()
	{
		var @value = BinaryPrimitives.ReadUInt16BigEndian(this._Buffer);
		this._Buffer = this._Buffer.Slice(sizeof(ushort));
		return @value;
	}
	public int ReadInt32()
	{
		var @value = BinaryPrimitives.ReadInt32LittleEndian(this._Buffer);
		this._Buffer = this._Buffer.Slice(sizeof(int));
		return @value;
	}
	public uint ReadUInt32()
	{
		var @value = BinaryPrimitives.ReadUInt32LittleEndian(this._Buffer);
		this._Buffer = this._Buffer.Slice(sizeof(uint));
		return @value;
	}
	public string ReadString(int length = 0, int maxCapacity = 128)
	{
		Debug.Assert(length > 0);
		byte[]? storage = null;
		Span<byte> buffer = length > maxCapacity
			? (storage = ArrayPool<byte>.Shared.Rent(length)).AsSpan(0, length)
			: stackalloc byte[length];
		try {
			this._Buffer[..length].CopyTo(buffer);
			var terminator = buffer.IndexOf<byte>(0);
			buffer = terminator == -1 ? buffer : buffer[..terminator];
			this._Buffer = this._Buffer.Slice(length);
			return Encoding.UTF8.GetString(buffer);
		} finally {
			if (storage is not null) ArrayPool<byte>.Shared.Return(storage);
		}
	}
	public void CopyTo(scoped Span<byte> buffer)
	{
		this._Buffer[..buffer.Length].CopyTo(buffer);
		this._Buffer = this._Buffer.Slice(buffer.Length);
	}
	public T ReadObject<T>()
		=> JJxSerializationOptions.Default.GetConverter<T>().Read(ref this);
	/* Properties */
	private ReadOnlySpan<byte> _Buffer;
	public int Remaining => this._Buffer.Length;
}
