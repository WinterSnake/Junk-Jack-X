/*
	Junk Jack X: Archiver
	- Chunk

	Segment Breakdown:
	------------------------------------------------------------------------------------------------------------------------
	:<Chunk>
	Segment[0x0 : 0x1] = Type       | Length: 2 (0x2) | Type: enum[uint16]
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

internal enum ChunkType : ushort
{
	Padding            = 0x0000,  // Used in Creative/Flat maps to fill 19th chunk
	// Player
	PlayerInfo         = 0x8000,
	PlayerInventory    = 0x8001,
	PlayerCraftbook    = 0x8002,
	PlayerAchievements = 0x8003,
	PlayerStatus       = 0x8004,
	// World
	WorldInfo          = 0x0001,
	WorldBlocks        = 0x0002,
	WorldUnknown00     = 0x0003,
	WorldBorders       = 0x0004,
	WorldChests        = 0x0005,
	WorldForges        = 0x0006,
	WorldStables       = 0x0007,
	WorldLabs          = 0x0008,
	WorldSigns         = 0x0009,
	WorldShelves       = 0x000A,
	WorldPlants        = 0x000B,
	WorldUnknown01     = 0x000C,
	WorldUnknown02     = 0x000D,
	WorldLocks         = 0x000E,
	WorldFluid         = 0x000F,
	WorldUnknown04     = 0x0010,
	WorldWeather       = 0x0011,
	WorldTime          = 0x0012,
	WorldEntities      = 0x0013,
}

internal sealed class Chunk
{
	/* Constructors */
	public Chunk(ChunkType type, byte version, bool compressed, uint location, uint size)
	{
		this.Type = type;
		this.Version = version;
		this.Compressed = compressed;
		this.Location = location;
		this.Size = size;
	}
	/* Instance Methods */
	public override string ToString()
	{
		return $"Type:{this.Type} | Version:{this.Version} | Compressed:{this.Compressed} | Location:0x{this.Location:X4} | Size:0x{this.Size:X4}";
	}
	public void ToStream(Stream stream, uint origin)
	{
		var workingData = new byte[SIZE];
		Utilities.ByteConverter.Write(new Span<byte>(workingData), (ushort)this.Type,      0);
		Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Version,           2);
		Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Compressed,        3);
		Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Location + origin, 4);
		Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Size,              8);
		stream.Write(workingData, 0, workingData.Length);
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
		if (!Enum.IsDefined(typeof(ChunkType), id))
			throw new ArgumentException(
				$"Unknown chunk type: [Id: 0x{id:X4} | Version:{version} | Compressed:{compressed} | Location:0x{location:X4} | Size:0x{size:X4}]"
			);
		return new Chunk((ChunkType)id, version, compressed, location, size);
	}
	/* Properties */
	public readonly ChunkType Type;
	public readonly byte Version;
	public readonly bool Compressed;
	public readonly uint Location;
	public readonly uint Size;
	/* Class Properties */
	internal const byte SIZE = 12;
}

internal sealed class WritableChunk : IDisposable
{
	/* Constructors */
	public WritableChunk(ChunkType type, byte version, bool compressed, MemoryStream stream, Action<Chunk> func)
	{
		this.Type = type;
		this.Version = version;
		this.Compressed = compressed;
		this.Location = (uint)stream.Position;
		this._InternalStream = stream;
		this._AddChunk = func;
	}
	/* Instance Methods */
	public void Dispose()
	{
		var size = (uint)(this._InternalStream.Position - this.Location);
		var chunk = new Chunk(this.Type, this.Version, this.Compressed, this.Location, size);
		this._AddChunk(chunk);
	}
	public Task WriteAsync(byte[] bytes, int offset, int count) => this._InternalStream.WriteAsync(bytes, offset, count);
	/* Static Methods */
    public static implicit operator Stream(WritableChunk chunk) => chunk._InternalStream;
	/* Properties */
	private readonly ChunkType Type;
	private readonly byte Version;
	private readonly bool Compressed;
	private readonly uint Location;
	private readonly MemoryStream _InternalStream;
	private readonly Action<Chunk> _AddChunk;
}
