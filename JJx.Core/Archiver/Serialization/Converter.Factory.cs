/*
	Junk Jack X: Core
	- [Serializer]Converter

	Written By: Ryan Smith
*/
using System;
using System.Collections.Generic;
using System.Reflection;

namespace JJx.Serialization;

public abstract class JJxConverterFactory
{
	/* Instance Methods */
	public abstract bool CanConvert(Type type);
	public abstract JJxConverter Build(Type type);
	protected JJxConverter _CreateConverter(Type baseType, Type converterType)
	{
		#if DEBUG
			Console.WriteLine($"Creating new [{baseType}]Converter with {converterType}");
		#endif
		var converterCTor = converterType.GetConstructor(new Type[0]);
		var converter = (JJxConverter)converterCTor.Invoke(null);
		JJxConverter._InternalConverters.Add(baseType, converter);
		return converter;
	}
	protected JJxConverter _CreateConverter(Type baseType, Type converterType, params (Type type, object @value)[] arguments)
	{
		#if DEBUG
			Console.WriteLine($"Creating new [{baseType}]Converter with {converterType}");
		#endif
		// Create type array for constructor
		var cTorTypes = new Type[arguments.Length];
		for (var i = 0; i < cTorTypes.Length; ++i)
			cTorTypes[i] = arguments[i].type;
		var converterCTor = converterType.GetConstructor(cTorTypes);
		// Create value array for parameters
		var cTorValues = new object[arguments.Length];
		for (var i = 0; i < cTorValues.Length; ++i)
			cTorValues[i] = arguments[i].@value;
		var converter = (JJxConverter)converterCTor.Invoke(cTorValues);
		JJxConverter._InternalConverters.Add(baseType, converter);
		return converter;
	}
	/* Class Properties */
	internal static readonly JJxConverterFactory[] _InternalFactories = {
		// System
		new EnumConverterFactory(),
		// JJx
		new JJxObjectConverterFactory(),
	};
}
