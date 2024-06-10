/*
	Junk Jack X: Core
	- Player

	Segment Breakdown:
	-----------------------------------------------------------------------------------------------------------------------------
	:<Info>
	Segment[0x48  : 0x57]  = UUID                   | Length: 16  (0x10)  | Type: uuid
	Segment[0x58  : 0x67]  = Name                   | Length: 16  (0x10)  | Type: char*
	Segment[0x68  : 0x6B]  = Game Version           | Length:  4   (0x4)  | Type: enum[uint32]      | Parent: Version
	Segment[0x6C  : 0x6F]  = Theme\Unlocked Planets | Length:  4   (0x4)  | Type: enum flag[uint32] | Parent: Planet
	Segment[0x70  : 0x73]  = Gameplay Flags         | Length:  4   (0x4)  | Type: enum flag[uint32] | Parent: Gameplay.Flag
	Segment[0x74]          = Hair Color             | Length:  1   (0x1)  | Type: bitfield          | Parent: Character
	Segment[0x75]          = Gender/Skin/Hair       | Length:  1   (0x1)  | Type: bitfield          | Parent: Character
	Segment[0x76  :  0x77] = UNKNOWN                | Length:  2   (0x2)  | Type: ???
	Segment[0x78]          = Gameplay Difficulty    | Length:  1   (0x1)  | Type: enum[uint8]       | Parent: Difficulty
	Segment[0x79  :  0x7B] = UNKNOWN                | Length:  3   (0x3)  | Type: ???
	:<Inventory>
	Segment[0x7C  :  0xF3] = Hotbar: Survival       | Length: 120  (0x78) | Type: struct Item[10]   | Parent: Item
	Segment[0xF4  : 0x16B] = Hotbar: Creative       | Length: 120  (0x78) | Type: struct Item[10]   | Parent: Item
	Segment[0x16C : 0x16B] = Crafting Slots         | Length: 108  (0x6C) | Type: struct Item[9]    | Parent: Item
	Segment[0x1D8 : 0x387] = Inventory              | Length: 432 (0x1B0) | Type: struct Item[36]   | Parent: Item
	Segment[0x388 : 0x3C3] = Actual Armor Slots     | Length:  60  (0x3C) | Type: struct Item[5]    | Parent: Item
	Segment[0x3C4 : 0x3FF] = Visual Armor Slots     | Length:  60  (0x3C) | Type: struct Item[5]    | Parent: Item
	Segment[0x400 : 0x40B] = Craft Slot             | Length:  12   (0xC) | Type: struct Item       | Parent: Item
	Segment[0x40C : 0x417] = Arrow Slot             | Length:  12   (0xC) | Type: struct Item       | Parent: Item
	:<Craftbooks>
	:<Achievements>
	:<Status>
	Segment[0x548 : 0x53B] = Health                 | Length:   4   (0x4) | Type: float32
	Segment[0x53C : 0x54C] = Effects                | Length:  16  (0x10) | Type: struct Effect[4]  | Parent: Effect
	-----------------------------------------------------------------------------------------------------------------------------

	Written By: Ryan Smith
*/
using System;
using System.Threading.Tasks;

namespace JJx;

