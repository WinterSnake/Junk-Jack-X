/*
	Junk Jack X: Archiver
	- Stream

	Segment Breakdown:
	------------------------------------------------------------------------------------------------------------------------
	:<Archiver Header>
	Segment[0x0 : 0x3] = Magic       | Length: 2 (0x2) | Type: char[4]
	Segment[0x4 : 0x5] = File Type   | Length: 2 (0x2) | Type: enum[uint16]
	Segment[0x6 : 0x7] = Chunk Count | Length: 2 (0x2) | Type: uint16
	Segment[0x8 : 0xC] = UNKNOWN     | Length: 4 (0x4) | Type: ???
	------------------------------------------------------------------------------------------------------------------------
	Size: 12 (0xC)

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;
using JJx.Utilities;

namespace JJx;

internal enum ArchiverType : byte
{
	Unknown,
	Player,
	Adventure,
	Map,
	Stat
}

#nullable enable
internal sealed class ArchiverStream : FileStream
{
	/* Constructors */
	private ArchiverStream(string path, FileMode mode, FileAccess access): base(path, mode, access) {}
	/* Instance Methods */
	// Reading
	public bool AtChunk(Chunk.Type type)
	{
		if (!this.CanRead || this._Chunks == null)
			return false;
		foreach (var chunk in this._Chunks)
			if (chunk.Id == type && this.Position == chunk.Location)
				return true;
		return false;
	}
	public bool HasChunk(Chunk.Type type)
	{
		if (!this.CanRead || this._Chunks == null)
			return false;
		foreach (var chunk in this._Chunks)
			if (chunk.Id == type)
				return true;
		return false;
	}
	public void SeekChunk(Chunk.Type type)
	{
		if (!this.CanRead || this._Chunks == null)
			return;
		foreach (var chunk in this._Chunks)
			if (chunk.Id == type)
				this.Position = chunk.Location;
	}
	// Writing
	/* Static Methods */
	public static async Task<ArchiverStream> Reader(string path)
	{
		var reader = new ArchiverStream(path, FileMode.Open, FileAccess.Read);
		Console.WriteLine($"Read: {reader.CanRead} | Write: {reader.CanWrite}");
		int bytesRead = 0;
		var workingData = new byte[SIZEOF_HEADER];
		while (bytesRead < SIZEOF_HEADER)
			bytesRead += await reader.ReadAsync(workingData, bytesRead, SIZEOF_HEADER - bytesRead);
		/// Header
		var headerMagic = ByteConverter.GetString(new Span<byte>(workingData), 0, 4);
		var headerId    = ByteConverter.GetUInt16(new Span<byte>(workingData), 4);
		var chunkCount  = ByteConverter.GetUInt16(new Span<byte>(workingData), 6);
		// Type
		if (headerMagic == "JJXC" && headerId == 0)
			reader.Type = ArchiverType.Player;
		else if (headerMagic == "JJXM")
		{
			if (headerId == 1)
				reader.Type = ArchiverType.Map;
			else if (headerId == 2)
				reader.Type = ArchiverType.Adventure;
		}
		// =UNKNOWN=
		reader.Seek(4, SeekOrigin.Current);
		// Chunk
		var chunks = new Chunk[chunkCount];
		for (var i = 0; i < chunkCount; ++i)
			chunks[i] = await Chunk.FromStream(reader);
		reader._Chunks = chunks;
		return reader;
	}
	public static async Task<ArchiverStream> Writer(string path, ArchiverType type)
	{
		var writer = new ArchiverStream(path, FileMode.Create, FileAccess.Write);
		Console.WriteLine($"Read: {writer.CanRead} | Write: {writer.CanWrite}");
		var workingData = new byte[SIZEOF_HEADER - 2];
		switch (type)
		{
			case ArchiverType.Player:
			{
				Utilities.ByteConverter.Write(new Span<byte>(workingData), "JJXC", length: 4);
				Utilities.ByteConverter.Write(new Span<byte>(workingData), (ushort)0, 4);
			} break;
			case ArchiverType.Map:
			{
				Utilities.ByteConverter.Write(new Span<byte>(workingData), (ushort)1, 4);
				goto World;
			}
			case ArchiverType.Adventure:
			{
				Utilities.ByteConverter.Write(new Span<byte>(workingData), (ushort)2, 4);
				goto World;
			}
			default:
			{
				throw new ArgumentException($"Unsupported writer format '{type}'");
			}
			World:
			{
				Utilities.ByteConverter.Write(new Span<byte>(workingData), "JJXM", length: 4);
			} break;
		}
		await writer.WriteAsync(workingData, 0, workingData.Length);
		return writer;
	}
	/* Properties */
	public ArchiverType Type { get; private set; } = ArchiverType.Unknown;
	private Chunk[]? _Chunks = null;
	private readonly MemoryStream? _InternalBuffer = null;
	/* Class Properties */
	private const byte SIZEOF_HEADER = 8;
}
#nullable disable
