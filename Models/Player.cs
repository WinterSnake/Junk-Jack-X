/*
	Junk Jack X Editor: Models
	- Player

	Player class references both the player file and player stats file for creating, loading, editing, and saving.
	This file documents both the hex offsets (and length) and their C equivalent types.

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace JJx.Models;

public enum Color : byte
{
	White       = 0x0,
	Grey        = 0x1,
	Black       = 0x2,
	Brown       = 0x3,
	DarkBrown   = 0x4,
	LightBrown  = 0x5,
	Blonde      = 0x6,
	DirtyBlonde = 0x7,
	LightBlonde = 0x8,
	Ginger      = 0x9,
	Red         = 0xA,
	Purple      = 0xB,
	Blue        = 0xC,
	Teal        = 0xD,
	Green       = 0xE,
	Yellow      = 0xF
}

public class Player
{
	/* Constructor */
	public Player(string name)
	{
		this.Id = Guid.NewGuid();
		this._Name = name;
	}
	public Player(Guid id, string name, ushort features)
	{
		this.Id = id;
		this._Name = name;
		this._Features = features;
	}
	/* Static Methods */
	public static async Task<Player> FromStream(Stream stream)
	{
		var workingData = new byte[64];
		// Uuid
		stream.Seek(0x48, SeekOrigin.Begin);
		await stream.ReadAsync(workingData, 0, 0x10);
		Guid id = new Guid(new Span<byte>(workingData).Slice(0, 0x10));
		// Name
		await stream.ReadAsync(workingData, 0, 0x10);
		string name = Encoding.ASCII.GetString(
			new Span<byte>(workingData).Slice(0, Array.IndexOf(workingData, byte.MinValue))
		);
		// Features
		stream.Seek(0xC, SeekOrigin.Current);
		await stream.ReadAsync(workingData, 0, 0x02);
		ushort features = (ushort)(((workingData[0]) << 4) | workingData[1]);
		return new Player(id, name, features);
	}
	/* Properties */
	/// Offset Properties
	                               // Offset: 0x04 | Length: 0x44 [End = 0x47] | Type: UNKNOWN FOR NOW [Possible Header/File Length/Crc?]
	public Guid Id { get; init; }  // Offset: 0x48 | Length: 0x10 [End = 0x57] | Type: Uuid
	private string _Name;          // Offset: 0x58 | Length: 0x10 [End = 0x67] | Type: char*
	                               // Offset: 0x68 | Length: 0x0C [End = 0x73] | Type: UNKNOWN FOR NOW
	private ushort _Features;      // Offset: 0x74 | Length: 0x02 [End = 0x75] | Type: byte/enum | byte
	/// Modifable Properties
	public string Name {
		// Max length: 15 characters + null terminator
		get { return this._Name; }
		set {
			if (value.Length < 16) this._Name = value;
			this._Name = value.Substring(0, 15);
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
	public Color HairColor
	{
		get { return (Color)(this._Features >> 8); }
		set {
			this._Features = (ushort)(((ushort)value << 8) | (this._Features & 0xFF));
		}
	}
}
