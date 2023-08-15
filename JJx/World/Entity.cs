/*
	Junk Jack X: World
	- Entity

	Segment Breakdown:
	------------------------------------------------------------------------------------------------------------------------
	Segment[0x0 : 0x1] = X Position      | Length: 2  (0x02) | Type: uint16
	Segment[0x2 : 0x3] = Y Position      | Length: 2  (0x02) | Type: uint16
	Segment[0x4]       = UNKNOWN FOR NOW | Length: 1  (0x01) | Type: ???
	Segment[0x5 : 0x6] = Id              | Length: 2  (0x02) | Type: uint16
	------------------------------------------------------------------------------------------------------------------------
	Length: 7 (0x7)

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Entity
{
	/* Contructors */
	public Entity(ushort id, (ushort x, ushort y) position)
	{
		this.Id = id;
		this.Position = position;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
		// TODO: Single WriteAsync | Use SIZE buffer
		// TODO: Ensure BitConverter.GetBytes<T> forces little endian
	{
		byte[] bytes;
		var workingData = new byte[SIZE];
		// Position
		// -X
		bytes = BitConverter.GetBytes(this.Position.X);
		Array.Copy(bytes, 0, workingData, 0, bytes.Length);
		// -Y
		bytes = BitConverter.GetBytes(this.Position.Y);
		Array.Copy(bytes, 0, workingData, 2, bytes.Length);
		await stream.WriteAsync(workingData, 0, SIZEOF_POSITION);
		//----Unknown----\\
		stream.WriteByte(0);
		// Id
		bytes = BitConverter.GetBytes(this.Id);
		Array.Copy(bytes, 0, workingData, 5, bytes.Length);
		await stream.WriteAsync(workingData, 0, SIZEOF_ID);
	}
	/* Static Methods */
	public static async Task<Entity> FromStream(Stream stream)
		// TODO: Ensure BitConverter.To<T> forces little endian
	{
		var bytesRead = 0;
		var workingData = new byte[SIZE];
		while (bytesRead < SIZE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZE - bytesRead);
		// Position
		var position = (
			BitConverter.ToUInt16(new Span<byte>(workingData).Slice(0, 2)),
			BitConverter.ToUInt16(new Span<byte>(workingData).Slice(2, 2))
		);
		//----Unknown----\\
		//workingData[4];
		// Id
		var id = BitConverter.ToUInt16(new Span<byte>(workingData).Slice(5, 2));
		return new Entity(id, position);
	}
	/* Properties */
	public ushort Id;
	public (ushort X, ushort Y) Position;
	/* Class Properties */
	private const byte SIZE            = 7;
	private const byte SIZEOF_ID       = 2;
	private const byte SIZEOF_POSITION = 4;
}
