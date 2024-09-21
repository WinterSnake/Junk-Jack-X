/*
	Junk Jack X: Core
	- [Serializer.Converters]GUID

	Written By: Ryan Smith
*/
using System;

namespace JJx.Serialization;

internal sealed class GuidConverter : JJxConverter<Guid>
{
	/* Instance Methods */
	public override Guid Read(JJxReader reader) => new Guid(reader.GetBytes(16));
}
