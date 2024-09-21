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
		return base._CreateConverter(type, typeConverter);
	}
}

// TODO: Cache
internal sealed class EnumConverter<T> : JJxConverter<T> where T : Enum
{
	/* Instance Methods */
	public override T Read(JJxReader reader)
	{
		var typeCode = Type.GetTypeCode(typeof(T));
		switch (typeCode)
		{
			case TypeCode.Byte:
			{
				byte @value = reader.GetUInt8();
				return Unsafe.As<byte, T>(ref @value);
			}
			case TypeCode.UInt16:
			{
				ushort @value = reader.GetUInt16();
				return Unsafe.As<ushort, T>(ref @value);
			}
			case TypeCode.UInt32:
			{
				uint @value = reader.GetUInt32();
				return Unsafe.As<uint, T>(ref @value);
			}
			default: throw new ArgumentException($"Unhandled enum typecode '{typeCode}' in EnumConverter");
		}
	}
}
