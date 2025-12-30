/*
	Junk Jack X: Core
	- [Serialization]Converter - Enum Factory

	Written By: Ryan Smith
*/

using System;

namespace JJx.Core.Serialization;

public sealed class EnumFactoryConverter : JJxConverterFactory
{
    /* Instance Methods */
    public override bool CanSupportType(Type type) => type.IsEnum;
    public override JJxConverter Build(Type type)
    {
		var converterType = typeof(EnumConverter<>).MakeGenericType(type);
		return (JJxConverter)Activator.CreateInstance(converterType)!;
    }
}
