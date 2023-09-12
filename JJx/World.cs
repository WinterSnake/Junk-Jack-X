/*
	Junk Jack X: World

	Segment Breakdown:
	----------------------------------------------------------------------------------------------------------------------------------------------------------------------
	:<Info>
	Segment[0xF0      :       0xFF] = UUID                                | Length: 16  (0x10) | Type: uuid
	Segment[0x100     :      0x103] = Last Played Timestamp               | Length: 4   (0x4)  | Type: DateTime / Epoch / Timestamp / uint32
	Segment[0x104     :      0x107] = Game Version                        | Length: 4   (0x4)  | Type: uint32                   | Parent: JJx.Version
	Segment[0x108     :      0x127] = Name                                | Length: 32  (0x20) | Type: char*
	Segment[0x128     :      0x137] = Author                              | Length: 16  (0x10) | Type: char*
	Segment[0x138     :      0x139] = World.Width                         | Length: 2   (0x2)  | Type: uint16                   | Parent: Blocks.Width
	Segment[0x13A     :      0x13B] = World.Height                        | Length: 2   (0x2)  | Type: uint16                   | Parent: Blocks.Height
	Segment[0x13C     :      0x13D] = Player.X                            | Length: 2   (0x2)  | Type: uint16                   | Parent: Player.X
	Segment[0x13E     :      0x13F] = Player.Y                            | Length: 2   (0x2)  | Type: uint16                   | Parent: Player.Y
	Segment[0x140     :      0x141] = Spawn.X                             | Length: 2   (0x2)  | Type: uint16                   | Parent: Spawn.X
	Segment[0x142     :      0x143] = Spawn.Y                             | Length: 2   (0x2)  | Type: uint16                   | Parent: Spawn.Y
	Segment[0x144     :      0x147] = Planet                              | Length: 4   (0x4)  | Type: enum flag[uint32]
	Segment[0x148]                  = Season                              | Length: 1   (0x1)  | Type: enum[uint8]              | Parent: Season
	Segment[0x149]                  = Gamemode                            | Length: 1   (0x1)  | Type: enum[uint8]              | Parent: Gamemode
	Segment[0x14A]                  = World Size                          | Length: 1   (0x1)  | Type: enum[uint8]              | Parent: InitSize
	Segment[0x14B]                  = Sky Size                            | Length: 1   (0x1)  | Type: enum[uint8]              | Parent: InitSize
	Segment[0x14C     :      0x14F] = UNKNOWN FOR NOW                     | Length: 4   (0x4)  | Type: ???
	Segment[0x150     :      0x1CF] = Padding                             | Length: 128 (0x80) | Type: uint32[32] = {0}
	G:<Border>

	----------------------------------------------------------------------------------------------------------------------------------------------------------------------

	Written By: Ryan Smith
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.IO.Compression;

namespace JJx;

public enum Gamemode : byte
{
	Survival = 0x0,
	Creative,
	Flat
}

public sealed class World
{
	/* Constructors */
	public World(
		string name, InitSize worldSize, string author = "author", InitSize skySize = InitSize.Normal,
		Gamemode gamemode = Gamemode.Creative, Season season = Season.None, Planet planet = Planet.Terra,
		(ushort, ushort)? customSize = null
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
				this.Size = customSize.Value; break;
		}
		this.Player = (0, 1);
		this.Spawn  = (0, 1);
		this.Season = season;
		this.Gamemode = gamemode;
		this.WorldInitSize = worldSize;
		this.SkyInitSize = skySize;
		this.Borders = new ushort[this.Size.Width];
		for (var i = 0; i < this.Borders.Length; ++i)
			this.Borders[i] = (ushort)(this.Size.Height / 2);
	}
	private World(
		Guid id, DateTime lastPlayed, Version version, string name, string author,
		(ushort, ushort) size, (ushort, ushort) player, (ushort, ushort) spawn, Planet planet,
		Season season, Gamemode gamemode, InitSize worldInitSize, InitSize skyInitSize, ushort[] borders,
		Chest[] chests
	)
	{
		this.Id = id;
		this.LastPlayed = lastPlayed;
		this.Version = version;
		this._Name = name;
		this._Author = author;
		this.Size = size;
		this.Player = player;
		this.Spawn = spawn;
		this.Planet = planet;
		this.Season = season;
		this.Gamemode = gamemode;
		this.WorldInitSize = worldInitSize;
		this.SkyInitSize = skyInitSize;
		this.Borders = borders;
		this.Chests.AddRange(chests);
	}
	/* Instance Methods */
	public async Task Save(string path)
	{
		using var stream = await ArchiverStream.Writer(path, ArchiverType.Map);
		var workingData = new byte[BUFFER_SIZE];
	}
	/* Static Methods */
	public static async Task<World> Load(string path)
	{
		using var stream = await ArchiverStream.Reader(path);
		if (stream.Type != ArchiverType.Map)
			throw new ArgumentException($"Expected world stream, found {stream.Type}");
		// DEBUG
		#if DEBUG
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
		// {World Size, Player Location, Spawn Location}
		bytesRead = 0;
		while (bytesRead < SIZEOF_POSITIONS)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_POSITIONS - bytesRead);
			// World Size
			(ushort width, ushort height) worldSize = (
				Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData),  0),
				Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData),  2)
			);
			// Player Location
			var playerPos = (
				Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData),  4),
				Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData),  6)
			);
			// Spawn Location
			var spawnPos = (
				Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData),  8),
				Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData), 10)
			);
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
		// =UNKNOWN=
		stream.Seek(4, SeekOrigin.Current);
		// Padding
		stream.Seek(SIZEOF_PADDING, SeekOrigin.Current);
		/// Border
		var borders = new ushort[worldSize.width];
		for (var i = 0; i < borders.Length / (workingData.Length / 2); ++i)
		{
			bytesRead = 0;
			while (bytesRead < workingData.Length)
				bytesRead += await stream.ReadAsync(workingData, bytesRead, workingData.Length - bytesRead);
			for (var j = 0; j < workingData.Length / 2; ++j)
			{
				var index = j + (i * (workingData.Length / 2));
				var offset = j * 2;
				borders[index] = Utilities.ByteConverter.GetUInt16(new Span<byte>(workingData), offset);
			}
		}
		/// Blocks
		var blocksCompressionSize = stream.GetChunkSize(Chunk.Type.WorldBlocks);
		using (var blocksCompressedStream = new MemoryStream((int)blocksCompressionSize))
		{
			// Clone block compressed data to memory stream
			bytesRead = 0;
			while (bytesRead < blocksCompressionSize)
				bytesRead += await stream.ReadAsync(blocksCompressedStream.GetBuffer(), bytesRead, (int)blocksCompressionSize - bytesRead);
			blocksCompressedStream.SetLength(blocksCompressionSize.Value);
			// Decompress stream
			using (var blocksDecompressedStream = new GZipStream(blocksCompressedStream, CompressionMode.Decompress))
			{
			}
		}
		/// Time
		Console.WriteLine($"Position: {stream.Position:X8} | Postion = Time Location: {stream.IsAtChunk(Chunk.Type.WorldTime)} | Size: {stream.GetChunkSize(Chunk.Type.WorldTime):X4}");
		stream.Seek(8, SeekOrigin.Current);
		/// Weather
		Console.WriteLine($"Position: {stream.Position:X8} | Postion = Weather Location: {stream.IsAtChunk(Chunk.Type.WorldWeather)} | Size: {stream.GetChunkSize(Chunk.Type.WorldWeather):X4}");
		stream.Seek(8, SeekOrigin.Current);
		/// Chests
		bytesRead = 0;
		while (bytesRead < SIZEOF_TILECOUNT)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TILECOUNT - bytesRead);
		var chestCount = Utilities.ByteConverter.GetUInt32(new Span<byte>(workingData));
		var chests = new Chest[chestCount];
		for (var i = 0; i < chestCount; ++i)
			chests[i] = await Chest.FromStream(stream);
		/// Forges
		Console.WriteLine($"Position: {stream.Position:X8} | Postion = Forges Location: {stream.IsAtChunk(Chunk.Type.WorldForges)} | Size: {stream.GetChunkSize(Chunk.Type.WorldForges):X4}");
		bytesRead = 0;
		while (bytesRead < SIZEOF_TILECOUNT)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TILECOUNT - bytesRead);
		var forgeCount = Utilities.ByteConverter.GetUInt32(new Span<byte>(workingData));
		Console.WriteLine($"Forges: {forgeCount}");
		/// Signs
		Console.WriteLine($"Position: {stream.Position:X8} | Postion = Signs Location: {stream.IsAtChunk(Chunk.Type.WorldSigns)} | Size: {stream.GetChunkSize(Chunk.Type.WorldSigns):X4}");
		bytesRead = 0;
		while (bytesRead < SIZEOF_TILECOUNT)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TILECOUNT - bytesRead);
		var signCount = Utilities.ByteConverter.GetUInt32(new Span<byte>(workingData));
		Console.WriteLine($"Signs: {signCount}");
		/// Stables
		Console.WriteLine($"Position: {stream.Position:X8} | Postion = Stables Location: {stream.IsAtChunk(Chunk.Type.WorldStables)} | Size: {stream.GetChunkSize(Chunk.Type.WorldStables):X4}");
		bytesRead = 0;
		while (bytesRead < SIZEOF_TILECOUNT)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TILECOUNT - bytesRead);
		var stableCount = Utilities.ByteConverter.GetUInt32(new Span<byte>(workingData));
		Console.WriteLine($"Stables: {stableCount}");
		/// Labs
		Console.WriteLine($"Position: {stream.Position:X8} | Postion = Labs Location: {stream.IsAtChunk(Chunk.Type.WorldLabs)} | Size: {stream.GetChunkSize(Chunk.Type.WorldLabs):X4}");
		bytesRead = 0;
		while (bytesRead < SIZEOF_TILECOUNT)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TILECOUNT - bytesRead);
		var labCount = Utilities.ByteConverter.GetUInt32(new Span<byte>(workingData));
		Console.WriteLine($"Labs: {labCount}");
		/// Shelves
		Console.WriteLine($"Position: {stream.Position:X8} | Postion = Shelves Location: {stream.IsAtChunk(Chunk.Type.WorldShelves)} | Size: {stream.GetChunkSize(Chunk.Type.WorldShelves):X4}");
		bytesRead = 0;
		while (bytesRead < SIZEOF_TILECOUNT)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TILECOUNT - bytesRead);
		var shelfCount = Utilities.ByteConverter.GetUInt32(new Span<byte>(workingData));
		Console.WriteLine($"Shelves: {shelfCount}");
		/// =UNKNOWN 07=
		Console.WriteLine($"Position: {stream.Position:X8} | Postion = UNKOWN Location: {stream.IsAtChunk(Chunk.Type.WorldUnknown09)} | Size: {stream.GetChunkSize(Chunk.Type.WorldUnknown09):X4}");
		stream.Seek(4, SeekOrigin.Current);
		/// =UNKNOWN 08=
		Console.WriteLine($"Position: {stream.Position:X8} | Postion = UNKOWN Location: {stream.IsAtChunk(Chunk.Type.WorldUnknown10)} | Size: {stream.GetChunkSize(Chunk.Type.WorldUnknown10):X4}");
		stream.Seek(4, SeekOrigin.Current);
		/// =UNKNOWN 09=
		Console.WriteLine($"Position: {stream.Position:X8} | Postion = UNKOWN Location: {stream.IsAtChunk(Chunk.Type.WorldUnknown11)} | Size: {stream.GetChunkSize(Chunk.Type.WorldUnknown11):X4}");
		stream.Seek(4, SeekOrigin.Current);
		/// Locks
		Console.WriteLine($"Position: {stream.Position:X8} | Postion = Locks Location: {stream.IsAtChunk(Chunk.Type.WorldLocks)} | Size: {stream.GetChunkSize(Chunk.Type.WorldLocks):X4}");
		stream.Seek(4, SeekOrigin.Current);
		/// =UNKNOWN 11=
		Console.WriteLine($"Position: {stream.Position:X8} | Postion = UNKOWN Location: {stream.IsAtChunk(Chunk.Type.WorldUnknown13)} | Size: {stream.GetChunkSize(Chunk.Type.WorldUnknown13):X4}");
		stream.Seek(8, SeekOrigin.Current);
		/// =UNKNOWN 12=
		Console.WriteLine($"Position: {stream.Position:X8} | Postion = UNKOWN Location: {stream.IsAtChunk(Chunk.Type.WorldUnknown14)} | Size: {stream.GetChunkSize(Chunk.Type.WorldUnknown14):X4}");
		stream.Seek(16, SeekOrigin.Current);
		/// Entities
		Console.WriteLine($"Position: {stream.Position:X8} | Postion = Entities Location: {stream.IsAtChunk(Chunk.Type.WorldEntities)} | Size: {stream.GetChunkSize(Chunk.Type.WorldEntities):X4}");
		bytesRead = 0;
		while (bytesRead < SIZEOF_TILECOUNT)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_TILECOUNT - bytesRead);
		var entityCount = Utilities.ByteConverter.GetUInt32(new Span<byte>(workingData));
		Console.WriteLine($"Entities: {entityCount}");
		return new World(
			id, lastPlayed, version, name, author, worldSize, playerPos, spawnPos,
			planet, season, gamemode, worldInitSize, skyInitSize, borders,
			chests
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
	public (ushort X, ushort Y) Player;
	public (ushort X, ushort Y) Spawn;
	public Planet Planet;
	public Season Season;
	public Gamemode Gamemode;
	public InitSize WorldInitSize;
	public InitSize SkyInitSize;
	// Border
	public ushort[] Borders { get; private set; }
	// Time
	// Weather
	// Containers
	public readonly List<Chest>  Chests   = new List<Chest>();
	public readonly List<Forge>  Forges   = new List<Forge>();
	public readonly List<Sign>   Signs    = new List<Sign>();
	public readonly List<Stable> Stables  = new List<Stable>();
	public readonly List<Lab>    Labs     = new List<Lab>();
	public readonly List<Shelf>  Shelves  = new List<Shelf>();
	public readonly List<Entity> Entities = new List<Entity>();
	/* Class Properties */
	private const byte BUFFER_SIZE           =  32;
	private const byte SIZEOF_UUID           =  16;
	private const byte SIZEOF_TIMESTAMP      =   4;
	private const byte SIZEOF_VERSION        =   4;
	private const byte SIZEOF_NAME           =  32;
	private const byte SIZEOF_AUTHOR         =  16;
	private const byte SIZEOF_POSITIONS      = 2 * 6;  // World.Width, World.Height, Player.X, Player.Y, Spawn.X, Spawn.Y
	private const byte SIZEOF_PLANET         =   4;
	private const byte SIZEOF_PADDING        = 128;
	private const byte SIZEOF_TILECOUNT      =   4;
}
