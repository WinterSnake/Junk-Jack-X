/*
	Junk Jack X: World
	- Mob

	Segment Breakdown:
	------------------------------------------------------------------------
	Segment[0x0 : 0x1] = X Position | Length: 2 (0x2) | Type: uint16
	Segment[0x2 : 0x3] = Y Position | Length: 2 (0x2) | Type: uint16
	Segment[0x4]       = Type       | Length: 1 (0x1) | Type: enum[uint8]
	Segment[0x5 : 0x6] = Id         | Length: 2 (0x2) | Type: uint16
	------------------------------------------------------------------------
	Size: 7 (0x7)

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Mob
{
	/* Constructors */
	public Mob(ushort id, (ushort, ushort) position, Type spec = Type.Creature)
	{
		this.Id = id;
		this.Position = position;
		this.Spec = spec;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var workingData = new byte[SIZE];
		// Position
		BitConverter.Write(workingData, this.Position.X, 0);
		BitConverter.Write(workingData, this.Position.Y, 2);
		// Spec
		BitConverter.Write(workingData, (byte)this.Spec, 4);
		// Id
		BitConverter.Write(workingData, this.Id, 5);
		await stream.WriteAsync(workingData, 0, workingData.Length);
	}
	/* Static Methods */
	public async Task<Mob> FromStream(Stream stream)
	{
		int bytesRead = 0;
		var workingData = new byte[SIZE];
		while (bytesRead < workingData.Length)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, workingData.Length - bytesRead);
		// Position
		var position = (
			BitConverter.GetUInt16(workingData, 0),
			BitConverter.GetUInt16(workingData, 2)
		);
		// Spec
		var spec = (Type)workingData[4];
		// Id
		var id = BitConverter.GetUInt16(workingData, 5);
		return new Mob(id, position, spec);
	}
	/* Properties */
	public ushort Id;
	public Type Spec;
	public (ushort X, ushort Y) Position;
	/* Class Properties */
	private const byte SIZE = 7;
	/* Sub-Classes */
	public enum Type
	{
		Creature,
		Monster,
		Pet,
		Special,
	}
}
