/*
	Junk Jack X: Protocol
	- [Message: Requests]World

	Written By: Ryan Smith
*/

namespace JJx.Protocol;

public sealed class WorldRequestMessage
{
	/* Constructor */
	public WorldRequestMessage() { }
	/* Instance Methods */
	public byte[] Serialize()
	{
		var buffer = new byte[SIZE + sizeof(ushort)];
		BitConverter.BigEndian.Write((ushort)MessageHeader.WorldRequest, buffer);
		BitConverter.LittleEndian.Write((ushort)0, buffer, sizeof(ushort));
		return buffer;
	}
	/* Class Properties */
	private const byte SIZE = sizeof(ushort);
}
