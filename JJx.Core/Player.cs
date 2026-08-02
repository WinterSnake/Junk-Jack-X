/*
	Junk Jack X: Core
	- Player

	Segment Breakdown:
	--------------------------------------------------------------------------------------------------------------------
	:<Info>
	Segment[0x48  : 0x57]  = UUID                | Length: 16   (0x10) | Type: uuid
	Segment[0x58  : 0x67]  = Name                | Length: 16   (0x10) | Type: char*
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
	Segment[0x16C : 0x1D7] = Crafting Slots      | Length: 108  (0x6C) | Type: struct Item[9]    | Parent: Item
	Segment[0x1D8 : 0x387] = Inventory           | Length: 432 (0x1B0) | Type: struct Item[36]   | Parent: Item
	Segment[0x388 : 0x3C3] = Actual Armor Slots  | Length:  60  (0x3C) | Type: struct Item[5]    | Parent: Item
	Segment[0x3C4 : 0x3FF] = Visual Armor Slots  | Length:  60  (0x3C) | Type: struct Item[5]    | Parent: Item
	Segment[0x400 : 0x40B] = Craft Slot          | Length:  12   (0xC) | Type: struct Item       | Parent: Item
	Segment[0x40C : 0x417] = Arrow Slot          | Length:  12   (0xC) | Type: struct Item       | Parent: Item
	:<Craftbooks>
	:<Achievements>
	:<Status>
	Segment[0x538 : 0x53B] = Health              | Length:   4   (0x4) | Type: float32
	Segment[0x53C : 0x54C] = Effects             | Length:  16  (0x10) | Type: struct Effect[4]  | Parent: Effect
	--------------------------------------------------------------------------------------------------------------------

	Written By: Ryan Smith
*/

using System;
using System.Diagnostics;
using JJx.Core.Serialization;

namespace JJx.Core;

