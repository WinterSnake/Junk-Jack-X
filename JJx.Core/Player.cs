/*
	Junk Jack X: Core
	- Player

	Segment Breakdown:
	--------------------------------------------------------------------------------------------------------------------
	:<Info>
	Segment[0x48  : 0x57]  = UUID                | Length: 16   (0x10) | Type: uuid
	Segment[0x58  : 0x67]  = Name                | Length: 16   (0x10) | Type: char[16]
	Segment[0x68  : 0x6B]  = Game Version        | Length:  4    (0x4) | Type: enum[uint32]      | Parent: Version
	Segment[0x6C  : 0x6F]  = Unlocked Planets    | Length:  4    (0x4) | Type: enum flag[uint32] | Parent: Planet
	Segment[0x70  : 0x73]  = Gameplay Flags      | Length:  4    (0x4) | Type: enum flag[uint32] | Parent: Gameplay.Flag
	Segment[0x74]          = Hair Color          | Length:  1    (0x1) | Type: bitfield          | Parent: Character
	Segment[0x75]          = Gender/Skin/Hair    | Length:  1    (0x1) | Type: bitfield          | Parent: Character
	Segment[0x76  :  0x77] = UNKNOWN             | Length:  2    (0x2) | Type: ???
	Segment[0x78]          = Gameplay Difficulty | Length:  1    (0x1) | Type: enum[uint8]       | Parent: Difficulty
	Segment[0x79  :  0x7B] = UNKNOWN             | Length:  3    (0x3) | Type: ???
	:<Inventory>
	Segment[0x7C  :  0xF3] = Hotbar: Survival    | Length: 120  (0x78) | Type: struct Item[10]   | Parent: Item
	Segment[0xF4  : 0x16B] = Hotbar: Creative    | Length: 120  (0x78) | Type: struct Item[10]   | Parent: Item
	Segment[0x16C : 0x16B] = Crafting Slots      | Length: 108  (0x6C) | Type: struct Item[9]    | Parent: Item
	Segment[0x1D8 : 0x387] = Inventory           | Length: 432 (0x1B0) | Type: struct Item[36]   | Parent: Item
	Segment[0x388 : 0x3C3] = Actual Armor Slots  | Length:  60  (0x3C) | Type: struct Item[5]    | Parent: Item
	Segment[0x3C4 : 0x3FF] = Visual Armor Slots  | Length:  60  (0x3C) | Type: struct Item[5]    | Parent: Item
	Segment[0x400 : 0x40B] = Craft Slot          | Length:  12   (0xC) | Type: struct Item       | Parent: Item
	Segment[0x40C : 0x417] = Arrow Slot          | Length:  12   (0xC) | Type: struct Item       | Parent: Item
	:<Craftbooks>
	:<Achievements>
	:<Status>
	Segment[0x548 : 0x53B] = Health              | Length:   4   (0x4) | Type: float32
	Segment[0x53C : 0x54C] = Effects             | Length:  16  (0x10) | Type: struct Effect[4]  | Parent: Effect
	--------------------------------------------------------------------------------------------------------------------

	Written By: Ryan Smith
*/
using System;
using System.Diagnostics;

namespace JJx;

public sealed class Player
{
	/* Constructors */
	public Player(string name)
	{
		this.Name = name;
	}
	internal Player(Guid uid, string name, Version version, Character character, Gameplay gameplay, Planet planets)
	{
		this.Uid = uid;
		this._Name = name;
		this.Version = version;
		this.Character = character;
		this.Gameplay = gameplay;
		this.UnlockedPlanets = planets;
	}
	/* Instance Methods */
	/* Static Methods */
	public static Player FromStream(ArchiverStream stream)
	{
		if (stream.Type != ArchiverStreamType.Player)
			throw new ArgumentException($"Passed in a non-player ({stream.Type}) archiver stream to Player.FromStream()");
		if (!stream.CanRead)
			throw new ArgumentException("Passed in a non-readable archiver stream to Player.FromStream()");
		var reader = stream._Reader ?? throw new ArgumentNullException("Invalid state of ArchiverStream::Reader");
		/// Info
		var uid = reader.Get<Guid>();
		var name = reader.GetString(length: SIZEOF_NAME);
		var version = reader.Get<Version>();
		var planet = reader.Get<Planet>();
		var flags = reader.Get<Gameplay.Flag>();
		var character = reader.Get<Character>();
		// -UNKNOWN(2)- \\
		reader.GetBytes(2);
		var difficulty = reader.Get<Difficulty>();
		// -UNKNOWN(3)- \\
		reader.GetBytes(3);
		/// Items
		/// Player
		return new Player(
			// Info
			uid, name, version, character, new Gameplay(difficulty, flags), planet
			// Items
		);
	}
	/* Properties */
	public readonly Guid Uid = Guid.NewGuid();
	private string _Name;
	public string Name {
		get { return this._Name; }
		set {
			if (String.IsNullOrEmpty(value)) throw new ArgumentException("Name cannot be null or 0 characters long");
			else if (value.Length < SIZEOF_NAME) this._Name = value;
			else this._Name = value.Substring(0, SIZEOF_NAME - 1);
		}
	}
	public readonly Version Version = Version.Latest;
	public Character Character;
	public Gameplay Gameplay;
	public Planet UnlockedPlanets = Planet.Terra;
	/* Class Properties */
	private const byte SIZEOF_NAME = 16;
}
