/*
	Junk Jack X Editor: Player

	Player class references both the player file and player stats file for creating, loading, editing, and saving.
	This file documents both the hex offsets (and length) and their C equivalent types.

	Segment Breakdown:
	------------------------------------------------------------------------------------------------------------------------
	Segment[0x0]  - Segment[0x3]  = JJ Character Header | Length: 4  (0x04) | Type: char[4]
	Segment[0x4]  - Segment[0x47] = UNKNOWN FOR NOW     | Length: 71 (0x47) | Type: ??? Possible Header/File Length/CRC
	Segment[0x48] - Segment[0x57] = UUID                | Length: 16 (0x10) | Type: uuid
	Segment[0x58] - Segment[0x67] = Name                | Length: 16 (0x10) | Type: char*
	Segment[0x68] - Segment[0x6F] = UNKNOWN FOR NOW     | Length: 8  (0x08) | Type: ???
	Segment[0x70]                 = Gameplay Flags      | Length: 1  (0x01) | Type: enum flag | Parent: Gameplay.Flags
	Segment[0x71] - Segment[0x73] = UNKNOWN FOR NOW     | Length: 3  (0x03) | Type: ???
	Segment[0x74]                 = Hair Color          | Length: 1  (0x01) | Type: enum      | Parent: _Features
	Segment[0x75]                 = Gender/Skin/Hair    | Length: 1  (0x01) | Type: bitfield  | Parent: _Features
	Segment[0x76] - Segment[0x77] = UNKNOWN FOR NOW     | Length: 2  (0x02) | Type: ???
	Segment[0x78]                 = Gameplay Difficulty | Length: 1  (0x01) | Type: enum      | Parent: Gameplay.Difficulty
	------------------------------------------------------------------------------------------------------------------------

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace JJx;

public sealed class Player
{
	/* Constructor */
	public Player(string name)
	{
		this.Id = Guid.NewGuid();
		this._Name = name;
		this.Gameplay = new Gameplay(0, 0);
	}
	private Player(Guid id, string name, ushort features, Gameplay gameplay)
	{
		this.Id = id;
		this._Name = name;
		this._Features = features;
		this.Gameplay = gameplay;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var byteCount = 0;
		var workingData = new byte[64];
		// Uuid
		stream.Seek(0x48, SeekOrigin.Current);
		var uuid = this.Id.ToByteArray();
		await stream.WriteAsync(uuid, 0, 0x10);
		// Name
		byteCount = Encoding.ASCII.GetBytes(this._Name, 0, this._Name.Length, workingData, 0);
		for (var i = byteCount; i < 0x10; ++i) { workingData[i] = 0; }
		await stream.WriteAsync(workingData, 0, 0x10);
		// Gameplay: Flags
		stream.Seek(0x8, SeekOrigin.Current);
		stream.WriteByte((byte)this.Gameplay.Flags);
		// Features
		stream.Seek(0x3, SeekOrigin.Current);
		workingData[0] = (byte)((this._Features & 0xFF00) >> 4);
		workingData[1] = (byte)(this._Features & 0xFF);
		await stream.WriteAsync(workingData, 0, 0x2);
		// Gameplay: Difficulty
		stream.Seek(0x2, SeekOrigin.Current);
		stream.WriteByte((byte)this.Gameplay.Difficulty);
	}
	/* Static Methods */
	public static async Task<Player> FromStream(Stream stream)
	{
		var workingData = new byte[64];
		// Uuid
		stream.Seek(0x48, SeekOrigin.Current);
		await stream.ReadAsync(workingData, 0, 0x10);
		Guid id = new Guid(new Span<byte>(workingData).Slice(0, 0x10));
		// Name
		await stream.ReadAsync(workingData, 0, 0x10);
		string name = Encoding.ASCII.GetString(
			new Span<byte>(workingData).Slice(0, Array.IndexOf(workingData, byte.MinValue))
		);
		// Gameplay: Flags
		stream.Seek(0x8, SeekOrigin.Current);
		var flags = stream.ReadByte();
		// Features
		stream.Seek(0x3, SeekOrigin.Current);
		await stream.ReadAsync(workingData, 0, 0x02);
		ushort features = (ushort)(((workingData[0]) << 4) | workingData[1]);
		// Gameplay: Difficulty
		stream.Seek(0x2, SeekOrigin.Current);
		var difficulty = stream.ReadByte();
		// -Gameplay
		var gameplay = new Gameplay((byte)difficulty, (byte)flags);
		return new Player(id, name, features, gameplay);
	}
	/* Properties */
	/// Offset Properties
	public Guid Id { get; init; }
	private string _Name;
	public ushort _Features;
	public Gameplay Gameplay;
	/// Modifable Properties
	public string Name {
		// Min Length: 1 | Max length: 15 characters -- 16 = null termination
		get { return this._Name; }
		set {
			if (value.Length < 16) this._Name = value;
			else this._Name = value.Substring(0, 15);
		}
	}
	public bool Gender
	{
		// Male: 0 | Female: 1
		get { return ((this._Features & 0x10) >> 4) == 1; }
		set {
			this._Features = (ushort)((this._Features & 0xFFEF) | (Convert.ToByte(value) << 4));
		}
	}
	public byte SkinTone
	{
		// Value range: 0x0 - 0x4 [5]
		get { return (byte)((this._Features & 0xE0) >> 5); }
		set {
			value = Math.Min(value, (byte)0x04);
			this._Features = (ushort)((this._Features & 0xFF1F) | (value << 5));
		}
	}
	public byte HairStyle
	{
		// Value range: 0x0 - 0xD [13]
		get { return (byte)(this._Features & 0xF); }
		set {
			value = Math.Min(value, (byte)0x0D);
			this._Features = (ushort)((this._Features & 0xFFF0) | value);
		}
	}
	public HairColor HairColor
	{
		get { return (HairColor)(this._Features >> 8); }
		set {
			this._Features = (ushort)(((ushort)value << 8) | (this._Features & 0xFF));
		}
	}
}
