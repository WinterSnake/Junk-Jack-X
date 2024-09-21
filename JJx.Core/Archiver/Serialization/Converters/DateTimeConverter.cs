/*
	Junk Jack X: Core
	- [Serializer.Converters]DateTime

	Written By: Ryan Smith
*/
using System;

namespace JJx.Serialization;

internal sealed class DateTimeConverter : JJxConverter<DateTime>
{
	/* Instance Methods */
	public override DateTime Read(JJxReader reader)
		=> DateTimeOffset.FromUnixTimeSeconds(reader.GetUInt32()).LocalDateTime;
}
