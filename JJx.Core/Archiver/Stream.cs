/*
	Junk Jack X: Core
	- Archiver

	Segment Breakdown:
	----------------------------------------------------------------------------------------------------
	Segment[0x0 : 0x3] = Magic       | Length: 4 (0x4) | Type: char[4]
	Segment[0x4 : 0x5] = File Type   | Length: 2 (0x2) | Type: enum[uint16] | Parent: ArchiverStreamType
	Segment[0x6 : 0x7] = Chunk Count | Length: 2 (0x2) | Type: uint16
	Segment[0x8 : 0xB] = UNKNOWN     | Length: 4 (0x4) | Type: ???
	----------------------------------------------------------------------------------------------------
	Size: 12 (0xC)

	Written By: Ryan Smith
*/

using System;
using System.Collections.Generic;
using System.IO;
using JJx.Core.Serialization;

namespace JJx.Core;

public interface IArchiverReader
{
	/* Instance Methods */
}

public interface IArchiverWriter
{
	/* Instance Methods */
}

public enum ArchiverType : ushort
{
	Player    = 0x00,
	World     = 0x01,
	Adventure = 0x02,
}

public sealed class ArchiverStream : IDisposable, IArchiverReader, IArchiverWriter
{
	/* Constructor */
	private ArchiverStream(ArchiverType type, Stream stream)
		:this(type, stream, new()) { }
	private ArchiverStream(ArchiverType type, Stream stream, List<ArchiverChunk> chunks)
	{
		this.Type = type;
		this._Stream = stream;
		this._Chunks = chunks;
	}
	/* Instance Methods */
	public void Dispose() => this._Stream.Dispose();
	/* Static Methods */
	public static IArchiverReader Reader(string file) => ArchiverStream.Reader(File.Open(file, FileMode.Open));
	public static IArchiverReader Reader(Stream stream)
	{
		// Header
		var reader = new JJxReader(stream);
		var magic = reader.ReadString(4);
		var type = reader.ReadObject<ArchiverType>();
		// Header: Validation
		var expected = type switch {
			ArchiverType.Player => MAGIC_PLAYER,
			ArchiverType.Adventure or ArchiverType.World => MAGIC_WORLD,
			_ => throw new InvalidOperationException($"Non-expected archiver type {type} in reader creation"),
		};
		if (magic != expected)
			throw new InvalidDataException($"Tried loading {type} file with invalid format");
		// Chunks
		var chunkCount = reader.ReadUInt16();
		var chunks = new List<ArchiverChunk>(chunkCount);
		reader.Skip(sizeof(uint));
		for (var i = 0; i < chunks.Capacity; ++i)
			chunks.Add(reader.ReadObject<ArchiverChunk>());
		return new ArchiverStream(type, stream, chunks);
	}
	/* Properties */
	// Common
	public readonly ArchiverType Type;
	private readonly Stream _Stream;
	private readonly List<ArchiverChunk> _Chunks;
	// Reading
	// Writing
	/* Class Properties */
	private const string MAGIC_PLAYER = "JJXC";
	private const string MAGIC_WORLD  = "JJXM";
}
