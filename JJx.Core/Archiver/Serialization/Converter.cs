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
	#nullable enable
	internal static JJxConverter GetTypeConverter(Type type, params object?[]? properties)
	{
		if (properties != null && properties.Length == 0) properties = null;
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
			Console.WriteLine($"Checking {type} against {factoryConverter.GetType()}");
			if (factoryConverter.CanConvert(type))
				return factoryConverter.Build(type, properties);
		}
		throw new ArgumentException($"Unhandled type '{type}' in JJxConverters");
	}
	#nullable disable
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
	public virtual T Read(JJxReader reader)
		=> throw new NotImplementedException($"{this.GetType()} does not implement Read()");
	public virtual void Write(JJxWriter writer, object @value)
		=> throw new NotImplementedException($"{this.GetType()} does not implement Write()");
	/* Properties */
	#nullable enable
	public sealed override Type? Type => typeof(T);
	#nullable disable
}
