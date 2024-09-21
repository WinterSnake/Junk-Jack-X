/*
	Junk Jack X: Core
	- [Serializer]Converter

	Written By: Ryan Smith
*/
using System;
using System.Collections.Generic;
using System.Reflection;

namespace JJx.Serialization;

public abstract class JJxConverter
{
	/* Properties */
	#nullable enable
	public abstract Type? Type { get; }
	#nullable disable
	/* Class Properties */
	internal static readonly Dictionary<Type, JJxConverter> Defaults = new() {
		// System
		{ typeof(Guid), new GuidConverter() },
		// JJx
		{ typeof(ArchiverChunk), new ArchiverChunkConverter() },
		{ typeof(Character), new CharacterConverter() },
	};
}

public abstract class JJxConverterFactory : JJxConverter
{
	/* Instance Methods */
	public abstract bool CanConvert(Type type);
	public abstract JJxConverter Build(Type type);
	protected JJxConverter _Build(Type type)
	{
		var converterCTor = type.GetConstructors()[0];  // TODO: factory ctor binding
		return (JJxConverter)converterCTor.Invoke(null);
	}
	/* Properties */
	#nullable enable
	public sealed override Type? Type => null;
	#nullable disable
	/* Class Properties */
	internal static readonly new JJxConverterFactory[] Defaults = {
		new EnumConverterFactory(),
	};
}

public abstract class JJxConverter<T> : JJxConverter
{
	/* Instance Methods */
	public abstract T Deserialize(JJxReader reader);
	/* Properties */
	#nullable enable
	public sealed override Type? Type => typeof(T);
	#nullable disable
}
