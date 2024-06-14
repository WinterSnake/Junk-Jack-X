/*
	Junk Jack X: Protocol
	- [Messages]World

	Written By: Ryan Smith
*/
using System;

namespace JJx.Protocol;

public sealed class WorldTimeMessage
{
	/* Constructors */
	public WorldTimeMessage(Period period, ushort ticks)
	{
		this.Time = period;
		this.Ticks = ticks;
	}
	public WorldTimeMessage(Period period, uint ticks): this(period, (ushort)(ticks & 0x0000FFFF)) { }
	/* Instance Methods */
	public byte[] Serialize()
	{
		var buffer = new byte[SIZE + sizeof(ushort)];
		BitConverter.BigEndian.Write((ushort)MessageHeader.WorldTime, buffer);
		buffer[OFFSET_PERIOD + sizeof(ushort)] = (byte)this.Time;
		BitConverter.LittleEndian.Write(this.Ticks, buffer, OFFSET_TICKS + sizeof(ushort));
		Console.WriteLine($"WorldTime={BitConverter.ToString(buffer)}");
		return buffer;
	}
	/* Static Methods */
	public static WorldTimeMessage Deserialize(ReadOnlySpan<byte> buffer)
	{
		Console.WriteLine($"WorldTime={BitConverter.ToString(buffer)}");
		var period = (Period)buffer[OFFSET_PERIOD];
		var ticks = BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_TICKS);
		return new WorldTimeMessage(period, ticks);
	}
	/* Properties */
	public readonly Period Time;
	public readonly ushort Ticks;
	/* Class Properties */
	private const byte SIZE          = 3;
	private const byte OFFSET_PERIOD = 0;
	private const byte OFFSET_TICKS  = 1;
}
