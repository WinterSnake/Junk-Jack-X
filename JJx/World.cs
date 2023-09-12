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
	Segment[0x144     :      0x147] = Planet                              | Length: 4   (0x4)  | Type: enum flags
	Segment[0x148]                  = Season                              | Length: 1   (0x1)  | Type: enum                     | Parent: Season
	Segment[0x149]                  = Gamemode                            | Length: 1   (0x1)  | Type: enum                     | Parent: Gamemode
	Segment[0x14A]                  = World Size                          | Length: 1   (0x1)  | Type: enum                     | Parent: Size
	Segment[0x14B]                  = Sky Size                            | Length: 1   (0x1)  | Type: enum                     | Parent: Size
	Segment[0x14C     :      0x14F] = UNKNOWN FOR NOW                     | Length: 4   (0x4)  | Type: ???
	Segment[0x150     :      0x1CF] = Padding                             | Length: 128 (0x80) | Type: uint32[32] = {0}
	----------------------------------------------------------------------------------------------------------------------------------------------------------------------

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class World
{
	/* Constructors */
	public World(string name, string author = "author")
	{
		this.Id = Guid.NewGuid();
		this.LastPlayed = DateTime.Now;
		this.Version = Version.Latest;
		this.Name = name;
		this.Author = author;
	}
	private World(Guid id, DateTime lastPlayed, Version version, string name, string author)
	{
		this.Id = id;
		this.LastPlayed = lastPlayed;
		this.Version = version;
		this._Name = name;
		this._Author = author;
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
		/// Border
		Console.WriteLine($"position = border: {stream.AtChunk(Chunk.Type.WorldBorders)}");
		/// Blocks
		return new World(id, lastPlayed, version, name, author);
	}
	/* Properties */
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
	/* Class Properties */
	private const byte BUFFER_SIZE           = 32;
	private const byte SIZEOF_UUID           = 16;
	private const byte SIZEOF_TIMESTAMP      =  4;
	private const byte SIZEOF_VERSION        =  4;
	private const byte SIZEOF_NAME           = 32;
	private const byte SIZEOF_AUTHOR         = 16;
}
