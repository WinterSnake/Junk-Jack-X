/*
	Junk Jack X: Core - Tests
	- [Serialization]Enum + Factory

	Written By: Ryan Smith
*/

using System;
using System.IO;

namespace JJx.Core.Serialization.Tests;

public enum UInt8Enum : byte { A = Byte.MinValue, B = Byte.MaxValue, C = 20 }
public enum Int32Enum : int  { A = Int32.MinValue, B = Int32.MaxValue, C = 20, D = -50 }

public partial class SerializationTests
{
	[Theory]
	[InlineData(UInt8Enum.A)]
	[InlineData(UInt8Enum.B)]
	[InlineData(UInt8Enum.C)]
	public void EnumUInt8_RoundTrip(UInt8Enum @value)
	{
		using var ms = new MemoryStream();
		var writer = new JJxWriter(ms);
		writer.Write(@value);
		ms.Position = 0;
		var reader = new JJxReader(ms);
		Assert.Equal(@value, reader.ReadObject<UInt8Enum>());
	}
	[Theory]
	[InlineData(Int32Enum.A)]
	[InlineData(Int32Enum.B)]
	[InlineData(Int32Enum.C)]
	[InlineData(Int32Enum.D)]
	public void EnumInt32_RoundTrip(Int32Enum @value)
	{
		using var ms = new MemoryStream();
		var writer = new JJxWriter(ms);
		writer.Write(@value);
		ms.Position = 0;
		var reader = new JJxReader(ms);
		Assert.Equal(@value, reader.ReadObject<Int32Enum>());
	}
	[Fact]
	public void Enum_Cache_Is_Shared()
	{
		var options = JJxSerializationOptions.Default;
		var first = options.GetConverter<UInt8Enum>();
		var second = options.GetConverter<UInt8Enum>();
		Assert.Same(first, second);
	}
}
