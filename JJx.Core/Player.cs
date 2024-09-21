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
using JJx.Serialization;

namespace JJx;

public sealed class Player
{
	/* Constructors */
	#nullable enable
	public Player(
		string name, Difficulty difficulty = Difficulty.Normal,
		GameOptions.Flag flags = GameOptions.Flag.None, System.Random? rng = null
	)
	{
		// Info
		this.Name = name;
		this.Options = new GameOptions(difficulty, flags);
		this.Character = Character.Random(rng);
		// Items
		for (var i = 0; i < this.Items.Length; ++i)
			this.Items[i] = new Item(0xFFFF, 0);
		// Status
		for (var i = 0; i < this.Effects.Length; ++i)
			this.Effects[i] = new Effect(0x00, 0);
	}
	#nullable disable
	internal Player(
		Guid uid, string name, Version version, Character character,
		GameOptions options, Planet planets, Item[] items,
		float health, Effect[] effects
	)
	{
		// Info
		this.Uid = uid;
		this._Name = name;
		this.Version = version;
		this.Character = character;
		this.Options = options;
		this.UnlockedPlanets = planets;
		// Items
		this.Items = items;
		// Effects
		this.Health = health;
		this.Effects = effects;
	}
	/* Instance Methods */
	public void Save(string filePath)
	{
		using var stream = ArchiverStream.Writer(filePath, ArchiverStreamType.Player);
		this.ToStream(stream);
	}
	public void ToStream(ArchiverStream stream)
	{
		if (stream.Type != ArchiverStreamType.Player)
			throw new ArgumentException($"Passed in a non-player ({stream.Type}) archiver stream to Player.ToStream()");
		if (!stream.CanWrite)
			throw new ArgumentException("Passed in a non-writable archiver stream to Player.ToStream()");
	}
	/* Static Methods */
	public static Player Load(string filePath)
	{
		using var stream = ArchiverStream.Reader(filePath);
		return Player.FromStream(stream);
	}
	public static Player FromStream(ArchiverStream stream)
	{
		if (stream.Type != ArchiverStreamType.Player)
			throw new ArgumentException($"Passed in a non-player ({stream.Type}) archiver stream to Player.FromStream()");
		if (!stream.CanRead)
			throw new ArgumentException("Passed in a non-readable archiver stream to Player.FromStream()");
		ArchiverChunk chunk;
		var reader = new JJxReader(stream);
		/// Info
		chunk = stream.GetChunk(ArchiverChunkType.PlayerInfo);
		Debug.Assert(stream.Position == chunk.Position, $"ArchiverStream::Reader not aligned with PlayerInfoChunk || Current: {stream.Position:X8} ; Expected: {chunk.Position:X8}");
		if (stream.Position != chunk.Position)
			stream.Position  = chunk.Position;
		var uid = reader.Get<Guid>();
		var name = reader.GetString(length: SIZEOF_NAME);
		var version = reader.Get<Version>();
		var planet = reader.Get<Planet>();
		var flags = reader.Get<GameOptions.Flag>();
		var character = reader.Get<Character>();
		// -UNKNOWN(2)- \\
		reader.GetBytes(2);
		var difficulty = reader.Get<Difficulty>();
		// -UNKNOWN(3)- \\
		reader.GetBytes(3);
		/// Items
		chunk = stream.GetChunk(ArchiverChunkType.PlayerItems);
		Debug.Assert(stream.Position == chunk.Position, $"ArchiverStream::Reader not aligned with PlayerItemsChunk || Current: {stream.Position:X8} ; Expected: {chunk.Position:X8}");
		if (stream.Position != chunk.Position)
			stream.Position  = chunk.Position;
		var items = new Item[SIZEOF_ITEMS];
		for (var i = 0; i < items.Length; ++i)
			items[i] = reader.Get<Item>();
		/// Craftbooks :: UNKNOWN
		chunk = stream.GetChunk(ArchiverChunkType.PlayerCraftbooks);
		Debug.Assert(stream.Position == chunk.Position, $"ArchiverStream::Reader not aligned with PlayerCraftbooksChunk || Current: {stream.Position:X8} ; Expected: {chunk.Position:X8}");
		if (stream.Position != chunk.Position)
			stream.Position  = chunk.Position;
		var craftbooksData = reader.GetBytes((int)chunk.Length);
		/// Achievements :: UNKNOWN
		chunk = stream.GetChunk(ArchiverChunkType.PlayerAchievements);
		Debug.Assert(stream.Position == chunk.Position, $"ArchiverStream::Reader not aligned with PlayerAchievementsChunk || Current: {stream.Position:X8} ; Expected: {chunk.Position:X8}");
		if (stream.Position != chunk.Position)
			stream.Position  = chunk.Position;
		var achievementsData = reader.GetBytes((int)chunk.Length);
		/// Status
		chunk = stream.GetChunk(ArchiverChunkType.PlayerStatus);
		Debug.Assert(stream.Position == chunk.Position, $"ArchiverStream::Reader not aligned with PlayerStatusChunk || Current: {stream.Position:X8} ; Expected: {chunk.Position:X8}");
		if (stream.Position != chunk.Position)
			stream.Position  = chunk.Position;
		var health = reader.GetFloat32() * 10.0f;
		var effects = new Effect[SIZEOF_EFFECTS];
		for (var i = 0; i < effects.Length; ++i)
			effects[i] = reader.Get<Effect>();
		Debug.Assert(stream.Position == stream.Length, $"ArchiverStream::Reader not at end of stream || Current: {stream.Position:X8} ; Expected: {stream.Length:X8}");
		/// Player
		return new Player(
			uid, name, version, character, new GameOptions(difficulty, flags), planet, items, health, effects
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
	public GameOptions Options;
	public Planet UnlockedPlanets = Planet.Terra;
	public readonly Item[] Items = new Item[SIZEOF_ITEMS];  // TODO: Custom collection
	public float Health = 50.0f;
	public readonly Effect[] Effects = new Effect[SIZEOF_EFFECTS];
	/* Class Properties */
	private const byte SIZEOF_NAME    = 16;
	private const byte SIZEOF_ITEMS   = 77;
	private const byte SIZEOF_EFFECTS =  4;
}
