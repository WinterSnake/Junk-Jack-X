/*
	Junk Jack X: Core
	- [Archiver]Converter

	Written By: Ryan Smith
*/
using System;
using System.Collections.Generic;

namespace JJx.Serialization;

public abstract class JJxConverter
{
	/* Properties */
	public abstract Type Type { get; }
	/* Class Properties */
	public static readonly Dictionary<Type, JJxConverter> Defaults = new() {
		{ typeof(ArchiverChunk), new ArchiverChunkConverter() },
	};
}

public abstract class JJxConverter<T> : JJxConverter
{
	/* Instance Methods */
	public abstract T Deserialize(JJxReader reader);
	/* Properties */
	public sealed override Type Type { get => typeof(T); }
}
