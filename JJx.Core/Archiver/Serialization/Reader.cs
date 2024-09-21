/*
	Junk Jack X: Core
	- [Serializer]Reader

	Written By: Ryan Smith
*/
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace JJx;
using Serialization;

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
	public Span<byte> GetBytes(int numberOfBytes)
	{
		if (numberOfBytes < 0) throw new ArgumentException("Must read more than 0 bytes");
		byte[] buffer;
		if (numberOfBytes <= this._Buffer.Length)
			buffer = this._Buffer;
		else
			buffer = new byte[numberOfBytes];
		var bytesRead = this.BaseStream.Read(buffer.AsSpan(0, numberOfBytes));
		Debug.Assert(bytesRead == numberOfBytes, $"Read: {bytesRead} | Expected: {numberOfBytes}");
		return buffer.AsSpan(0, numberOfBytes);
	}
	public bool GetBool() => this._InternalReadByte() != 0;
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
	public string GetString(ulong length = 0, bool isLengthEncoded = false)
	{
		byte[] buffer;
		if (length > 0) buffer = new byte[length];
		else
		{
			// Calculate buffer
			buffer = new byte[this._Buffer.Length]; // temporary
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
	public T Get<T>()
	{
		Type type = typeof(T);
		// Get converter
		JJxConverter converter = null;
		if (JJxConverter.Defaults.ContainsKey(type))
			converter = JJxConverter.Defaults[type];
		else
		{
			// Build factory converter
			foreach (var factoryConverter in JJxConverterFactory.Defaults)
			{
				if (factoryConverter.CanConvert(type))
				{
					converter = factoryConverter.Build(type);
					break;
				}
			}
		}
		if (converter == null) throw new ArgumentException($"Unhandled type '{type}' in Reader.Get()");
		return (converter as JJxConverter<T>).Deserialize(this);
	}
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
