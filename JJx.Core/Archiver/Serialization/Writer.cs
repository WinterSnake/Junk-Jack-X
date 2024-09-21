/*
	Junk Jack X: Core
	- [Serializer]Writer

	Written By: Ryan Smith
*/
using System;
using System.IO;

namespace JJx.Serialization;

public partial struct JJxWriter
{
	/* Constructor */
	public JJxWriter(Stream stream, ushort bufferSize = 32)
	{
		this.BaseStream = stream;
		this._Buffer = new byte[bufferSize];
	}
	/* Instance Methods */
	public void Write(ReadOnlySpan<byte> @value)
	{
		throw new NotImplementedException("Writer.Write<byte[]> not implemented");
	}
	public void Write(bool @value) => this.Write((byte)(@value ? 1 : 0));
	// Int
	// UInt
	public void Write(byte @value)
	{
		throw new NotImplementedException("Writer.Write<byte> not implemented");
	}
	public void Write(ushort @value)
	{
		throw new NotImplementedException("Writer.Write<ushort> not implemented");
	}
	public void Write(uint @value)
	{
		throw new NotImplementedException("Writer.Write<uint> not implemented");
	}
	// Float
	public void Write(float @value)
	{
		throw new NotImplementedException("Writer.Write<float> not implemented");
	}
	// String
	public void Write(string @value, bool lengthEncoded = false)
	{
		throw new NotImplementedException("Writer.Write<string> not implemented");
	}
	public void Write(object @value)
	{
		throw new NotImplementedException("Writer.Write<object> not implemented");
	}
	/* Properties */
	public readonly Stream BaseStream;
	private readonly byte[] _Buffer;
}
