/*
	Junk Jack X: Core
	- [Serializer.Converters]Enum

	Written By: Ryan Smith
*/
using System;
using System.Runtime.CompilerServices;

namespace JJx.Serialization;

internal sealed class EnumConverterFactory : JJxConverterFactory
{
	/* Instance Methods */
	public override bool CanConvert(Type type) => type.IsEnum;
	public override JJxConverter Build(Type type)
	{
		Type typeConverter = typeof(EnumConverter<>).MakeGenericType(type);
		return base._Build(typeConverter);
	}
}

internal sealed class EnumConverter<T> : JJxConverter<T> where T : Enum
{
	/* Instance Methods */
	public override T Deserialize(JJxReader reader)
	{
		var typeCode = Type.GetTypeCode(typeof(T));
		switch (typeCode)
		{
			case TypeCode.Byte:
			{
				var @value = reader.GetUInt8();
				return Unsafe.As<byte, T>(ref @value);
			}
			case TypeCode.UInt16:
			{
				var @value = reader.GetUInt16();
				return Unsafe.As<ushort, T>(ref @value);
			}
			case TypeCode.UInt32:
			{
				var @value = reader.GetUInt32();
				return Unsafe.As<uint, T>(ref @value);
			}
			default: throw new ArgumentException($"Unhandled enum typecode '{typeCode}' in EnumConverter");
		}
	}
}
