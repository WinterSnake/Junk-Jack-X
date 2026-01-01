/*
	Junk Jack X: Core
	- [Serialization]Converter - Enum

	Written By: Ryan Smith
*/

using System;

namespace JJx.Core.Serialization;

internal sealed class GuidConverter : JJxConverter<Guid>
{
	/* Instance Methods */
	public override Guid Read(ref JJxReader reader)
	{
		Span<byte> buffer = stackalloc byte[GuidConverter.SIZE];
		reader.ReadSpan(buffer);
		return new Guid(buffer);
	}
	public override void Write(Guid @value, JJxWriter writer)
	{
		Span<byte> buffer = stackalloc byte[GuidConverter.SIZE];
		if (!@value.TryWriteBytes(buffer))
			throw new InvalidOperationException("Unable to write Guid to JJxWriter");
		writer.Write(buffer);
	}
	/* Class Properties */
	private const int SIZE = 16;
}
