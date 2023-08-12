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

public class Player
{
	/* Constructor */
	public Player(string name)
	{
		this.Id = Guid.NewGuid();
		this._Name = name;
	}
	public Player(Guid id, string name)
	{
		this.Id = id;
		this._Name = name;
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
		return new Player(name);
	}
	/* Properties */
	public Guid Id { get; init; }	// Offset: 0x48 | Length: 0x10 [End = 0x57] | Type: Uuid
	private string _Name;			// Offset: 0x58 | Length: 0x10 [End = 0x67] | Type: char*
	public string Name {
		get { return this._Name; }
		set {
			if (value.Length < 16) this._Name = value;
			this._Name = value.Substring(0, 15);
		}
	}
}
