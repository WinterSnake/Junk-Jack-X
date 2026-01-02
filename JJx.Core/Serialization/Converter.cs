/*
	Junk Jack X: Core
	- [Serialization]Converter

	Written By: Ryan Smith
*/

using System;

namespace JJx.Core.Serialization;

public abstract class JJxConverter
{
	/* Instance Methods */
	public abstract bool CanSupportType(Type type);
}

public abstract class JJxConverter<T> : JJxConverter
{
	/* Instance Methods */
    public override bool CanSupportType(Type type) => type == typeof(T);
	public abstract T Read(ref JJxReader reader);
	public abstract void Write(in T value, JJxWriter writer);
}

public abstract class JJxConverterFactory : JJxConverter
{
	/* Instance Methods */
	public abstract JJxConverter Build(Type type);
}
