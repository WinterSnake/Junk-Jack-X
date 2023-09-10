/*
	Junk Jack X: Utilities
	- Archiver Stream

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

namespace JJx.Utilities;

internal sealed class ArchiverStream : FileStream
{
	/* Constructors */
	private ArchiverStream(string path, FileMode mode, FileAccess access): base(path, mode, access) {}
	/* Instance Methods */
	/* Static Methods */
	public static async Task<ArchiverStream> Reader(string path)
	{
		var reader = new ArchiverStream(path, FileMode.Open, FileAccess.Read);
		int bytesRead = 0;
		var workingData = new byte[SIZEOF_HEADER];
		while (bytesRead < SIZEOF_HEADER)
			bytesRead += await reader.ReadAsync(workingData, bytesRead, SIZEOF_HEADER - bytesRead);
		/// Header
		var headerMagic = ByteConverter.GetString(new Span<byte>(workingData), 0, 4);
		var headerType  = ByteConverter.GetUInt16(new Span<byte>(workingData), 4);
		var chunkCount  = ByteConverter.GetUInt16(new Span<byte>(workingData), 6);
		// Type
		FileHeader header = FileHeader.Unknown;
		if (headerMagic == "JJXC" && headerType == 0)
			header = FileHeader.Player;
		else if (headerMagic == "JJXM")
		{
			if (headerType == 1)
				header = FileHeader.Map;
			else if (headerType == 2)
				header = FileHeader.Adventure;
		}
		reader.Header = header;
		// =UNKNOWN=
		reader.Seek(4, SeekOrigin.Current);
		// Chunk
		var chunks = new Chunk[chunkCount];
		for (var i = 0; i < chunkCount; ++i)
			chunks[i] = await Chunk.FromStream(reader);
		return reader;
	}
	public static async Task<ArchiverStream> Writer(string path, FileHeader header)
	{
		var writer = new ArchiverStream(path, FileMode.Create, FileAccess.Write);
		return writer;
	}
	/* Properties */
	public FileHeader Header { get; private set; }
	/* Class Properties */
	private const byte SIZEOF_HEADER = 8;
}

public enum FileHeader : byte
{
	Unknown,
	Player,
	Adventure,
	Map,
	Stat
}
