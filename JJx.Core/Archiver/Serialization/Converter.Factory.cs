/*
	Junk Jack X: Core
	- [Serializer]Converter

	Written By: Ryan Smith
*/
using System;
using System.Collections.Generic;
using System.Reflection;

namespace JJx.Serialization;

// TODO: Cache
public abstract class JJxConverterFactory
{
	/* Instance Methods */
	public abstract bool CanConvert(Type type);
	public abstract JJxConverter Build(Type type);
	protected JJxConverter _CreateConverter(Type converterType)
	{
		var converterCTor = converterType.GetConstructors()[0];  // TODO: factory ctor binding
		return (JJxConverter)converterCTor.Invoke(null);
	}
	/* Properties */
	protected readonly Dictionary<Type, JJxConverter> _CachedConverters = new();
	/* Class Properties */
	internal static readonly JJxConverterFactory[] Defaults = {
		new EnumConverterFactory(),
		new JJxObjectConverterFactory(),
	};
}