public sealed class Player
{
	/* Constructor */
	public Player(string name, Gameplay.Flag flags = Gameplay.Flag.None)
	{
		// Info
		this.Id = Guid.NewGuid();
		this.Name = name;
		this.UnlockedPlanets = Planet.Terra;
		this.Character = new Character(
			Random.Shared.NextDouble() >= 0.5, // Gender
			(byte)Random.Shared.Next(Character.MAX_SKINTONES + 1), // Skin
			(byte)Random.Shared.Next(Character.MAX_HAIRSTYLES + 1), // Hair: Style
			(HairColor)Random.Shared.Next(Enum.GetValues(typeof(HairColor)).Length) // Hair: Color
		);
		this.Gameplay = new Gameplay(Difficulty.Normal, flags);
		// Inventory
		for (var i = 0; i < this.Inventory.Length; ++i)
			this.Inventory[i] = new Item(0xFFFF, 0x0000);
		// Status
	}
	private Player(Guid id, string name, Version version, Planet planet, Character character, Gameplay gameplay, Item[] inventory, float health, Effect[] effects)
	{
		this.Id = id;
		this._Name = name;
		this.Version = version;
		this.UnlockedPlanets = planet;
		this.Character = character;
		this.Gameplay = gameplay;
		this.Inventory = inventory;
		this.Health = health;
		this.Effects = effects;
	}
	/* Instance Methods */
	public override string ToString()
		=> $"{this.Name}({this.Character}): Unlocked Planets={this.UnlockedPlanets}, Gameplay=[{this.Gameplay}], Health: {this.Health}";
	/* Static Methods */
	public static async Task<Player> FromStream(ArchiverStream stream)
	{
		if (stream.Type != ArchiverStreamType.Player || !stream.CanRead)
			throw new ArgumentException($"Expected readable player stream, either incorrect stream type (Type:{stream.Type}) or not readable (Readable:{stream.CanRead})");
		var bytesRead = 0;
		var buffer = new byte[SIZEOF_BUFFER];
		/// Info
		if (!stream.IsAtChunk(ArchiverChunkType.PlayerInfo))
			stream.JumpToChunk(ArchiverChunkType.PlayerInfo);
		while (bytesRead < buffer.Length)
			bytesRead += await stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
		// UUID
		var id = new Guid(new Span<byte>(buffer, Player.OFFSET_UUID, Player.SIZEOF_UUID));
		// Name
		var name = JJx.BitConverter.GetString(buffer, Player.OFFSET_NAME);
		// {Version | Unlocked Planets | Gameplay Flags | Character | Unknown | Difficulty | Unknown}
		bytesRead = 0;
		while (bytesRead < Player.SIZEOF_INFO)
			bytesRead += await stream.ReadAsync(buffer, bytesRead, Player.SIZEOF_INFO - bytesRead);
		var version    = (Version)JJx.BitConverter.LittleEndian.GetUInt32(buffer, Player.OFFSET_VERSION);
		var planet     = (Planet)JJx.BitConverter.LittleEndian.GetUInt32(buffer, Player.OFFSET_PLANET);
		var flags      = (Gameplay.Flag)JJx.BitConverter.LittleEndian.GetUInt32(buffer, Player.OFFSET_FLAGS);
		var character  = Character.Unpack(buffer, Player.OFFSET_CHARACTER);
		var difficulty = (Difficulty)buffer[Player.OFFSET_DIFFICULTY];
		/// Inventory
		if (!stream.IsAtChunk(ArchiverChunkType.PlayerInventory))
			stream.JumpToChunk(ArchiverChunkType.PlayerInventory);
		var inventory = new Item[Player.COUNTOF_INVENTORY];
		for (var i = 0; i < inventory.Length; ++i)
			inventory[i] = await Item.FromStream(stream);
		/// Craftbook
		if (!stream.IsAtChunk(ArchiverChunkType.PlayerCraftbooks))
			stream.JumpToChunk(ArchiverChunkType.PlayerCraftbooks);
		/// Achievements
		if (!stream.IsAtChunk(ArchiverChunkType.PlayerAchievements))
			stream.JumpToChunk(ArchiverChunkType.PlayerAchievements);
		/// Status
		if (!stream.IsAtChunk(ArchiverChunkType.PlayerStatus))
			stream.JumpToChunk(ArchiverChunkType.PlayerStatus);
		bytesRead = 0;
		while (bytesRead < sizeof(float))
			bytesRead += await stream.ReadAsync(buffer, bytesRead, sizeof(float) - bytesRead);
		var health = JJx.BitConverter.LittleEndian.GetFloat(buffer) * 10.0f;
		var effects = new Effect[Player.COUNTOF_EFFECTS];
		for (var i = 0; i < effects.Length; ++i)
			effects[i] = await Effect.FromStream(stream);
		return new Player(id, name, version, planet, character, new Gameplay(difficulty, flags), inventory, health, effects);
	}
	/* Properties */
	// Info
	public readonly Guid Id;
	private string _Name;
	public string Name {
		get { return this._Name; }
		set {
			if (String.IsNullOrEmpty(value)) return;
			else if (value.Length < Player.SIZEOF_NAME) this._Name = value;
			else this._Name = value.Substring(0, SIZEOF_NAME - 1);
		}
	}
	public readonly Version Version = Version.Latest;
	public Planet UnlockedPlanets;
	public Character Character;
	public Gameplay Gameplay;
	// Inventory
	public readonly Item[] Inventory = new Item[Player.COUNTOF_INVENTORY];
	// Craftbook
	// Achievements
	// Status
	public float Health = 50.0f;
	public readonly Effect[] Effects = new Effect[Player.COUNTOF_EFFECTS];
	/* Class Properties */
	private const byte OFFSET_UUID       =  0;
	private const byte OFFSET_NAME       = 16;
	private const byte OFFSET_VERSION    =  0;
	private const byte OFFSET_PLANET     =  4;
	private const byte OFFSET_FLAGS      =  8;
	private const byte OFFSET_CHARACTER  = 12;
	private const byte OFFSET_DIFFICULTY = 16;
	private const byte OFFSET_HEALTH     =  0;
	private const byte SIZEOF_BUFFER     = 32;
	private const byte SIZEOF_UUID       = 16;
	private const byte SIZEOF_NAME       = 16;
	private const byte SIZEOF_INFO       = 18 + Character.SIZE; // Version(4), Planets(4), Flags(4), Character(2), UNKNOWN(2), Difficulty(1), UNKNOWN(3)
	private const byte COUNTOF_INVENTORY = 77;
	private const byte COUNTOF_EFFECTS   =  4;
}
