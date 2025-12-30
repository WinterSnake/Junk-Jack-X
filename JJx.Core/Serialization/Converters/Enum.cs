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
			TypeCode.UInt16 => Unsafe.BitCast<ushort, T>(reader.ReadUInt16()),
			_ => throw new InvalidOperationException($"EnumConverter<T> does not support typecode '{_EnumType}'"),
		};
	}
	public override void Write(T value, JJxWriter writer)
	{
		switch(_EnumType)
		{
			case TypeCode.UInt16: writer.Write(Unsafe.BitCast<T, ushort>(value)); break;
			default: throw new InvalidOperationException($"EnumConverter<T> does not support typecode '{_EnumType}'");
		}
	}
	/* Class Properties */
	private static readonly TypeCode _EnumType = Type.GetTypeCode(typeof(T));
}
