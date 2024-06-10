/*
	Junk Jack X: Core
	- Effect

	Segment Breakdown:
	-----------------------------------------------------------
	Segment[0x0 : 0x1] = Id    | Length: 2 (0x2) | Type: uint16
	Segment[0x2 : 0x3] = Ticks | Length: 2 (0x2) | Type: uint16
	-----------------------------------------------------------

	Written By: Ryan Smith
*/
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Effect
{
	/* Constructor */
	public Effect(ushort id, ushort ticks)
	{
		this.Id = id;
		this.Ticks = ticks;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var buffer = new byte[SIZE];
		BitConverter.LittleEndian.Write(this.Id, buffer, OFFSET_ID);
		BitConverter.LittleEndian.Write(this.Ticks, buffer, OFFSET_TICKS);
		await stream.WriteAsync(buffer, 0, buffer.Length);
	}
	/* Static Methods */
	public static async Task<Effect> FromStream(Stream stream)
	{
		var bytesRead = 0;
		var buffer = new byte[SIZE];
		while (bytesRead < buffer.Length)
			bytesRead += await stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
		var id    = BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_ID);
		var ticks = BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_TICKS);
		return new Effect(id, ticks);
	}
	/* Properties */
	public ushort Id;
	public ushort Ticks;
	/* Class Properties */
	private const byte SIZE         = 4;
	private const byte OFFSET_ID    = 0;
	private const byte OFFSET_TICKS = 2;
}