public sealed class JJxPlayer : IArchive
{
	/* Constructor */
	public JJxPlayer(string name)
	{
		this.Name = name;
		this.Model = new(false, 0, 0, HairColor.White);
		this.Rules = new(Difficulty.Peaceful, Ruleset.GameplayOptions.None);
		this._Items = new Item[SIZEOF_ITEMS];
		for (var i = 0; i < this._Items.Length; ++i)
			this._Items[i] = Item.Empty;
	}
	private JJxPlayer(
		Guid id, JJxVersion version, string name,
		Ruleset.GameplayOptions flags, CharacterModel model, Difficulty difficulty,
		Item[] items, byte[] craftbook, byte[] achievements, Effect[] effects
	)
	{
		this.Id = id;
		this.Version = version;
		this._Name = name;
		this.Rules = new(difficulty, flags);
		this.Model = model;
		this._Items = items;
		this._Craftbook = craftbook;
		this._Achievements = achievements;
		this._Effects = effects;
	}
	/* Instance Methods */
	/* Static Properties */
	public static JJxPlayer Deserialize(IArchiveReader reader)
	{
		Debug.Assert(reader.Type is ArchiveType.Player);
		// Info
		Guid id;
		string name;
		JJxVersion version;
		Planet unlockedPlanets;
		Ruleset.GameplayOptions flags;
		CharacterModel model;
		Difficulty difficulty;
		using (var memory = reader.GetChunkReader(ArchiverChunkType.PlayerInfo, out var chunkReader))
		{
			id = chunkReader.ReadObject<Guid>(JJxSerializationOptions.Default);
			name = chunkReader.ReadString(SIZEOF_NAME);
			version = chunkReader.ReadObject<JJxVersion>(JJxSerializationOptions.Default);
			unlockedPlanets = chunkReader.ReadObject<Planet>(JJxSerializationOptions.Default);
			flags = chunkReader.ReadObject<Ruleset.GameplayOptions>(JJxSerializationOptions.Default);
			model = chunkReader.ReadObject<CharacterModel>(JJxSerializationOptions.Default);
			chunkReader.Advance(2);  // Unknown: Likely padding
			difficulty = chunkReader.ReadObject<Difficulty>(JJxSerializationOptions.Default);
			chunkReader.Advance(3);  // Unknown: Likely padding
			Debug.Assert(chunkReader.Remaining == 0);
		}
		// Items
		var items = new Item[SIZEOF_ITEMS];
		using (var memory = reader.GetChunkReader(ArchiverChunkType.PlayerItems, out var chunkReader))
		{
			for (var i = 0; i < items.Length; ++i)
				items[i] = chunkReader.ReadObject<Item>(JJxSerializationOptions.Default);
			Debug.Assert(chunkReader.Remaining == 0);
		}
		// Craftbooks
		var craftbook = new byte[SIZEOF_CRAFTBOOK];
		using (var memory = reader.GetChunkReader(ArchiverChunkType.PlayerCraftbooks, out var chunkReader))
		{
			chunkReader.CopyTo(craftbook);
			Debug.Assert(chunkReader.Remaining == 0);
		}
		// Achievements
		var achievemets = new byte[SIZEOF_ACHIEVEMENTS];
		using (var memory = reader.GetChunkReader(ArchiverChunkType.PlayerAchievements, out var chunkReader))
		{
			chunkReader.CopyTo(achievemets);
			Debug.Assert(chunkReader.Remaining == 0);
		}
		// Status
		float health;
		var effects = new Effect[SIZEOF_EFFECTS];
		using (var memory = reader.GetChunkReader(ArchiverChunkType.PlayerStatus, out var chunkReader))
		{
			health = chunkReader.ReadFloat32();
			for (var i = 0; i < effects.Length; ++i)
				effects[i] = chunkReader.ReadObject<Effect>(JJxSerializationOptions.Default);
			Debug.Assert(chunkReader.Remaining == 0);
		}
		return new(id, version, name, flags, model, difficulty, items, craftbook, achievemets, effects) {
			UnlockedPlanets=unlockedPlanets,
			Health = health,
		};
	}
	public static void Serialize(JJxPlayer archive, IArchiveWriter writer)
	{

	}
	/* Properties */
	// Info
	public readonly Guid Id = Guid.NewGuid();
	public readonly JJxVersion Version = JJxVersion.Latest;
	private string _Name = null!;
	public string Name {
		get => this._Name;
		set {
			if (String.IsNullOrEmpty(value))
				throw new InvalidOperationException("Cannot set name to null/empty.");
			this._Name = value.Length > MAX_NAME_LENGTH
				? value[..MAX_NAME_LENGTH]
				: value;
		}
	}
	public Planet UnlockedPlanets = Planet.Terra;
	public Ruleset Rules;
	public CharacterModel Model;
	// Items
	private readonly Item[] _Items;
	public Span<Item> Items => this._Items;
	public Span<Item> SurvivalHotbar => this._Items.AsSpan(OFFSET_SURVIVAL_HOTBAR..SIZEOF_HOTBAR);
	public Span<Item> CreativeHotbar => this._Items.AsSpan(OFFSET_CREATIVE_HOTBAR..SIZEOF_HOTBAR);
	public Span<Item> CraftingSlots  => this._Items.AsSpan(OFFSET_CRAFTING, SIZEOF_CRAFTING);
	public Span<Item> Inventory      => this._Items.AsSpan(OFFSET_INVENTORY, SIZEOF_INVENTORY);
	public Span<Item> ArmorActual    => this._Items.AsSpan(OFFSET_ARMOR_ACTIVE, SIZEOF_ARMOR);
	public Span<Item> ArmorVisual    => this._Items.AsSpan(OFFSET_ARMOR_VISUAL, SIZEOF_ARMOR);
	public ref Item CraftSlot        => ref this._Items[OFFSET_CRAFT];
	public ref Item ArrowSlot        => ref this._Items[OFFSET_ARROW];
	// Craftbook
	private readonly byte[] _Craftbook = new byte[SIZEOF_CRAFTBOOK];
	public Span<byte> Craftbook => this._Craftbook;
	// Achievements
	private readonly byte[] _Achievements = new byte[SIZEOF_ACHIEVEMENTS];
	public Span<byte> Achievements => this._Achievements;
	// Status
	public float Health = 5.0f;
	private readonly Effect[] _Effects = new Effect[SIZEOF_EFFECTS];
	public Span<Effect> Effects => this._Effects;
	/* Class Properties */
	private const int SIZEOF_NAME         = 16;
	private const int MAX_NAME_LENGTH     = SIZEOF_NAME - 1;
	private const int SIZEOF_ITEMS        = 77;
	private const int SIZEOF_CRAFTBOOK    = 256;
	private const int SIZEOF_ACHIEVEMENTS = 32;
	private const int SIZEOF_EFFECTS      = 4;
	// Items
	private const byte OFFSET_SURVIVAL_HOTBAR =  0;
	private const byte OFFSET_CREATIVE_HOTBAR = 10;
	private const byte SIZEOF_HOTBAR          = 10;
	private const byte OFFSET_CRAFTING        = 20;
	private const byte SIZEOF_CRAFTING        =  9;
	private const byte OFFSET_INVENTORY       = 29;
	private const byte SIZEOF_INVENTORY       = 36;
	private const byte OFFSET_ARMOR_ACTIVE    = 65;
	private const byte OFFSET_ARMOR_VISUAL    = 70;
	private const byte SIZEOF_ARMOR           =  5;  // Order: Helm, Chestpiece, Leggings, Feet, Pet
	private const byte OFFSET_CRAFT           = 75;
	private const byte OFFSET_ARROW           = 76;
}
