/*
	Junk Jack X: Player

	Segment Breakdown:
	-----------------------------------------------------------
	:<Effect>
	Segment[0x0 : 0x1] = Id    | Length: 2 (0x2) | Type: uint16
	Segment[0x2 : 0x3] = Ticks | Length: 2 (0x2) | Type: uint16
	-----------------------------------------------------------

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Effect
{
	/* Constructors */
	public Effect(ushort id, ushort ticks)
	{
		this.Id = id;
		this.Ticks = ticks;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var workingData = new byte[SIZE];
		BitConverter.Write(workingData, this.Id,    0);
		BitConverter.Write(workingData, this.Ticks, 2);
		await stream.WriteAsync(workingData, 0, workingData.Length);
	}
	/* Static Methods */
	public static async Task<Effect> FromStream(Stream stream)
	{
		int bytesRead = 0;
		var workingData = new byte[SIZE];
		while (bytesRead < SIZE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZE - bytesRead);
		var id    = BitConverter.GetUInt16(workingData, 0);
		var ticks = BitConverter.GetUInt16(workingData, 2);
		return new Effect(id, ticks);
	}
	/* Properties */
	public ushort Id;
	public ushort Ticks;
	/* Class Properties */
	private const byte SIZE = 4;
}
