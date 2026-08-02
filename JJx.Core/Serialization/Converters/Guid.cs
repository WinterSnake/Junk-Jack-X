/*
	Junk Jack X: Core
	- [Serialization]Converter - Guid

	Written By: Ryan Smith
*/

using System;
using System.Diagnostics;

namespace JJx.Core.Serialization;

internal sealed class GuidConverter : JJxConverter<Guid>
{
	/* Instance Methods */
	public override Guid Read(ref JJxReader reader, JJxSerializationOptions options)
	{
		Span<byte> buffer = stackalloc byte[SIZE];
		reader.CopyTo(buffer);
		return new Guid(buffer);
	}
	public override void Write(in Guid @value, JJxWriter writer, JJxSerializationOptions options)
	{
		Span<byte> buffer = stackalloc byte[SIZE];
		Debug.Assert(@value.TryWriteBytes(buffer));
		writer.Write(buffer);
	}
	/* Class Properties */
	private const int SIZE = 16;
}
