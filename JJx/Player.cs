/*
	Junk Jack X: Player

	Segment Breakdown:
	------------------------------------------------------------------------------------------------------------------------
	:<Info>
	Segment[0x48  : 0x57]  = UUID                   | Length: 16  (0x10)  | Type: uuid
	Segment[0x58  : 0x67]  = Name                   | Length: 16  (0x10)  | Type: char*
	Segment[0x68  : 0x6B]  = Game Version           | Length: 4   (0x4)   | Type: enum[uint32]      | Parent: JJx.Version
	Segment[0x6C  : 0x6F]  = Theme\Unlocked Planets | Length: 4   (0x4)   | Type: enum flag[uint32] | Parent: World.Planet
	Segment[0x70  : 0x73]  = Gameplay Flags         | Length: 4   (0x4)   | Type: enum flag[uint32] | Parent: Gameplay.Flags
	Segment[0x74]          = Hair Color             | Length: 1   (0x1)   | Type: bitfield          | Parent: Character
	Segment[0x75]          = Gender/Skin/Hair       | Length: 1   (0x1)   | Type: bitfield          | Parent: Character
	Segment[0x76  :  0x77] = UNKNOWN                | Length: 2   (0x2)   | Type: ???
	Segment[0x78]          = Gameplay Difficulty    | Length: 1   (0x1)   | Type: enum[uint8]       | Parent: Gameplay.Difficulty
	Segment[0x79  :  0x7B] = UNKNOWN                | Length: 3   (0x3)   | Type: ???
	:<Inventory>
	Segment[0x7C  :  0xF3] = Hotbar: Survival       | Length: 120 (0x78)  | Type: struct Item[10] | Parent: Items
	Segment[0xF4  : 0x16B] = Hotbar: Creative       | Length: 120 (0x78)  | Type: struct Item[10] | Parent: Items
	Segment[0x16C : 0x16B] = Crafting Slots         | Length: 108 (0x6C)  | Type: struct Item[9]  | Parent: Items
	Segment[0x1D8 : 0x387] = Inventory              | Length: 432 (0x1B0) | Type: struct Item[36] | Parent: Items
	Segment[0x388 : 0x3C3] = Actual Armor Slots     | Length: 60  (0x3C)  | Type: struct Item[5]  | Parent: Items
	Segment[0x3C4 : 0x3FF] = Visual Armor Slots     | Length: 60  (0x3C)  | Type: struct Item[5]  | Parent: Items
	Segment[0x400 : 0x40B] = Craft Slot             | Length: 12  (0xC)   | Type: struct Item     | Parent: Items
	Segment[0x40C : 0x417] = Arrow Slot             | Length: 12  (0xC)   | Type: struct Item     | Parent: Items
	:<Craftbook>
	Segment[0x418 : 0x443] = Items                  | Length: 44  (0x2C)  | Type: struct Recipes  | Parent: ItemBook
	Segment[0x444 : 0x497] = UNKNOWN \ UNUSED       | Length: 84  (0x54)  | Type: ???
	Segment[0x498 : 0x517] = Potions                | Length: 128 (0x80)  | Type: struct Recipes  | Parent: PotionBook
	:<Achievements>
	:<Status>
	------------------------------------------------------------------------------------------------------------------------

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Player
{
	/* Constructors */
	public Player(string name, Gameplay.Flag flags = Gameplay.Flag.None)
	{
		// Default player info
		this.Id = Guid.NewGuid();
		this.Name = name;
		this.Version = Version.Latest;
		this.UnlockedPlanets = Planet.Terra;
		// Random character design
		var rnd = new Random();
		this.Character = new Character(
			rnd.NextDouble() >= 0.5 ,
			(byte)rnd.Next(Character.MAX_SKINTONES + 1),
			(byte)rnd.Next(Character.MAX_HAIRSTYLES + 1),
			(HairColor)(rnd.Next(Enum.GetValues(typeof(HairColor)).Length))
		);
		// Default gameplay
		this.Gameplay = new Gameplay(Difficulty.Normal, flags);
		// Set items to none
		for (var i = 0; i < this.Items.Length; ++i)
			this.Items[i] = new Item(0xFFFF, 0);
	}
	private Player(Guid id, string name, Version version, Planet unlockedPlanets, Character character, Gameplay gameplay, Item[] items)
	{
		this.Id = id;
		this._Name = name;
		this.Version = version;
		this.UnlockedPlanets = unlockedPlanets;
		this.Character = character;
		this.Gameplay = gameplay;
		this.Items = items;
	}
	/* Instance Methods */
	public async Task Save(string path)
	{
		using var stream = await ArchiverStream.Writer(path, ArchiverType.Player);
		var workingData = new byte[BUFFER_SIZE];
		/// Info
		using (var info = stream.NewChunk(Chunk.Type.PlayerInfo))
		{
			// Uuid
			var uuid = this.Id.ToByteArray();
			for (var i = uuid.Length - 1; i <= 0; --i)
				workingData[uuid.Length - i] = uuid[i];
			await info.WriteAsync(uuid, 0, uuid.Length);
			// Name
			Utilities.ByteConverter.Write(new Span<byte>(workingData), this._Name, length: SIZEOF_NAME);
			await info.WriteAsync(workingData, 0, workingData.Length);
			// Game Version
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (uint)this.Version, 0);
			// Themes/Unlocked Worlds
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (uint)this.UnlockedPlanets, 4);
			// Gameplay: Flags
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (uint)this.Gameplay.Flags,  8);
			// Character
			this.Character.Pack(new Span<byte>(workingData), 12);
			// =UNKNOWN=
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (ushort)0, 14);
			await info.WriteAsync(workingData, 0, workingData.Length);
			// Gameplay: Difficulty
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (byte)this.Gameplay.Difficulty, 0);
			// =UNKNOWN=
			Utilities.ByteConverter.Write(new Span<byte>(workingData), (uint)0, 1);
			await info.WriteAsync(workingData, 0, 4);
		}
		/// Inventory
		using (var inventory = stream.NewChunk(Chunk.Type.PlayerInventory))
		{
			foreach (var item in this.Items)
				await item.ToStream(inventory);
		}
		/// Craftbook
		using (var craftbook = stream.NewChunk(Chunk.Type.PlayerCraftbook))
		{
			workingData = new byte[0x100];
			await craftbook.WriteAsync(workingData, 0, workingData.Length);
		}
		/// Achievements
		using (var craftbook = stream.NewChunk(Chunk.Type.PlayerAchievements, version: 1))
		{
			workingData = new byte[0x20];
			await craftbook.WriteAsync(workingData, 0, workingData.Length);
		}
		/// Status
		using (var craftbook = stream.NewChunk(Chunk.Type.PlayerStatus))
		{
			workingData = new byte[0x14];
			await craftbook.WriteAsync(workingData, 0, workingData.Length);
		}
	}
	/* Static Methods */
	public static async Task<Player> Load(string path)
	{
		using var stream = await ArchiverStream.Reader(path);
		if (stream.Type != ArchiverType.Player)
			throw new ArgumentException($"Expected player stream, found {stream.Type}");
		// DEBUG
		#if DEBUG
			Console.WriteLine(String.Join("\n", stream.GetChunkStrings()));
		#endif
		int bytesRead = 0;
		var workingData = new byte[BUFFER_SIZE];
		/// Info
		// Uuid
		while (bytesRead < SIZEOF_UUID)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_UUID - bytesRead);
		var id = new Guid(new Span<byte>(workingData));
		// Name
		bytesRead = 0;
		while (bytesRead < SIZEOF_NAME)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_NAME - bytesRead);
		var name = Utilities.ByteConverter.GetString(new Span<byte>(workingData));
		// Game Version
		bytesRead = 0;
		while (bytesRead < SIZEOF_VERSION)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_VERSION - bytesRead);
		var version = (Version)Utilities.ByteConverter.GetUInt32(new Span<byte>(workingData));
		// Themes/Unlocked Worlds
		bytesRead = 0;
		while (bytesRead < SIZEOF_THEME)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_THEME - bytesRead);
		var unlockedPlanets = (Planet)Utilities.ByteConverter.GetUInt32(new Span<byte>(workingData));
		// Gameplay: Flags
		bytesRead = 0;
		while (bytesRead < SIZEOF_GAMEPLAY_FLAGS)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_GAMEPLAY_FLAGS - bytesRead);
		var flags = (Gameplay.Flag)Utilities.ByteConverter.GetUInt32(new Span<byte>(workingData));
		// Character
		bytesRead = 0;
		while (bytesRead < Character.SIZE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, Character.SIZE - bytesRead);
		var character = Character.Unpack(new Span<byte>(workingData));
		// =UNKNOWN=
		stream.Seek(2, SeekOrigin.Current);
		// Gameplay: Difficulty
		var difficulty = (Difficulty)stream.ReadByte();
		var gameplay = new Gameplay(difficulty, flags);
		// =UNKNOWN=
		stream.Seek(3, SeekOrigin.Current);
		/// Inventory
		var items = new Item[COUNT_ITEMS];
		for (var i = 0; i < COUNT_ITEMS; ++i)
			items[i] = await Item.FromStream(stream);
		/// Craftbook
		// Items
		//var itemBook = await ItemBook.FromStream(stream);
		// =UNKNOWN | UNUSED=
		//stream.Seek(0x54, SeekOrigin.Current);
		// Potions
		//var potionBook = await PotionBook.FromStream(stream);
		/// Achievements
		/// Status
		// Health
		return new Player(id, name, version, unlockedPlanets, character, gameplay, items);
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
	public readonly Character Character;
	public readonly Gameplay Gameplay;
	// Inventory
	public readonly Item[] Items = new Item[COUNT_ITEMS];
	public ArraySegment<Item> SurvivalHotbar { get { return new ArraySegment<Item>(this.Items,  0, 10); }}  // 10
	public ArraySegment<Item> CreativeHotbar { get { return new ArraySegment<Item>(this.Items, 10, 10); }}  // 20
	public ArraySegment<Item> CraftingSlots  { get { return new ArraySegment<Item>(this.Items, 20,  9); }}  // 29
	public ArraySegment<Item> Inventory      { get { return new ArraySegment<Item>(this.Items, 29, 36); }}  // 65
	// Order: Helm, Chestpiece, Leggings, Feet, Special
	public ArraySegment<Item> ArmorActual    { get { return new ArraySegment<Item>(this.Items, 65,  5); }}  // 70
	public ArraySegment<Item> ArmorVisual    { get { return new ArraySegment<Item>(this.Items, 70,  5); }}  // 75
	public Item CraftSlot                    { get { return Items[75]; }}                                   // 76
	public Item ArrowSlot                    { get { return Items[76]; }}                                   // 77
	// Craftbook
	// Achievements
	// Status
	public float Health = 50.0f;
	public Effect[] Effects = new Effect[COUNT_EFFECTS];
	/* Class Properties */
	private const byte BUFFER_SIZE           = 16;
	private const byte SIZEOF_UUID           = 16;
	private const byte SIZEOF_NAME           = 16;
	private const byte SIZEOF_VERSION        =  4;
	private const byte SIZEOF_THEME          =  4;
	private const byte SIZEOF_GAMEPLAY_FLAGS =  4;
	private const byte COUNT_ITEMS           = 77;
	private const byte SIZEOF_HEALTH         =  4;
	private const byte COUNT_EFFECTS         =  4;
}
