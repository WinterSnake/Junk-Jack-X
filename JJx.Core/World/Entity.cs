/*
	Junk Jack X: Core
	- [World]Entity

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
	Creature = 0x00,
	Monster  = 0x02,
}

public sealed class Entity
{
	/* Constructors */
	public Entity(ushort x, ushort y, ushort id, EntityType type = EntityType.Creature)
	{
		this.Position = (x, y);
		this.Id = id;
	}
	public Entity((ushort, ushort) position, ushort id, EntityType type)
	{
		this.Position = position;
		this.Id = id;
		this.Type = type;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var buffer = new byte[SIZE];
		BitConverter.LittleEndian.Write(this.Position.X, buffer, OFFSET_POSITION);
		BitConverter.LittleEndian.Write(this.Position.Y, buffer, OFFSET_POSITION + sizeof(ushort));
		buffer[OFFSET_TYPE] = (byte)this.Type;
		BitConverter.LittleEndian.Write(this.Id, buffer, OFFSET_ID);
		await stream.WriteAsync(buffer, 0, buffer.Length);
	}
	/* Static Methods */
	public static async Task<Entity> FromStream(Stream stream)
	{
		var bytesRead = 0;
		var buffer = new byte[SIZE];
		while (bytesRead < buffer.Length)
			bytesRead += await stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
		var position = (
			BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_POSITION),
			BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_POSITION + sizeof(ushort))
		);
		var type = (EntityType)buffer[OFFSET_TYPE];
		var id = BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_ID);
		return new Entity(position, id, type);
	}
	/* Properties */
	public (ushort X, ushort Y) Position;
	public ushort Id;
	public EntityType Type;
	/* Class Properties */
	private const byte SIZE            = 7;
	private const byte OFFSET_POSITION = 0;
	private const byte OFFSET_TYPE     = 4;
	private const byte OFFSET_ID       = 5;
}
