/*
	Junk Jack X: World
	- Adventure

	Segment Breakdown:
	----------------------------------------------------------------------------------------------------------------
	:<Info>
	Segment[0x24 : 0x33] = UUID                  | Length: 16  (0x10) | Type: uuid
	Segment[0x34 : 0x37] = Last Played Timestamp | Length: 4   (0x4)  | Type: DateTime[uint32]
	Segment[0x38 : 0x3C] = Game Version          | Length: 4   (0x4)  | Type: uint32           | Parent: JJx.Version
	Segment[0x3D : 0x5C] = Name                  | Length: 32  (0x20) | Type: char*
	Segment[0x5D : 0x6C] = Author                | Length: 16  (0x10) | Type: char*
	----------------------------------------------------------------------------------------------------------------

	Written By: Ryan Smith
*/
using System;
using System.Threading.Tasks;

namespace JJx;

public sealed class Adventure
{
	/* Constructors */
	public Adventure(string name, string author = "author")
	{
		this.Id = Guid.NewGuid();
		this.LastPlayed = DateTime.Now;
		this.Version = Version.Latest;
		this.Name = name;
		this.Author = author;
	}
	private Adventure(Guid id, DateTime lastPlayed, Version version, string name, string author)
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
	public static async Task<Adventure> Load(string path)
	{
		using var stream = await ArchiverStream.Reader(path);
		if (stream.Type != ArchiverType.Adventure)
			throw new ArgumentException($"Expected adventure stream, found {stream.Type}");
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

		return new Adventure(id, lastPlayed, version, name, author);
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
	/* Class Properties */
	private const byte BUFFER_SIZE           =  32;
	private const byte SIZEOF_UUID           =  16;
	private const byte SIZEOF_TIMESTAMP      =   4;
	private const byte SIZEOF_VERSION        =   4;
	private const byte SIZEOF_NAME           =  32;
	private const byte SIZEOF_AUTHOR         =  16;
}
