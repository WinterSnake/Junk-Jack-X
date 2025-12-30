/*
	Junk Jack X: Core
	- [Serialization]Converter Factory

	Written By: Ryan Smith
*/

using System;

namespace JJx.Core.Serialization;

public abstract class JJxConverterFactory : JJxConverter
{
	/* Instance Methods */
	public abstract JJxConverter Build(Type type);
}
