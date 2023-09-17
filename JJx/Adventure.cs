/*
	Junk Jack X: World
	- Adventure

	Segment Breakdown:
	---------------------------------------------------------------------------------------------------------------------
	:<Info>
	Segment[0x24 :  0x33] = UUID                  | Length: 16  (0x10) | Type: uuid
	Segment[0x34 :  0x37] = Last Played Timestamp | Length: 4    (0x4) | Type: DateTime[uint32]
	Segment[0x38 :  0x3B] = Game Version          | Length: 4    (0x4) | Type: uint32             | Parent: JJx.Version
	Segment[0x3C :  0x5B] = Name                  | Length: 32  (0x20) | Type: char*
	Segment[0x5C :  0x6B] = Author                | Length: 16  (0x10) | Type: char*
	Segment[0x6C :  0x6D] = World.Width           | Length: 2    (0x2) | Type: uint16             | Parent: Blocks.Width
	Segment[0x6E :  0x6F] = World.Height          | Length: 2    (0x2) | Type: uint16             | Parent: Blocks.Height
	Segment[0x70 :  0x77] = Padding               | Length: 8    (0x8) | Type: uint64
	Segment[0x78 :  0x7B] = Planet                | Length: 4    (0x4) | Type: enum flag[uint32]
	Segment[0x7C]         = Season                | Length: 1    (0x1) | Type: enum[uint8]        | Parent: Season
	Segment[0x7D]         = Gamemode              | Length: 1    (0x1) | Type: enum[uint8]        | Parent: Gamemode
	Segment[0x7E]         = World Size            | Length: 1    (0x1) | Type: enum[uint8]        | Parent: InitSize
	Segment[0x7F]         = Sky Size              | Length: 1    (0x1) | Type: enum[uint8]        | Parent: InitSize
	Segment[0x80 :  0x83] = Language              | Length: 4    (0x4) | Type: char[4]
	Segment[0x84 : 0x103] = Padding               | Length: 128 (0x80) | Type: uint32[32] = {0}
	:<Portal>
	Segment[0x104 : 0x107] = Portal Count         | Length: 4    (0x4) | Type: uint32
		Portal[]
	---------------------------------------------------------------------------------------------------------------------

	Written By: Ryan Smith
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Adventure
{
	/* Constructors */
	public Adventure(
		string name, InitSize worldSize, string author = "author", InitSize skySize = InitSize.Normal,
		Season season = Season.None, Planet planet = Planet.Terra, (ushort, ushort)? customSize = null
	)
	{
		this.Id = Guid.NewGuid();
		this.LastPlayed = DateTime.Now;
		this.Version = Version.Latest;
		this.Name = name;
		this.Author = author;
		switch (worldSize)
		{
			case InitSize.Tiny:
				this.Size = (512, 128); break;
			case InitSize.Small:
				this.Size = (768, 256); break;
			case InitSize.Normal:
				this.Size = (1024, 256); break;
			case InitSize.Large:
				this.Size = (2048, 384); break;
			case InitSize.Huge:
				this.Size = (4096, 512); break;
			case InitSize.Custom:
			default:
				// Minimum working size: 128 x 64
				this.Size = customSize.Value; break;
		}
		this.Season = season;
		this.WorldInitSize = worldSize;
		this.SkyInitSize = skySize;
	}
	private Adventure(
		Guid id, DateTime lastPlayed, Version version, string name, string author,
		(ushort, ushort) size, Planet planet, Season season, Gamemode gamemode,
		InitSize worldInitSize, InitSize skyInitSize, string language, Portal[] portals
	)
	{
		this.Id = id;
		this.LastPlayed = lastPlayed;
		this.Version = version;
		this._Name = name;
		this._Author = author;
		this.Size = size;
		this.Planet = planet;
		this.Season = season;
		this.Gamemode = gamemode;
		this.WorldInitSize = worldInitSize;
		this.SkyInitSize = skyInitSize;
		this.Language = language;
		this.Portals.AddRange(portals);
	}
	/* Instance Methods */
	public async Task Save(string path)
	{
		using var stream = await ArchiverStream.Writer(path, ArchiverType.Adventure);
		var workingData = new byte[BUFFER_SIZE];
		/// Info
		using (var chunk = stream.NewChunk(ChunkType.WorldInfo))
		{
			// Uuid
			var uuid = this.Id.ToByteArray();
			for (var i = uuid.Length - 1; i <= 0; --i)
				workingData[uuid.Length - i] = uuid[i];
			await chunk.WriteAsync(uuid, 0, uuid.Length);
			// Last Played
			Utilities.ByteConverter.Write(new Span<byte>(workingData), this.LastPlayed,    0);
			// Game Version
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (uint)this.Version, 4);
			await chunk.WriteAsync(workingData, 0, 8);
			// Name
			Utilities.ByteConverter.Write(new Span<byte>(workingData), this._Name, length: SIZEOF_NAME);
			await chunk.WriteAsync(workingData, 0, SIZEOF_NAME);
			// Author
			Utilities.ByteConverter.Write(new Span<byte>(workingData), this._Author, length: SIZEOF_AUTHOR);
			await chunk.WriteAsync(workingData, 0, SIZEOF_AUTHOR);
			// World Size
			Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Size.Width,  0);
			Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Size.Height, 2);
			// Padding-00
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (ulong)0, 4);
			// Planet
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (uint)this.Planet, 12);
			// Season
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (byte)this.Season, 16);
			// Gamemode
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (byte)this.Gamemode, 17);
			// World Init Size
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (byte)this.WorldInitSize, 18);
			// Sky Init Size
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (byte)this.SkyInitSize, 19);
			// Language
			Utilities.ByteConverter.Write(new Span<byte>(workingData), this.Language, offset: 20, length: SIZEOF_LANGUAGE);
			await chunk.WriteAsync(workingData, 0, 24);
			// Padding
			workingData = new byte[SIZEOF_PADDING1];
			await chunk.WriteAsync(workingData, 0, workingData.Length);
		}
		/// Portals
		using (var chunk = stream.NewChunk(ChunkType.AdventurePortals))
		{
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (uint)this.Portals.Count, 0);
			await chunk.WriteAsync(workingData, 0, SIZEOF_PORTALCOUNT);
			foreach (var portal in this.Portals)
				await portal.ToStream(chunk);
		}
	}
	/* Static Methods */
	public static async Task<Adventure> Load(string path)
	{
		using var stream = await ArchiverStream.Reader(path);
		if (stream.Type != ArchiverType.Adventure)
			throw new ArgumentException($"Expected adventure stream, found {stream.Type} stream");
		// DEBUG
		#if (PRINT_CHUNKS)
			Console.WriteLine(String.Join("\n", stream.GetChunkStrings()));
		#endif
		int bytesRead = 0;
		var workingData = new byte[BUFFER_SIZE];
		/// Info
		// Uuid
		while (bytesRead < SIZEOF_UUID)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_UUID - bytesRead);
		var id = new Guid(new Span<byte>(workingData, 0, SIZEOF_UUID));
		// Last Played
		bytesRead = 0;
		while (bytesRead < SIZEOF_TIMESTAMP)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TIMESTAMP - bytesRead);
		var lastPlayed = Utilities.ByteConverter.GetDateTime(new Span<byte>(workingData));
		// Game Version
		bytesRead = 0;
		while (bytesRead < SIZEOF_VERSION)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_VERSION - bytesRead);
		var version = (Version)Utilities.ByteConverter.GetUInt32(new Span<byte>(workingData));
		// Name
		bytesRead = 0;
		while (bytesRead < SIZEOF_NAME)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_NAME - bytesRead);
		var name = Utilities.ByteConverter.GetString(new Span<byte>(workingData));
		// Author
		bytesRead = 0;
		while (bytesRead < SIZEOF_AUTHOR)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_AUTHOR - bytesRead);
		var author = Utilities.ByteConverter.GetString(new Span<byte>(workingData));
		// World Size
		bytesRead = 0;
		while (bytesRead < SIZEOF_POSITIONS)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_POSITIONS - bytesRead);
		(ushort width, ushort height) worldSize = (
			Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData),  0),
			Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData),  2)
		);
		// Padding-00
		bytesRead = 0;
		while (bytesRead < SIZEOF_PADDING0)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_PADDING0 - bytesRead);
		// Planet
		bytesRead = 0;
		while (bytesRead < SIZEOF_PLANET)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_PLANET - bytesRead);
		var planet = (Planet)Utilities.ByteConverter.GetUInt32(new Span<byte>(workingData));
		// Season
		var season = (Season)((byte)stream.ReadByte());
		// Gamemode
		var gamemode = (Gamemode)((byte)stream.ReadByte());
		// World Init Size
		var worldInitSize = (InitSize)((byte)stream.ReadByte());
		// Sky Init Size
		var skyInitSize = (InitSize)((byte)stream.ReadByte());
		// Language
		bytesRead = 0;
		while (bytesRead < SIZEOF_LANGUAGE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_LANGUAGE - bytesRead);
		var language = Utilities.ByteConverter.GetString(new Span<byte>(workingData));
		// Padding-01
		stream.Seek(SIZEOF_PADDING1, SeekOrigin.Current);
		/// Portals
		Console.WriteLine($"Position: {stream.Position:X8} | Postion = Portals Location: {stream.IsAtChunk(ChunkType.AdventurePortals)} | Size: {stream.GetChunkSize(ChunkType.AdventurePortals):X4}");
		bytesRead = 0;
		while (bytesRead < SIZEOF_PORTALCOUNT)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_PORTALCOUNT - bytesRead);
		var portalCount = Utilities.ByteConverter.GetUInt32(new Span<byte>(workingData));
		Console.WriteLine($"Portal Count: {portalCount}");
		var portals = new Portal[portalCount];
		for (var i = 0; i < portals.Length; ++i)
			portals[i] = await Portal.FromStream(stream);

		return new Adventure(
			id, lastPlayed, version, name, author, worldSize, planet,
			season, gamemode, worldInitSize, skyInitSize, language, portals
		);
	}
	/* Properties */
	// Info
	public readonly Guid Id;
	public DateTime LastPlayed;
	public readonly Version Version;
	private string _Name;
	public string Name {
		get { return this._Name; }
		set {
			if (String.IsNullOrEmpty(value)) return;
			else if (value.Length < SIZEOF_NAME) this._Name = value;
			else this._Name = value.Substring(0, SIZEOF_NAME - 1);
		}
	}
	private string _Author;
	public string Author {
		get { return this._Author; }
		set {
			if (String.IsNullOrEmpty(value)) return;
			else if (value.Length < SIZEOF_AUTHOR) this._Author = value;
			else this._Author = value.Substring(0, SIZEOF_AUTHOR - 1);
		}
	}
	public (ushort Width, ushort Height) Size { get; private set; }
	public Planet Planet;
	public Season Season;
	public Gamemode Gamemode { get; private set; } = Gamemode.Adventure;
	public InitSize WorldInitSize;
	public InitSize SkyInitSize;
	public readonly string Language = "en";
	// Portals
	public readonly List<Portal> Portals = new List<Portal>();
	/* Class Properties */
	private const byte BUFFER_SIZE           =  32;
	private const byte SIZEOF_UUID           =  16;
	private const byte SIZEOF_TIMESTAMP      =   4;
	private const byte SIZEOF_VERSION        =   4;
	private const byte SIZEOF_NAME           =  32;
	private const byte SIZEOF_AUTHOR         =  16;
	private const byte SIZEOF_POSITIONS      =   4;  // World.Width, World.Height
	private const byte SIZEOF_PADDING0       =   8;
	private const byte SIZEOF_PLANET         =   4;
	private const byte SIZEOF_LANGUAGE       =   4;
	private const byte SIZEOF_PADDING1       = 128;
	private const byte SIZEOF_PORTALCOUNT    =   4;
}
