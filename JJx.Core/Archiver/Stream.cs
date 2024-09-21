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
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace JJx;

public enum ArchiverStreamType : ushort
{
	Player    = 0x00,
	World     = 0x01,
	Adventure = 0x02,
}

public sealed class ArchiverStream : FileStream
{
	/* Constructor */
	private ArchiverStream(
		ArchiverStreamType type, ArchiverChunk[] chunks,
		SafeFileHandle readerHandle, JJxReader reader
	): base(readerHandle, FileAccess.Read)
	{
		this.Type = type;
		this._Chunks = chunks;
		this._Reader = reader;
	}
	/* Instance Methods */
	/* Static Methods */
	public static ArchiverStream Reader(string filePath)
	{
		var fileReader = new FileStream(filePath, FileMode.Open, FileAccess.Read);
		var reader = new JJxReader(fileReader);
		// Header \\
		var magic = reader.GetString(length: SIZEOF_MAGIC);
		var type = reader.GetEnum<ArchiverStreamType>();
		var chunkCount = reader.GetUInt16();
		#if DEBUG
			Console.WriteLine($"Magic: {magic} | Type: {type} | Chunks: {chunkCount}");
		#endif
		// --UNKNOWN(4)-- \\
		var unknownData = reader.GetBytes(SIZEOF_UNKNOWN);
		// Chunks
		var chunks = new ArchiverChunk[chunkCount];
		for (var i = 0; i < chunks.Length; ++i)
		{
			chunks[i] = reader.Get<ArchiverChunk>();
			#if DEBUG
				Console.WriteLine($"\t{chunks[i]}");
			#endif
		}
		return new ArchiverStream(type, chunks, fileReader.SafeFileHandle, reader);
	}
	/* Properties */
	public readonly ArchiverStreamType Type;
	internal readonly ArchiverChunk[] _Chunks;
	#nullable enable
	internal readonly JJxReader? _Reader = null;
	#nullable disable
	/* Class Properties */
	private const byte SIZEOF_MAGIC   = 4;
	private const byte SIZEOF_UNKNOWN = 4;
}
