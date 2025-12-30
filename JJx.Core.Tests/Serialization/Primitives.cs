/*
	Junk Jack X: Core - Tests
	- [Serialization]Primitive

	Written By: Ryan Smith
*/

using System;
using System.IO;

namespace JJx.Core.Serialization.Tests;

public partial class SerializationTests
{
	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void Boolean_RoundTrip(bool @value)
	{
		using var ms = new MemoryStream();
		var writer = new JJxWriter(ms);
		writer.Write(@value);
		ms.Position = 0;
		var reader = new JJxReader(ms);
		Assert.Equal(@value, reader.ReadBool());
	}
	[Theory]
	[InlineData(-1)]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(SByte.MinValue)]
	[InlineData(SByte.MaxValue)]
	public void Int8_RoundTrip(sbyte @value)
	{
		using var ms = new MemoryStream();
		var writer = new JJxWriter(ms);
		writer.Write(@value);
		ms.Position = 0;
		var reader = new JJxReader(ms);
		Assert.Equal(@value, reader.ReadInt8());
	}
	[Theory]
	[InlineData(Byte.MinValue)]
	[InlineData(Byte.MaxValue / 2)]
	[InlineData(Byte.MaxValue)]
	public void UInt8_RoundTrip(byte @value)
	{
		using var ms = new MemoryStream();
		var writer = new JJxWriter(ms);
		writer.Write(@value);
		ms.Position = 0;
		var reader = new JJxReader(ms);
		Assert.Equal(@value, reader.ReadUInt8());
	}
	[Theory]
	[InlineData(-1)]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(Int16.MinValue)]
	[InlineData(Int16.MaxValue)]
	public void Int16_RoundTrip(short @value)
	{
		using var ms = new MemoryStream();
		var writer = new JJxWriter(ms);
		writer.Write(@value);
		ms.Position = 0;
		var reader = new JJxReader(ms);
		Assert.Equal(@value, reader.ReadInt16());
	}
	[Theory]
	[InlineData(UInt16.MinValue)]
	[InlineData(UInt16.MaxValue / 2)]
	[InlineData(UInt16.MaxValue)]
	public void UInt16_RoundTrip(ushort @value)
	{
		using var ms = new MemoryStream();
		var writer = new JJxWriter(ms);
		writer.Write(@value);
		ms.Position = 0;
		var reader = new JJxReader(ms);
		Assert.Equal(@value, reader.ReadUInt16());
	}
	[Theory]
	[InlineData(-1)]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(Int32.MinValue)]
	[InlineData(Int32.MaxValue)]
	public void Int32_RoundTrip(int @value)
	{
		using var ms = new MemoryStream();
		var writer = new JJxWriter(ms);
		writer.Write(@value);
		ms.Position = 0;
		var reader = new JJxReader(ms);
		Assert.Equal(@value, reader.ReadInt32());
	}
	[Theory]
	[InlineData(UInt32.MinValue)]
	[InlineData(UInt32.MaxValue / 2)]
	[InlineData(UInt32.MaxValue)]
	public void UInt32_RoundTrip(uint @value)
	{
		using var ms = new MemoryStream();
		var writer = new JJxWriter(ms);
		writer.Write(@value);
		ms.Position = 0;
		var reader = new JJxReader(ms);
		Assert.Equal(@value, reader.ReadUInt32());
	}
	[Theory]
	[InlineData(-1)]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(Int64.MinValue)]
	[InlineData(Int64.MaxValue)]
	public void Int64_RoundTrip(long @value)
	{
		using var ms = new MemoryStream();
		var writer = new JJxWriter(ms);
		writer.Write(@value);
		ms.Position = 0;
		var reader = new JJxReader(ms);
		Assert.Equal(@value, reader.ReadInt64());
	}
	[Theory]
	[InlineData(UInt64.MinValue)]
	[InlineData(UInt64.MaxValue / 2)]
	[InlineData(UInt64.MaxValue)]
	public void UInt64_RoundTrip(ulong @value)
	{
		using var ms = new MemoryStream();
		var writer = new JJxWriter(ms);
		writer.Write(@value);
		ms.Position = 0;
		var reader = new JJxReader(ms);
		Assert.Equal(@value, reader.ReadUInt64());
	}
}
