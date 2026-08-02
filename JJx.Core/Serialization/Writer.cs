/*
	Junk Jack X: Core
	- [Serialization]Writer

	Written By: Ryan Smith
*/

using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Text;

namespace JJx.Core.Serialization;

internal sealed class JJxWriter
{
	/* Constructor */
	public JJxWriter(IBufferWriter<byte> writer) => this._Writer = writer;
	/* Instance Methods */
	public void Advance(int length)
	{
		this._Writer.GetSpan(length).Clear();
		this._Writer.Advance(length);
	}
	public void Write(bool @value)
	{
		var size = sizeof(bool);
		this._Writer.GetSpan(size)[0] = Convert.ToByte(@value);
		this._Writer.Advance(size);
	}
	public void Write(byte @value)
	{
		var size = sizeof(byte);
		this._Writer.GetSpan(size)[0] = @value;
		this._Writer.Advance(size);
	}
	public void Write(ushort @value)
	{
		var size = sizeof(ushort);
		BinaryPrimitives.WriteUInt16LittleEndian(this._Writer.GetSpan(size), @value);
		this._Writer.Advance(size);
	}
	public void Write(int @value)
	{
		var size = sizeof(int);
		BinaryPrimitives.WriteInt32LittleEndian(this._Writer.GetSpan(size), @value);
		this._Writer.Advance(size);
	}
	public void Write(uint @value)
	{
		var size = sizeof(uint);
		BinaryPrimitives.WriteUInt32LittleEndian(this._Writer.GetSpan(size), @value);
		this._Writer.Advance(size);
	}
	public void Write(float @value)
	{
		var size = sizeof(float);
		BinaryPrimitives.WriteSingleLittleEndian(this._Writer.GetSpan(size), @value);
		this._Writer.Advance(size);
	}
	public void Write(string @value, int length = 0)
	{
		Debug.Assert(length > 0);
		var span = this._Writer.GetSpan(length);
		var written = Encoding.UTF8.GetBytes(@value, span);
		span[written..].Clear();
		this._Writer.Advance(length);
	}
	public void Write(ReadOnlySpan<byte> buffer)
	{
		buffer.CopyTo(this._Writer.GetSpan(buffer.Length));
		this._Writer.Advance(buffer.Length);
	}
	public void Write<T>(in T @value, JJxSerializationOptions options) => options.GetConverter<T>().Write(@value, this, options);
	/* Properties */
	private readonly IBufferWriter<byte> _Writer;
}
