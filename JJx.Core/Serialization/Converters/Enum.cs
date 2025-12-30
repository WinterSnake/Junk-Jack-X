/*
	Junk Jack X: Core
	- [Serialization]Converter - Enum

	Written By: Ryan Smith
*/

using System;
using System.Runtime.CompilerServices;

namespace JJx.Core.Serialization;

internal sealed class EnumConverter<T> : JJxConverter<T>
	where T: struct, Enum
{
	/* Instance Methods */
	public override T Read(ref JJxReader reader)
	{
		return _EnumType switch
		{
			TypeCode.SByte => Unsafe.BitCast<sbyte, T>(reader.ReadInt8()),
			TypeCode.Byte => Unsafe.BitCast<byte, T>(reader.ReadUInt8()),
			TypeCode.Int16 => Unsafe.BitCast<short, T>(reader.ReadInt16()),
			TypeCode.UInt16 => Unsafe.BitCast<ushort, T>(reader.ReadUInt16()),
			TypeCode.Int32 => Unsafe.BitCast<int, T>(reader.ReadInt32()),
			TypeCode.UInt32 => Unsafe.BitCast<uint, T>(reader.ReadUInt32()),
			TypeCode.Int64 => Unsafe.BitCast<long, T>(reader.ReadInt64()),
			TypeCode.UInt64 => Unsafe.BitCast<ulong, T>(reader.ReadUInt64()),
			_ => throw new InvalidOperationException($"EnumConverter<T> does not support typecode '{_EnumType}'"),
		};
	}
	public override void Write(T value, JJxWriter writer)
	{
		switch(_EnumType)
		{
			case TypeCode.SByte: writer.Write(Unsafe.BitCast<T, sbyte>(value)); break;
			case TypeCode.Byte: writer.Write(Unsafe.BitCast<T, byte>(value)); break;
			case TypeCode.Int16: writer.Write(Unsafe.BitCast<T, short>(value)); break;
			case TypeCode.UInt16: writer.Write(Unsafe.BitCast<T, ushort>(value)); break;
			case TypeCode.Int32: writer.Write(Unsafe.BitCast<T, int>(value)); break;
			case TypeCode.UInt32: writer.Write(Unsafe.BitCast<T, uint>(value)); break;
			case TypeCode.Int64: writer.Write(Unsafe.BitCast<T, long>(value)); break;
			case TypeCode.UInt64: writer.Write(Unsafe.BitCast<T, ulong>(value)); break;
			default: throw new InvalidOperationException($"EnumConverter<T> does not support typecode '{_EnumType}'");
		}
	}
	/* Class Properties */
	private static readonly TypeCode _EnumType = Type.GetTypeCode(typeof(T));
}
