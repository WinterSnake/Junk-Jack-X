/*
	Junk Jack X: Core
	- [Serializer]Reader

	Written By: Ryan Smith
*/
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace JJx.Serialization;

public partial struct JJxReader
{
	/* Constructor */
	public JJxReader(Stream stream, ushort bufferSize = 32)
	{
		if (!stream.CanRead)
			throw new ArgumentException("Invalid stream, must be readable");
		this.BaseStream = stream;
		this._Buffer = new byte[bufferSize];
	}
	/* Instance Methods */
	public void Skip(uint numberOfBytes)
	{
		if (this.BaseStream.CanSeek)
		{
			this.BaseStream.Position += numberOfBytes;
			return;
		}
		throw new NotImplementedException("JJxReader.Skip() not implemented for non-seekable streams");
	}
	public Span<byte> GetBytes(int numberOfBytes)
	{
		if (numberOfBytes < 0) throw new ArgumentException("Must read more than 0 bytes");
		byte[] buffer;
		if (numberOfBytes <= this._Buffer.Length)
			buffer = this._Buffer;
		else
			buffer = new byte[numberOfBytes];
		int bytesRead = 0;
		do
		{
			bytesRead += this.BaseStream.Read(buffer.AsSpan(bytesRead, numberOfBytes - bytesRead));
		} while (bytesRead < numberOfBytes);
		Debug.Assert(bytesRead == numberOfBytes, $"Read: {bytesRead} | Expected: {numberOfBytes}");
		if (numberOfBytes < this._Buffer.Length)
			return buffer.AsSpan(0, numberOfBytes);
		return buffer;
	}
	public bool GetBool() => this._InternalReadByte() != 0;
	// Int
	// UInt
	public byte GetUInt8() => this._InternalReadByte();
	public ushort GetUInt16()
	{
		var bytesRead = this.BaseStream.Read(this._Buffer.AsSpan(0, sizeof(ushort)));
		Debug.Assert(bytesRead == sizeof(ushort), $"Read: {bytesRead} | Expected: {sizeof(ushort)}");
		return (ushort)(
			(this._Buffer[1] << 8) |
			(this._Buffer[0] << 0)
		);
	}
	public uint GetUInt32()
	{
		var bytesRead = this.BaseStream.Read(this._Buffer.AsSpan(0, sizeof(uint)));
		Debug.Assert(bytesRead == sizeof(uint), $"Read: {bytesRead} | Expected: {sizeof(uint)}");
		return (uint)(
			(this._Buffer[3] << 24) |
			(this._Buffer[2] << 16) |
			(this._Buffer[1] <<  8) |
			(this._Buffer[0] <<  0)
		);
	}
	// Float
	public unsafe float GetFloat32()
	{
		var flt = this.GetUInt32();
		unsafe { return *(float*)&flt; }
	}
	// String
	public string GetString(ulong length = 0, bool isLengthEncoded = false)
	{
		byte[] buffer;
		if (length > 0) buffer = new byte[length];  // TODO: Use buffer if small enough length
		else
		{
			throw new NotImplementedException("Non-length string calls for Reader.GetString() not implemented yet.");
		}
		// Read string
		if (buffer.Length == 0) return String.Empty;
		var bytesRead = this.BaseStream.Read(buffer);
		Debug.Assert(bytesRead == buffer.Length, $"Read: {bytesRead} | Expected: {buffer.Length}");
		// Get null terminal
		var endSequence = Array.IndexOf(buffer, Byte.MinValue);
		endSequence = endSequence > 0 ? endSequence : buffer.Length;
		// Return string
		return Encoding.ASCII.GetString(buffer.AsSpan(0, endSequence));
	}
	// Object
	#nullable enable
	public T Get<T>(bool compressed = false, params object?[]? properties)
	{
		Type type = typeof(T);
		// Get converter
		var converter = (JJxConverter.GetTypeConverter(type, properties) as JJxConverter<T>);
		if (compressed)
			converter = new CompressionConverter<T>(converter);
		return converter!.Read(this);
	}
	#nullable disable
	private byte _InternalReadByte()
	{
		var data = this.BaseStream.ReadByte();
		if (data < 0) throw new ArgumentException("Stream at end");
		return (byte)data;
	}
	/* Properties */
	public readonly Stream BaseStream;
	private readonly byte[] _Buffer;
}
