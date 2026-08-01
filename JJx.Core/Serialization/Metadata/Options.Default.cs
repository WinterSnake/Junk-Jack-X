/*
	Junk Jack X: Protocol
	- [Packet::Registry]Server

	Written By: Ryan Smith
*/

using System;
using JJx.Core.Serialization;

namespace JJx.Core.Metadata;

internal static class JJxDefaultSerializationOptions
{
	/* Constructor */
	static JJxDefaultSerializationOptions()
	{
		Options = new();
		Options.AddConverter<EnumConverterFactory>();
		Options.AddConverter<ArchiverChunkConverter>(typeof(ArchiverChunk));
		Options.AddConverter<GuidConverter>(typeof(Guid));
		Options.AddConverter<DateTimeConverter>(typeof(DateTime));
		Options.AddConverter<TileConverter>(typeof(Tile));
	}
	/* Class Properties */
	internal static readonly JJxSerializationOptions Options;
}
