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

namespace JJx.Core;

public sealed class Player
{
	/* Constructor */
	internal Player(Guid id, string name, Version version, Planet unlockedPlanets, CharacterAppearance appearance, Ruleset ruleset)
	{
		this.Id = id;
		this.Version = version;
		this._Name = name;
		this.UnlockedPlanets = unlockedPlanets;
		this.Appearance = appearance;
		this.Rules = ruleset;
		this._Items = Array.Empty<Item>();
		this._Craftbook = Array.Empty<byte>();
		this._Achievements = Array.Empty<byte>();
		this._Effects = Array.Empty<Effect>();
	}
	/* Instance Methods */
	/* Properties */
	// Info
	public readonly Guid Id = Guid.NewGuid();
	public readonly Version Version = Version.Latest;
	private string _Name;
	public string Name {
		get => this._Name;
		set => this._Name = value.Length > MAX_NAME_LENGTH ? value[..MAX_NAME_LENGTH] : value;
	}
	public Planet UnlockedPlanets;
	public CharacterAppearance Appearance;
	public Ruleset Rules;
	// Items
	internal Item[] _Items;
	public Span<Item> Items => this._Items.AsSpan();
	public Span<Item> SurvivalHotbar => this._Items.AsSpan(OFFSET_SURVIVAL_HOTBAR, SIZEOF_HOTBAR);
	public Span<Item> CreativeHotbar => this._Items.AsSpan(OFFSET_CREATIVE_HOTBAR, SIZEOF_HOTBAR);
	public Span<Item> CraftingSlots  => this._Items.AsSpan(OFFSET_CRAFTING, SIZEOF_CRAFTING);
	public Span<Item> Inventory      => this._Items.AsSpan(OFFSET_INVENTORY, SIZEOF_INVENTORY);
	public Span<Item> ArmorActual    => this._Items.AsSpan(OFFSET_ARMOR_ACTIVE, SIZEOF_ARMOR);
	public Span<Item> ArmorVisual    => this._Items.AsSpan(OFFSET_ARMOR_VISUAL, SIZEOF_ARMOR);
	public ref Item CraftSlot => ref this._Items[OFFSET_CRAFT];
	public ref Item ArrowSlot => ref this._Items[OFFSET_ARROW];
	// Craftbook
	internal byte[] _Craftbook;
	public Span<byte> Craftbook => this._Craftbook.AsSpan();
	// Achievements
	internal byte[] _Achievements;
	public Span<byte> Achievements => this._Achievements.AsSpan();
	// Status
	public float Health;
	internal Effect[] _Effects;
	public Span<Effect> Effects => this._Effects.AsSpan();
	/* Class Properties */
	private const int MAX_NAME_LENGTH      = 16 - 1;
	internal const int COUNTOF_ITEMS       = 77;
	internal const int COUNTOF_EFFECTS     = 4;
	internal const int SIZEOF_CRAFTBOOK    = 256;
	internal const int SIZEOF_ACHIEVEMENTS = 32;
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
