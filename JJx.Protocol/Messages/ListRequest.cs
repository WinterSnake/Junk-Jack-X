/*
	Junk Jack X: Protocol
	- [Message: Requests]List

	Written By: Ryan Smith
*/

namespace JJx.Protocol;

public sealed class ListRequestMessage
{
	/* Constructor */
	public ListRequestMessage() { }
	/* Instance Methods */
	public byte[] Serialize()
	{
		var buffer = new byte[sizeof(ushort) * 2];
		BitConverter.BigEndian.Write((ushort)MessageHeader.ListRequest, buffer);
		BitConverter.LittleEndian.Write((ushort)0, buffer, sizeof(ushort));
		return buffer;
	}
}
