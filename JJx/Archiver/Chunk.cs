/*
	Junk Jack X: Archiver
	- Chunk

	Segment Breakdown:
	------------------------------------------------------------------------------------------------------------------------
	:<Chunk>
	Segment[0x0 : 0x1] = Id         | Length: 2 (0x2) | Type: enum[uint16]
	Segment[      0x2] = Version    | Length: 1 (0x1) | Type: uint8
	Segment[      0x3] = Compressed | Length: 1 (0x1) | Type: bool
	Segment[0x4 : 0x7] = Location   | Length: 4 (0x4) | Type: uint32
	Segment[0x8 : 0xC] = Size       | Length: 4 (0x4) | Type: uint32
	------------------------------------------------------------------------------------------------------------------------
	Size: 12 (0xC)

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;
using JJx.Utilities;

namespace JJx;

internal sealed class Chunk
{
	/* Constructors */
	public Chunk(Type id, byte version, bool compressed, uint location, uint size)
	{
		this.Id = id;
		this.Version = version;
		this.Compressed = compressed;
		this.Location = location;
		this.Size = size;
	}
	/* Instance Methods */
	public override string ToString()
	{
		return $"Type:{this.Id}|Version:{this.Version}|Compressed:{this.Compressed}|Location:0x{this.Location:X4}|Size:0x{this.Size:X4}";
	}
	/* Static Methods */
	public static async Task<Chunk> FromStream(Stream stream)
	{
		int bytesRead = 0;
		var workingData = new byte[SIZE];
		while (bytesRead < SIZE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZE - bytesRead);
		var id         = ByteConverter.GetUInt16(new Span<byte>(workingData), 0);
		var version    = ByteConverter.GetUInt8(new Span<byte>(workingData),  2);
		var compressed = ByteConverter.GetBool(new Span<byte>(workingData),   3);
		var location   = ByteConverter.GetUInt32(new Span<byte>(workingData), 4);
		var size       = ByteConverter.GetUInt32(new Span<byte>(workingData), 8);
		if (!Enum.IsDefined(typeof(Type), id))
			throw new ArgumentException(
				$"Unknown chunk type '0x{id:X4}|Version:{version}|Compressed:{compressed}|Location:0x{location:X4}|Size:0x{size:X4}'"
			);
		return new Chunk((Type)id, version, compressed, location, size);
	}
	/* Properties */
	public readonly Type Id;
	public readonly byte Version;
	public readonly bool Compressed;
	public readonly uint Location;
	public readonly uint Size;
	/* Class Properties */
	internal const byte SIZE = 12;
	/* Sub-Classes */
	internal enum Type : ushort
	{
		PlayerInfo         = 0x8000,
		PlayerInventory    = 0x8001,
		PlayerCraftbook    = 0x8002,
		PlayerAchievements = 0x8003,
		PlayerStatus       = 0x8004,
	}
}
