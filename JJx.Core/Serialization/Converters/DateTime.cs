/*
	Junk Jack X: Core
	- [Serialization]Converter - Enum

	Written By: Ryan Smith
*/

using System;

namespace JJx.Core.Serialization;

internal sealed class DateTimeConverter : JJxConverter<DateTime>
{
	/* Instance Methods */
	public override DateTime Read(ref JJxReader reader)
		=> DateTimeOffset.FromUnixTimeSeconds(reader.ReadUInt32()).LocalDateTime;
	public override void Write(DateTime value, JJxWriter writer)
		=> writer.Write((uint)new DateTimeOffset(value.ToUniversalTime()).ToUnixTimeSeconds());
}
