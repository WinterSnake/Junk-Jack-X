/*
	Junk Jack X: Core
	- [Serialization]Converter - DateTime

	Written By: Ryan Smith
*/

using System;

namespace JJx.Core.Serialization;

internal sealed class DateTimeConverter : JJxConverter<DateTime>
{
    /* Instance Methods */
    public override DateTime Read(ref JJxReader reader, JJxSerializationOptions options)
		=> DateTimeOffset.FromUnixTimeSeconds(reader.ReadInt32()).LocalDateTime;
    public override void Write(in DateTime @value, JJxWriter writer, JJxSerializationOptions options)
		=> writer.Write(new DateTimeOffset(@value.ToUniversalTime()).ToUnixTimeSeconds());
}
