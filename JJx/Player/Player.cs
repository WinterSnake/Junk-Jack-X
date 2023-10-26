/*
	Junk Jack X: Player

	Segment Breakdown:
	-----------------------------------------------------------------------------------------------------------------------------
	:<Info>
	Segment[0x48  : 0x57]  = UUID                   | Length: 16  (0x10)  | Type: uuid
	Segment[0x58  : 0x67]  = Name                   | Length: 16  (0x10)  | Type: char*
	Segment[0x68  : 0x6B]  = Game Version           | Length:  4   (0x4)  | Type: enum[uint32]      | Parent: JJx.Version
	Segment[0x6C  : 0x6F]  = Theme\Unlocked Planets | Length:  4   (0x4)  | Type: enum flag[uint32] | Parent: World.Planet
	Segment[0x70  : 0x73]  = Gameplay Flags         | Length:  4   (0x4)  | Type: enum flag[uint32] | Parent: Gameplay.Flags
	Segment[0x74]          = Hair Color             | Length:  1   (0x1)  | Type: bitfield          | Parent: Character
	Segment[0x75]          = Gender/Skin/Hair       | Length:  1   (0x1)  | Type: bitfield          | Parent: Character
	Segment[0x76  :  0x77] = UNKNOWN                | Length:  2   (0x2)  | Type: ???
	Segment[0x78]          = Gameplay Difficulty    | Length:  1   (0x1)  | Type: enum[uint8]       | Parent: Gameplay.Difficulty
	Segment[0x79  :  0x7B] = UNKNOWN                | Length:  3   (0x3)  | Type: ???
	:<Inventory>
	Segment[0x7C  :  0xF3] = Hotbar: Survival       | Length: 120  (0x78) | Type: struct Item[10]   | Parent: Items
	Segment[0xF4  : 0x16B] = Hotbar: Creative       | Length: 120  (0x78) | Type: struct Item[10]   | Parent: Items
	Segment[0x16C : 0x16B] = Crafting Slots         | Length: 108  (0x6C) | Type: struct Item[9]    | Parent: Items
	Segment[0x1D8 : 0x387] = Inventory              | Length: 432 (0x1B0) | Type: struct Item[36]   | Parent: Items
	Segment[0x388 : 0x3C3] = Actual Armor Slots     | Length:  60  (0x3C) | Type: struct Item[5]    | Parent: Items
	Segment[0x3C4 : 0x3FF] = Visual Armor Slots     | Length:  60  (0x3C) | Type: struct Item[5]    | Parent: Items
	Segment[0x400 : 0x40B] = Craft Slot             | Length:  12   (0xC) | Type: struct Item       | Parent: Items
	Segment[0x40C : 0x417] = Arrow Slot             | Length:  12   (0xC) | Type: struct Item       | Parent: Items
	:<Status>
	Segment[0x548 : 0x53B] = Health                 | Length:   4   (0x4) | Type: float32
	Segment[0x53C : 0x54C] = Effects                | Length:  16  (0x10) | Type: struct Effect[4]
	-----------------------------------------------------------------------------------------------------------------------------

	Written By: Ryan Smith
*/
using System;
using System.Threading.Tasks;

namespace JJx;

