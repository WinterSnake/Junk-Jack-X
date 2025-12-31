/*
	Junk Jack X: Core
	- [Archiver]Stream

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
using System.Runtime.InteropServices;
using JJx.Core.Serialization;

namespace JJx.Core;

public enum ArchiveType : ushort
{
	Player    = 0x00,
	World     = 0x01,
	Adventure = 0x02,
}

public interface IArchiveReader : IDisposable
{
	/* Instance Methods */
	public ChunkStream GetChunkStream(ArchiverChunkType type);
	/* Properties */
	public ArchiveType Type { get; }
	internal IEnumerable<ArchiverChunk> Chunks { get; }
	internal int ChunkCount { get; }
}

public interface IArchiveWriter : IDisposable
{
	/* Instance Methods */
}

public sealed class ArchiveStream : IDisposable, IArchiveReader, IArchiveWriter
{
	/* Constructor */
	private ArchiveStream(ArchiveType type, Stream stream)
		:this(type, stream, new()) { }
	private ArchiveStream(ArchiveType type, Stream stream, List<ArchiverChunk> chunks)
	{
		this.Type = type;
		this._Stream = stream;
		this._Chunks = chunks;
	}
	/* Instance Methods */
	public void Dispose() => this._Stream.Dispose();
	// Reading
	public ChunkStream GetChunkStream(ArchiverChunkType type)
	{
		foreach (ref var chunk in CollectionsMarshal.AsSpan(this._Chunks))
		{
			if (chunk.Type != type) continue;
			this._Stream.Position = chunk.Offset;
			return new ChunkStream(this._Stream, ref chunk);
		}
		throw new InvalidDataException($"Tried to create a chunk stream reader over non-existent '{type}' chunk");
	}
	// Writing
	/* Static Methods */
	public static IArchiveReader Reader(Stream stream)
	{
		if (!stream.CanRead)
			throw new InvalidOperationException("Tried create an archive reader from non-readable stream");
		// Header
		var reader = new JJxReader(stream);
		var magic = reader.ReadString(4);
		var type = reader.ReadObject<ArchiveType>();
		// Header: Validation
		var expected = type switch {
			ArchiveType.Player => MAGIC_PLAYER,
			ArchiveType.Adventure or ArchiveType.World => MAGIC_WORLD,
			_ => throw new InvalidOperationException($"Non-expected archive type {type} in reader creation"),
		};
		if (magic != expected)
			throw new InvalidDataException($"Tried loading {type} file with invalid format");
		// Chunks
		var chunkCount = reader.ReadUInt16();
		var chunks = new List<ArchiverChunk>(chunkCount);
		reader.Skip(sizeof(uint));
		for (var i = 0; i < chunks.Capacity; ++i)
			chunks.Add(reader.ReadObject<ArchiverChunk>());
		return new ArchiveStream(type, stream, chunks);
	}
	/* Properties */
	// Common
	public readonly ArchiveType Type;
	private readonly Stream _Stream;
	private readonly List<ArchiverChunk> _Chunks;
	// Reading
    ArchiveType IArchiveReader.Type => this.Type;
    IEnumerable<ArchiverChunk> IArchiveReader.Chunks => this._Chunks;
    int IArchiveReader.ChunkCount => this._Chunks.Count;
	// Writing
	/* Class Properties */
	private const string MAGIC_PLAYER = "JJXC";
	private const string MAGIC_WORLD  = "JJXM";
}
