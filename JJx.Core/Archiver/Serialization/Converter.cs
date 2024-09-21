/*
	Junk Jack X: Core
	- [Serializer]Converter

	Written By: Ryan Smith
*/
using System;
using System.Collections.Generic;

namespace JJx.Serialization;

// TODO: Cache
public abstract class JJxConverter
{
	/* Static Methods */
	internal static JJxConverter GetTypeConverter(Type type)
	{
		if (JJxConverter._InternalConverters.ContainsKey(type))
		{
			#if DEBUG
				Console.WriteLine($"Found existing converter for {type}");
			#endif
			return JJxConverter._InternalConverters[type];
		}
		// Build factory converter
		foreach (var factoryConverter in JJxConverterFactory._InternalFactories)
		{
			if (factoryConverter.CanConvert(type))
				return factoryConverter.Build(type);
		}
		throw new ArgumentException($"Unhandled type '{type}' in JJxConverters");
	}
	/* Properties */
	#nullable enable
	public abstract Type? Type { get; }
	#nullable disable
	/* Class Properties */
	internal static readonly Dictionary<Type, JJxConverter> _InternalConverters = new() {
		// System
		{ typeof(DateTime), new DateTimeConverter() },
		{ typeof(Guid), new GuidConverter() },
		// JJx
		{ typeof(Character), new CharacterConverter() },
	};
}

public abstract class JJxConverter<T> : JJxConverter
{
	/* Instance Methods */
	public abstract T Read(JJxReader reader);
	public abstract void Write(JJxWriter writer, object @value);
	/* Properties */
	#nullable enable
	public sealed override Type? Type => typeof(T);
	#nullable disable
}
