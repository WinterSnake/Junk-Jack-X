/*
	Junk Jack X: World

	- Currently only testing with TINY world size

	Sizes in blocks:
		[TINY]:   512 * 128
		[SMALL]:  {x} * {y}
		[NORMAL]: {x} * {y}
		[LARGE]:  {x} * {y}
		[HUGE]:   {x} * {y}

	Segment Breakdown:
	------------------------------------------------------------------------------------------------------------------------
	Segment[0x0   :   0x3] = JJ World Header     | Length: 4   (0x04)  | Type: char[4]
	Segment[0x4   :  0xEF] = UNKNOWN FOR NOW     | Length: 170 (0xEC)  | Type: ??? Possible Header/File Length/CRC
	Segment[0xF0  :  0xFF] = UUID                | Length: 16  (0x10)  | Type: uuid
	Segment[0x100 : 0x107] = UNKNOWN FOR NOW     | Length: 8   (0x8)   | Type: ??? Possible long/DateTime
	Segment[0x108 : 0x118] = Name                | Length: 16  (0x10)  | Type: char*
	Segment[0x118 : 0x148] = UNKNOWN FOR NOW     | Length: 49  (0x31)  | Type: ???
	Segment[0x149]         = Gamemode            | Length: 1   (0x1)   | Type: enum            | Parent: Gamemode
	Segment[0x14A : 0x---] = UNKNOWN FOR NOW     | Length: ?   (0x)    | Type: ???
	------------------------------------------------------------------------------------------------------------------------

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace JJx;

public enum Gamemode: byte
{
	Survival = 0x0,
	Creative,
	Flat
}

public sealed class World
{
	/* Constructors */
	public World(string name, Gamemode gamemode): this(Guid.NewGuid(), name, gamemode) { }
	private World(Guid id, string name, Gamemode gamemode)
	{
		this.Id = id;
		this._Name = name;
		this.Gamemode = gamemode;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var byteCount = 0;
		var workingData = new byte[64];
		//----Unknown----\\
		stream.Seek(0xF0, SeekOrigin.Current);
		// Uuid
		var uuid = this.Id.ToByteArray();
		await stream.WriteAsync(uuid, 0, 0x10);
		//----Unknown----\\
		stream.Seek(0x8, SeekOrigin.Current);
		// Name
		byteCount = Encoding.ASCII.GetBytes(this._Name, 0, this._Name.Length, workingData, 0);
		for (var i = byteCount; i < 0x10; ++i) { workingData[i] = 0; }
		await stream.WriteAsync(workingData, 0, 0x10);
		//----Unknown----\\
		stream.Seek(0x31, SeekOrigin.Current);
		// Gamemode
		stream.WriteByte((byte)this.Gamemode);
		//----Unknown----\\
	}
	/* Static Methods */
	public static async Task<World> FromStream(Stream stream)
	{
		var workingData = new byte[64];
		//----Unknown----\\
		stream.Seek(0xF0, SeekOrigin.Current);
		// Uuid
		await stream.ReadAsync(workingData, 0, 0x10);
		Guid id = new Guid(new Span<byte>(workingData).Slice(0, 0x10));
		//----Unknown----\\
		stream.Seek(0x8, SeekOrigin.Current);
		// Name
		await stream.ReadAsync(workingData, 0, 0x10);
		var name = Encoding.ASCII.GetString(
			new Span<byte>(workingData).Slice(0, Array.IndexOf(workingData, byte.MinValue))
		);
		//----Unknown----\\
		stream.Seek(0x31, SeekOrigin.Current);
		// Gamemode
		var gamemode = (Gamemode)stream.ReadByte();
		//----Unknown----\\
		return new World(id, name, gamemode);
	}
	/* Properties */
	public readonly Guid Id;
	private string _Name;
	public string Name {
		// Min Length: 1 | Max length: 15 characters -- 16 = null termination
		get { return this._Name; }
		set {
			if (String.IsNullOrEmpty(value)) return;
			else if (value.Length < 16) this._Name = value;
			else this._Name = value.Substring(0, 15);
		}
	}
	public Gamemode Gamemode;
}
