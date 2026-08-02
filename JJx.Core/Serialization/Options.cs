/*
	Junk Jack X: Core
	- [Serialization]Options

	Written By: Ryan Smith
*/

using System;
using System.Collections.Generic;
using JJx.Core.Metadata;

namespace JJx.Core.Serialization;

public sealed class JJxSerializationOptions
{
	/* Constructor */
	public JJxSerializationOptions(bool isPacked) => this.IsPacked = isPacked;
	/* Instance Methods */
	internal void AddConverter<T>() where T: JJxConverter, new() => this._Converters.Add(new T());
	internal void AddConverter<T>(Type lookupType) where T: JJxConverter, new() => this._ConverterCache[lookupType] = new T();
	internal JJxConverter<T> GetConverter<T>()
	{
		var converterType = typeof(T);
		if (this._ConverterCache.TryGetValue(converterType, out var cachedConverter))
			return (JJxConverter<T>)cachedConverter;
		while (converterType != null)
		{
			for (var i = 0; i < this._Converters.Count; ++i)
			{
				var converter = this._Converters[i];
				if (!converter.CanSupportType(converterType)) continue;
				if (converter is JJxConverterFactory converterFactory)
					converter = converterFactory.Build(converterType);
				this._ConverterCache[converterType] = converter;
				return (JJxConverter<T>)converter;
			}
			converterType = converterType.BaseType;
		}
		throw new InvalidOperationException($"JJx format does not support the type '{typeof(T).Name}'");
	}
	/* Properties */
	public readonly bool IsPacked;
	private readonly List<JJxConverter> _Converters = new();
	private readonly Dictionary<Type, JJxConverter> _ConverterCache = new();
	/* Class Properties */
	public static JJxSerializationOptions Default => JJxDefaultSerializationOptions.Options;
}
