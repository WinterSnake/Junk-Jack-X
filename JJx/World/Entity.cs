/*
	Junk Jack X: World
	- Entity

	Segment Breakdown:
	---------------------------------------------------------------------
	Segment[0x0 : 0x1] = X Position | Length: 2 (0x2) | Type: uint16
	Segment[0x2 : 0x3] = Y Position | Length: 2 (0x2) | Type: uint16
	Segment[0x4]       = Type       | Length: 1 (0x1) | Type: enum[uint8]
	Segment[0x5 : 0x6] = Id         | Length: 2 (0x2) | Type: uint16
	---------------------------------------------------------------------
	Size: 7 (0x7)

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public enum EntityType : byte
{
	Creature,
	Monster,
	Pet,
	Special,
}

public sealed class Entity
{
	/* Constructors */
	public Entity(ushort id, (ushort, ushort) position, EntityType type = EntityType.Creature)
	{
		this.Id = id;
		this.Position = position;
		this.Type = type;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var workingData = new byte[SIZE];
		// Position
		Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Position.X, 0);
		Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Position.Y, 2);
		// Type
		Utilities.ByteConverter.Write(new Span<byte>(workingData), (byte)this.Type, 4);
		// Id
		Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Id,         5);
		await stream.WriteAsync(workingData, 0, workingData.Length);
	}
	/* Static Methods */
	public static async Task<Entity> FromStream(Stream stream)
	{
		var bytesRead = 0;
		var workingData = new byte[SIZE];
		while (bytesRead < SIZE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZE - bytesRead);
		// Position
		var position = (
			Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData), 0),
			Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData), 2)
		);
		// Type
		var type = (EntityType)Utilities.ByteConverter.GetUInt8(new Span<byte>(workingData), 4);
		// Id
		var id = Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData), 5);
		return new Entity(id, position, type);
	}
	/* Properties */
	public ushort Id;
	public (ushort X, ushort Y) Position;
	public EntityType Type;
	/* Class Properties */
	private const byte SIZE = 7;
}
