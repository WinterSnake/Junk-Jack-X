/*
	Junk Jack X: Protocol
	- [Message: Requests]World

	Written By: Ryan Smith
*/
using System;

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
	public static WorldRequestMessage Deserialize(ReadOnlySpan<byte> buffer)
	{
		#if DEBUG
			Console.WriteLine($"WorldRequest.Data={BitConverter.ToString(buffer)}");
		#endif
		return new WorldRequestMessage();
	}
	/* Class Properties */
	private const byte SIZE = sizeof(ushort);
}

public sealed class WorldProgressMessage
{
	/* Constructor */
	public WorldProgressMessage(float percent) { this.Percent = percent; }
	/* Instance Methods */
	public byte[] Serialize()
	{
		var buffer = new byte[SIZE + sizeof(ushort)];
		BitConverter.BigEndian.Write((ushort)MessageHeader.WorldProgress, buffer);
		BitConverter.LittleEndian.Write((ushort)(this.Percent * 100.0f), buffer, sizeof(ushort));
		return buffer;
	}
	/* Static Methods */
	public static WorldProgressMessage Deserialize(ReadOnlySpan<byte> buffer)
	{
		var percent = (float)BitConverter.LittleEndian.GetUInt16(buffer) / 100.0f;
		return new WorldProgressMessage(percent);
	}
	/* Properties */
	public readonly float Percent;
	/* Class Properties */
	private const byte SIZE = sizeof(ushort);
}