public sealed class Player
{
	/* Constructors */
	public Player(
		string name, Planet unlockedPlanets = Planet.Terra, Difficulty difficulty = Difficulty.Normal,
		Gameplay.Flag flags = Gameplay.Flag.None
	)
	{
		// Info
		this.Id = Guid.NewGuid();
		this.Name = name;
		this.Version = Version.Latest;
		this.UnlockedPlanets = unlockedPlanets;
		this.Gameplay = new Gameplay(difficulty, flags);
		// Inventory
		this.Items = new Item[COUNTOF_ITEMS];
		for (var i = 0; i < this.Items.Length; ++i)
			this.Items[i] = new Item(0xFFFF, 0);
		// Status
		this.Effects = new Effect[COUNTOF_EFFECTS];
	}
	private Player(
		Guid id, string name, Version version, Planet unlockedPlanets, Character character, Gameplay gameplay,
		Item[] items, float health, Effect[] effects
	)
	{
		this.Id = id;
		this._Name = name;
		this.Version = version;
		this.UnlockedPlanets = unlockedPlanets;
		this.Character = character;
		this.Gameplay = gameplay;
		this.Items = items;
		this.Health = health;
		this.Effects = effects;
	}
	/* Instance Methods */
	public async Task Save(string filePath)
	{
		var workingData = new byte[SIZEOF_BUFFER];
		using var stream = await ArchiverStream.Writer(filePath, ArchiverType.Player);
		/// Info
		var playerInfo = stream.StartChunk(ChunkType.PlayerInfo);
			// Uuid
			var uuid = this.Id.ToByteArray();
			await playerInfo.WriteAsync(uuid, 0, uuid.Length);
			// Name
			BitConverter.Write(workingData, this._Name, length: SIZEOF_NAME);
			await playerInfo.WriteAsync(workingData, 0, SIZEOF_NAME);
			// Version
			BitConverter.Write(workingData, (uint)this.Version, 0);
			// Themes/Unlocked Worlds
			BitConverter.Write(workingData, (uint)this.UnlockedPlanets, 4);
			// Gameplay: Flags
			BitConverter.Write(workingData, (uint)this.Gameplay.Flags,  8);
			// Character
			this.Character.Pack(workingData, 12);
			// =UNKNOWN= :size(2):
			BitConverter.Write(workingData, (ushort)0, 14);
			// Gameplay: Difficulty
			BitConverter.Write(workingData, (byte)this.Gameplay.Difficulty, 16);
			// =UNKNOWN= :size(3):
			await playerInfo.WriteAsync(workingData, 0, workingData.Length);
		stream.EndChunk();
		/// Inventory
		var playerInventory = stream.StartChunk(ChunkType.PlayerInventory);
			foreach (var item in this.Items)
				await item.ToStream(playerInventory);
		stream.EndChunk();
		/// Craftbook
		var playerCraftbook = stream.StartChunk(ChunkType.PlayerCraftbook);
			workingData = new byte[0x100];
			await playerCraftbook.WriteAsync(workingData, 0, workingData.Length);
		stream.EndChunk();
		/// Achievements
		var playerAchievements = stream.StartChunk(ChunkType.PlayerAchievements);
			workingData = new byte[0x20];
			await playerAchievements.WriteAsync(workingData, 0, workingData.Length);
		stream.EndChunk();
		/// Status
		var playerStatus = stream.StartChunk(ChunkType.PlayerStatus);
			workingData = new byte[SIZEOF_HEALTH];
			BitConverter.Write(workingData, this.Health / 10.0f, 0);
			await playerStatus.WriteAsync(workingData, 0, workingData.Length);
			foreach (var effect in this.Effects)
				await effect.ToStream(playerStatus);
		stream.EndChunk();
	}
	/* Static Methods */
	public static async Task<Player> Load(string filePath) => await Player.FromStream(await ArchiverStream.Reader(filePath));
	public static async Task<Player> FromStream(ArchiverStream stream)
	{
		if (stream.Type != ArchiverType.Player && stream.CanRead)
			throw new ArgumentException($"Expected player stream, found {stream.Type} stream");
		int bytesRead = 0;
		var workingData = new byte[SIZEOF_BUFFER];
		/// Info
		if (!stream.AtChunk(ChunkType.PlayerInfo))
			stream.JumpToChunk(ChunkType.PlayerInfo);
		// UUID
		while (bytesRead < SIZEOF_UUID)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_UUID - bytesRead);
		var id = new Guid(new Span<byte>(workingData, 0, SIZEOF_UUID));
		// Name
		bytesRead = 0;
		while (bytesRead < SIZEOF_NAME)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_NAME - bytesRead);
		var name = BitConverter.GetString(workingData);
		// {Version | Unlocked Planets | Gameplay Flags | Character | Unknown | Difficulty | Unknown}
		bytesRead = 0;
		while (bytesRead < workingData.Length)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, workingData.Length - bytesRead);
		// -Version
		var version = (Version)BitConverter.GetUInt32(workingData, 0);
		// -Unlocked Planets
		var unlockedPlanets = (Planet)BitConverter.GetUInt32(workingData, 4);
		// -Gameplay Flags
		var flags = (Gameplay.Flag)BitConverter.GetUInt32(workingData, 8);
		// -Character
		var character = Character.Unpack(workingData, 12);
		// =UNKNOWN= :size(2):
		// -Difficulty
		var difficulty = (Difficulty)BitConverter.GetUInt8(workingData, 16);
		// =UNKNOWN= :size(3):
		/// Inventory
		if (!stream.AtChunk(ChunkType.PlayerInventory))
			stream.JumpToChunk(ChunkType.PlayerInventory);
		var items = new Item[COUNTOF_ITEMS];
		for (var i = 0; i < items.Length; ++i)
			items[i] = await Item.FromStream(stream);
		/// Craftbook
		if (!stream.AtChunk(ChunkType.PlayerCraftbook))
			stream.JumpToChunk(ChunkType.PlayerCraftbook);
		stream.Position += 0x100;
		/// Achievements
		if (!stream.AtChunk(ChunkType.PlayerAchievements))
			stream.JumpToChunk(ChunkType.PlayerAchievements);
		stream.Position += 0x20;
		/// Status
		if (!stream.AtChunk(ChunkType.PlayerStatus))
			stream.JumpToChunk(ChunkType.PlayerStatus);
		bytesRead = 0;
		while (bytesRead < SIZEOF_HEALTH)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_HEALTH - bytesRead);
		float health = BitConverter.GetFloat32(workingData) * 10.0f;
		var effects = new Effect[COUNTOF_EFFECTS];
		for (var i = 0; i < effects.Length; ++i)
			effects[i] = await Effect.FromStream(stream);
		/// Player
		return new Player(
			id, name, version, unlockedPlanets, character, new Gameplay(difficulty, flags), items, health, effects
		);
	}
	/* Properties */
	// Info
	public readonly Guid Id;
	private string _Name;
	public string Name {
		get { return this._Name; }
		set {
			if (String.IsNullOrEmpty(value)) return;
			else if (value.Length < SIZEOF_NAME) this._Name = value;
			else this._Name = value.Substring(0, SIZEOF_NAME - 1);
		}
	}
	public readonly Version Version;
	public Planet UnlockedPlanets;
	public Character Character;
	public Gameplay Gameplay;
	// Inventory
	public readonly Item[] Items;
	public ArraySegment<Item> SurvivalHotbar { get { return new ArraySegment<Item>(this.Items,  0, 10); }}  // 10
	public ArraySegment<Item> CreativeHotbar { get { return new ArraySegment<Item>(this.Items, 10, 10); }}  // 20
	public ArraySegment<Item> CraftingSlots  { get { return new ArraySegment<Item>(this.Items, 20,  9); }}  // 29
	public ArraySegment<Item> Inventory      { get { return new ArraySegment<Item>(this.Items, 29, 36); }}  // 65
	// -Order: Helm, Chestpiece, Leggings, Feet, Pet
	public ArraySegment<Item> ArmorActual    { get { return new ArraySegment<Item>(this.Items, 65,  5); }}  // 70
	public ArraySegment<Item> ArmorVisual    { get { return new ArraySegment<Item>(this.Items, 70,  5); }}  // 75
	public Item CraftSlot                    { get { return Items[75]; }}                                   // 76
	public Item ArrowSlot                    { get { return Items[76]; }}                                   // 77
	// Craftbook
	// Achievements
	// Status
	public float Health = 50.0f;
	public readonly Effect[] Effects;
	/* Class Properties */
	private const byte SIZEOF_BUFFER   = 20;
	private const byte SIZEOF_UUID     = 16;
	private const byte SIZEOF_NAME     = 16;
	private const byte SIZEOF_HEALTH   =  4;
	private const byte COUNTOF_ITEMS   = 77;
	private const byte COUNTOF_EFFECTS =  4;
}
