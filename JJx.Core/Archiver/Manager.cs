/*
	Junk Jack X: Core
	- [Archiver]Manager

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
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
	public bool HasChunkType(ArchiverChunkType type);
	public Stream GetChunkStream(ArchiverChunkType type);
	internal JJxReader GetChunkReader(ArchiverChunkType type, out byte[] buffer);
	/* Properties */
	public ArchiveType Type { get; }
}

public interface IArchiveWriter : IDisposable
{
	/* Instance Methods */
}

public sealed class ArchiveManager : IArchiveReader, IArchiveWriter
{
	/* Constructors */
	private ArchiveManager(Stream stream, IEnumerable<ArchiverChunk> chunks)
	{
		this._Chunks = new(chunks);
		this._Stream = stream;
	}
	/* Instance Methods */
	public void Dispose() => this._Stream.Dispose();
	public bool HasChunkType(ArchiverChunkType type)
	{
		foreach (ref var chunk in CollectionsMarshal.AsSpan(this._Chunks))
			if (type == chunk.Type)
				return true;
		return false;
	}
	public Stream GetChunkStream(ArchiverChunkType type)
	{
		var chunkStream = this._GetChunkStream(type);
		return chunkStream;
	}
	JJxReader IArchiveReader.GetChunkReader(ArchiverChunkType type, out byte[] buffer)
	{
		var chunkStream = this._GetChunkStream(type);
		buffer = ArrayPool<byte>.Shared.Rent((int)chunkStream.Length);
		var slice = buffer.AsSpan(0, (int)chunkStream.Length);
		chunkStream.ReadExactly(slice);
		return new JJxReader(slice);
	}
	private ChunkReaderStream _GetChunkStream(ArchiverChunkType type)
	{
		foreach (ref var chunk in CollectionsMarshal.AsSpan(this._Chunks))
			if (type == chunk.Type) return new ChunkReaderStream(this._Stream, ref chunk);
		throw new InvalidDataException($"Tried to create a chunk stream reader over non-existent '{type}' chunk");
	}
	/* Static Methods */
	public static IArchiveReader Reader(string path)
	{
		var stream = File.Open(path, FileMode.Open);
		try {
			Span<byte> buffer = stackalloc byte[SIZEOF_HEADER];
			stream.ReadExactly(buffer);
			var reader = new JJxReader(buffer);
			var magic = reader.ReadString(length: 4);
			var type = reader.ReadObject<ArchiveType>();
			var chunkCount = reader.ReadUInt16();
			var chunks = new ArchiverChunk[chunkCount];
			var chunkSize = chunkCount * ArchiverChunk.SIZE;
			var chunkBuffer = ArrayPool<byte>.Shared.Rent(chunkSize);
			try {
				stream.ReadExactly(chunkBuffer.AsSpan(0, chunkSize));
				reader = new JJxReader(chunkBuffer);
				for (var i = 0; i < chunks.Length; ++i)
					chunks[i] = reader.ReadObject<ArchiverChunk>();
				var manager = new ArchiveManager(stream, chunks) { Type=type };
				return manager;
			} finally {
				ArrayPool<byte>.Shared.Return(chunkBuffer);
			}
		} catch {
			stream.Dispose();
			throw;
		}
	}
	//public static IArchiveWriter Writer()
	//{
	//	var manager = new ArchiveManager();
	//	return manager;
	//}
	/* Properties */
	public ArchiveType Type { get; init; }
	private readonly Stream _Stream;
	private readonly List<ArchiverChunk> _Chunks;
	/* Class Properties */
	private const int SIZEOF_HEADER = 0xC;
}
