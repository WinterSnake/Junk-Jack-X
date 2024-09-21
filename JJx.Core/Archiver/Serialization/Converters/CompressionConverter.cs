/*
	Junk Jack X: Core
	- [Serializer.Converters]Compression

	Written By: Ryan Smith
*/
using System;

namespace JJx.Serialization;

internal sealed class CompressionConverter<T> : JJxConverter<T>
{
	/* Constructor */
	public CompressionConverter(JJxConverter<T> converter) { this._InternalConverter = converter; }
	/* Instance Methods */
	public override T Read(JJxReader reader)
	{
		throw new NotImplementedException($"CompressionConverter does not implement a Read");
	}
	public override void Write(JJxWriter writer, object @value) => throw new NotImplementedException($"CompressionConverter does not implement a Write");
	/* Properties */
	private readonly JJxConverter<T> _InternalConverter;
}
