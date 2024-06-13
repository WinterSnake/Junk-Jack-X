/*
	Junk Jack X: Protocol
	- [Message: Responses]List

	Written By: Ryan Smith
*/
using System;

namespace JJx.Protocol;

public sealed class ListResponseMessage
{
	/* Constructor */
	public ListResponseMessage(byte playerId, bool isConnectedPlayer, string name)
	{
		this.Id = playerId;
		this.IsConnectedPlayer = isConnectedPlayer;
		this.Name = name;
	}
	/* Instance Methods */
	public byte[] Serialize()
	{
		var buffer = new byte[SIZE + sizeof(ushort)];
		BitConverter.BigEndian.Write((ushort)MessageHeader.ListResponse, buffer);
		buffer[OFFSET_ID + sizeof(ushort)] = this.Id;
		BitConverter.Write(this.IsConnectedPlayer, buffer, OFFSET_CONNECTEDFLAG + sizeof(ushort));
		BitConverter.Write(this.Name, buffer, OFFSET_NAME + sizeof(ushort), length: SIZEOF_NAME);
		return buffer;
	}
	/* Static Methods */
	public static ListResponseMessage Deserialize(ReadOnlySpan<byte> buffer)
	{
		var id = buffer[OFFSET_ID];
		var isConnectedPlayer = BitConverter.GetBool(buffer, OFFSET_CONNECTEDFLAG);
		var name = BitConverter.GetString(buffer, OFFSET_NAME);
		return new(id, isConnectedPlayer, name);
	}
	/* Properties */
	public readonly byte Id;
	public readonly bool IsConnectedPlayer;
	public readonly string Name;
	/* Class Properties */
	private const byte SIZE                 = 34;
	private const byte OFFSET_ID            =  0;
	private const byte OFFSET_CONNECTEDFLAG =  1;
	private const byte OFFSET_NAME          =  2;
	private const byte SIZEOF_NAME          = 32;
}
