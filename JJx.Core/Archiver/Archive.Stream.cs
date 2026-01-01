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
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;
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
	public Stream GetChunkStream(ArchiverChunkType type);
	/* Properties */
	public ArchiveType Type { get; }
	internal IEnumerable<ArchiverChunk> Chunks { get; }
}

public interface IArchiveWriter : IDisposable
{
	/* Instance Methods */
	public void Flush();
	public Stream WriteChunk(ArchiverChunkType type, byte version = 1, bool IsCompressed = false);
}

public sealed class ArchiveStream : IDisposable, IArchiveReader, IArchiveWriter
{
	/* Constructor */
	private ArchiveStream(ArchiveType type, Stream stream)
		:this(type, stream, new()) => this._PendingChunks = new();
	private ArchiveStream(ArchiveType type, Stream stream, List<ArchiverChunk> chunks)
	{
		this.Type = type;
		this._Stream = stream;
		this._Chunks = chunks;
	}
	/* Instance Methods */
	public void Dispose()
	{
		this._Stream.Dispose();
		if (this._PendingChunks is null) return;
		foreach (var chunk in this._PendingChunks)
			chunk.Dispose();
	}
	// Reading
	public Stream GetChunkStream(ArchiverChunkType type)
	{
		foreach (ref var chunk in CollectionsMarshal.AsSpan(this._Chunks))
		{
			if (chunk.Type != type) continue;
			this._Stream.Seek(chunk.Offset, SeekOrigin.Begin);
			Stream substream = new ChunkStream(this._Stream, ref chunk);
			if (chunk.IsCompressed)
				substream = new GZipStream(substream, CompressionMode.Decompress, true);
			return substream;
		}
		throw new InvalidDataException($"Tried to create a chunk stream reader over non-existent '{type}' chunk");
	}
	// Writing
	public void Flush()
	{
		var writer = new JJxWriter(this._Stream);
		writer.Write((ushort)this._Chunks.Count);
		writer.Skip(4); // Unknown
		// Table
		int totalOffset = 0;
		var headerOffset = SIZEOF_HEADER + this._Chunks.Count * ArchiverChunk.SIZE;
		var chunks = CollectionsMarshal.AsSpan(this._Chunks);
		var compressionOptions = new ZLibCompressionOptions() { CompressionLevel=6 };
		for (var i = 0; i < this._Chunks.Count; ++i)
		{
			ref var chunk = ref chunks[i];
			var pendingChunkStream = this._PendingChunks![i];
			chunk.Offset = headerOffset + totalOffset;
			if (chunk.IsCompressed)
			{
				var compressedChunk = new MemoryStream();
				using (var compressorStream = new GZipStream(compressedChunk, compressionOptions, leaveOpen: true))
				{
					pendingChunkStream.Position = 0;
					pendingChunkStream.CopyTo(compressorStream);
				}
				pendingChunkStream.Dispose();
				this._PendingChunks[i] = pendingChunkStream = compressedChunk;
			}
			chunk.Length = (int)pendingChunkStream.Length;
			totalOffset += (int)pendingChunkStream.Length;
			writer.Write(chunk);
		}
		// Chunks
		foreach (var pendingChunk in this._PendingChunks!)
		{
			pendingChunk.Position = 0;
			pendingChunk.CopyTo(this._Stream);
			pendingChunk.Dispose();
		}
		this._PendingChunks.Clear();
		this._Stream.Flush();
	}
	public Stream WriteChunk(ArchiverChunkType type, byte version = 0, bool isCompressed = false)
	{
		var chunk = new ArchiverChunk() { Type=type, Version=version, IsCompressed=isCompressed };
		this._Chunks.Add(chunk);
		var stream = new ChunkStream();
		this._PendingChunks!.Add(stream);
		return stream;
	}
	/* Static Methods */
	public static IArchiveReader Reader(Stream stream)
	{
		if (!stream.CanRead)
			throw new InvalidOperationException("Tried creating an archive reader from non-readable stream");
		// Header
		var reader = new JJxReader(stream);
		var magic = reader.ReadString(SIZEOF_MAGIC);
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
		reader.Skip(4); // Unknown
		for (var i = 0; i < chunks.Capacity; ++i)
			chunks.Add(reader.ReadObject<ArchiverChunk>());
		return new ArchiveStream(type, stream, chunks);
	}
	public static IArchiveWriter Writer(Stream stream, ArchiveType type)
	{
		if (!stream.CanWrite)
			throw new InvalidOperationException("Tried creating an archive writer from non-writable stream");
		Span<byte> header = stackalloc byte[SIZEOF_MAGIC + sizeof(ushort)];
		switch(type)
		{
			case ArchiveType.Player:
			{
				Encoding.UTF8.GetBytes(MAGIC_PLAYER, header);
			} break;
			case ArchiveType.World:
			case ArchiveType.Adventure:
			{
				Encoding.UTF8.GetBytes(MAGIC_WORLD, header);
			} break;
		}
		BinaryPrimitives.WriteUInt16LittleEndian(header.Slice(SIZEOF_MAGIC), (ushort)type);
		stream.Write(header);
		return new ArchiveStream(type, stream);
	}
	/* Properties */
	// Common
	public readonly ArchiveType Type;
	private readonly Stream _Stream;
	private readonly List<ArchiverChunk> _Chunks;
	// Reading
    ArchiveType IArchiveReader.Type => this.Type;
    IEnumerable<ArchiverChunk> IArchiveReader.Chunks => this._Chunks;
	// Writing
	private readonly List<Stream>? _PendingChunks = null;
	/* Class Properties */
	private const int SIZEOF_HEADER   = 12;
	private const int SIZEOF_MAGIC    =  4;
	private const string MAGIC_PLAYER = "JJXC";
	private const string MAGIC_WORLD  = "JJXM";
}
