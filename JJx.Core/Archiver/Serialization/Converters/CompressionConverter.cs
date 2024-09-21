/*
	Junk Jack X: Core
	- [Serializer.Converters]Compression

	Written By: Ryan Smith
*/
using System;
using System.IO.Compression;

namespace JJx.Serialization;

internal sealed class CompressionConverter<T> : JJxConverter<T>
{
	/* Constructor */
	public CompressionConverter(JJxConverter<T> converter) { this._InternalConverter = converter; }
	/* Instance Methods */
	public override T Read(JJxReader reader)
	{
		using var compressionStream = new GZipStream(reader.BaseStream, CompressionMode.Decompress, true);
		var compressedReader = new JJxReader(compressionStream);
		return this._InternalConverter.Read(compressedReader);
	}
	/* Properties */
	private readonly JJxConverter<T> _InternalConverter;
}
