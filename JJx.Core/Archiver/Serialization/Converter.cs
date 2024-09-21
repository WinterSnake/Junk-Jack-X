/*
	Junk Jack X: Core
	- [Serializer]Converter

	Written By: Ryan Smith
*/
using System;
using System.Collections.Generic;

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

public abstract class JJxConverter<T> : JJxConverter
{
	/* Instance Methods */
	public abstract T Read(JJxReader reader);
	/* Properties */
	#nullable enable
	public sealed override Type? Type => typeof(T);
	#nullable disable
}
