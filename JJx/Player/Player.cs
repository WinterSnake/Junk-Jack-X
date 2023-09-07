/*
	Junk Jack X: Player

	Segment Breakdown:
	------------------------------------------------------------------------------------------------------------------------
	Segment[0x0   :  0x3]  = JJ Character Header | Length: 4   (0x04)  | Type: char[4]
	Segment[0x4   :  0x5]  = JJ Type Header      | Length: 2   (0x2)   | Type: uint16 <00 00: player>
	Segment[0x4   : 0x47]  = UNKNOWN FOR NOW     | Length: 71  (0x47)  | Type: ??? Possible Header/File Length/CRC
	Segment[0x48  : 0x57]  = UUID                | Length: 16  (0x10)  | Type: uuid
	Segment[0x58  : 0x67]  = Name                | Length: 16  (0x10)  | Type: char*
	Segment[0x68  : 0x6B]  = Game Version        | Length: 4   (0x4)   | Type: uint32          | Parent: JJx.Version
	Segment[0x6C  : 0x6F]  = UNKNOWN FOR NOW     | Length: 4   (0x4)   | Type: ???
	Segment[0x70]          = Gameplay Flags      | Length: 1   (0x1)   | Type: enum flag       | Parent: Gameplay.Flags
	Segment[0x71  : 0x73]  = UNKNOWN FOR NOW     | Length: 3   (0x3)   | Type: ???
	Segment[0x74]          = Hair Color          | Length: 1   (0x1)   | Type: enum            | Parent: Character
	Segment[0x75]          = Gender/Skin/Hair    | Length: 1   (0x1)   | Type: bitfield        | Parent: Character
	Segment[0x76  :  0x77] = UNKNOWN FOR NOW     | Length: 2   (0x2)   | Type: ???
	Segment[0x78]          = Gameplay Difficulty | Length: 1   (0x1)   | Type: enum            | Parent: Gameplay.Difficulty
	Segment[0x79  :  0x7C] = UNKNOWN FOR NOW     | Length: 3   (0x3)   | Type: ???
	Segment[0x7D  :  0xF3] = Hotbar: Survival    | Length: 120 (0x78)  | Type: struct Item[10] | Parent: Items
	Segment[0xF4  : 0x16B] = Hotbar: Creative    | Length: 120 (0x78)  | Type: struct Item[10] | Parent: Items
	Segment[0x16C : 0x16B] = Crafting Slots      | Length: 108 (0x6C)  | Type: struct Item[9]  | Parent: Items
	Segment[0x1D8 : 0x387] = Inventory           | Length: 432 (0x1B0) | Type: struct Item[36] | Parent: Items
	Segment[0x388 : 0x3C3] = Actual Armor Slots  | Length: 60  (0x3C)  | Type: struct Item[5]  | Parent: Items
	Segment[0x3C4 : 0x3FF] = Visual Armor Slots  | Length: 60  (0x3C)  | Type: struct Item[5]  | Parent: Items
	Segment[0x400 : 0x40B] = Craft Slot          | Length: 12  (0xC)   | Type: struct Item     | Parent: Items
	Segment[0x40C : 0x417] = Arrow Slot          | Length: 12  (0xC)   | Type: struct Item     | Parent: Items
	Segment[0x418 : 0x54C] = UNKNOWN FOR NOW     | Length: 308 (0x134) | Type: ???
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
	public Player(string name, Gameplay.Flag flags = Gameplay.Flag.None)
	{
		var rnd = new Random();
		this.Id = Guid.NewGuid();
		this.Name = name;
		// Random character design
		this.Character = new Character(
			rnd.NextDouble() >= 0.5 ,
			(byte)rnd.Next(Character.MAX_TONES + 1),
			(byte)rnd.Next(Hair.MAX_STYLES + 1),
			(Hair.HColor)(rnd.Next(Enum.GetValues(typeof(Hair.HColor)).Length))
		);
		// Default gameplay data
		this.Gameplay = new Gameplay(Difficulty.Normal, flags);
		// Set items to none
		for (var i = 0; i < this.Items.Length; ++i)
			this.Items[i] = new Item(0, 0);
	}
	private Player(Guid id, string name, Character character, Gameplay gameplay, Item[] items)
	{
		this.Id = id;
		this._Name = name;
		this.Character = character;
		this.Gameplay = gameplay;
		this.Items = items;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
		// TODO: Use full buffer before WriteAynsc
	{
		var workingData = new byte[BUFFER_SIZE];
		//----Unknown----\\
		stream.Seek(0x48, SeekOrigin.Current);
		// Uuid
		var uuid = this.Id.ToByteArray();
		await stream.WriteAsync(uuid, 0, uuid.Length);
		// Name
		var byteCount = Encoding.ASCII.GetBytes(this._Name, 0, this._Name.Length, workingData, 0);
		for (var i = byteCount; i < SIZEOF_NAME; ++i)
			workingData[i] = 0;
		await stream.WriteAsync(workingData, 0, SIZEOF_NAME);
		//----Unknown----\\
		stream.Seek(0x8, SeekOrigin.Current);
		// Gameplay: Flags
		stream.WriteByte((byte)this.Gameplay.Flags);
		//----Unknown----\\
		stream.Seek(0x3, SeekOrigin.Current);
		// Features
		this.Character.Pack(new Span<byte>(workingData));
		await stream.WriteAsync(workingData, 0, SIZEOF_CHARACTER);
		//----Unknown----\\
		stream.Seek(0x2, SeekOrigin.Current);
		// Gameplay: Difficulty
		stream.WriteByte((byte)this.Gameplay.Difficulty);
		//----Unknown----\\
		stream.Seek(0x3, SeekOrigin.Current);
		// Items
		foreach (var item in this.Items)
			await item.ToStream(stream);
		//----Unknown----\\
	}
	/* Static Methods */
	public static async Task<Player> FromStream(Stream stream)
	{
		var bytesRead = 0;
		var workingData = new byte[BUFFER_SIZE];
		//----Unknown----\\
		stream.Seek(0x48, SeekOrigin.Current);
		// Uuid
		bytesRead = 0;
		while (bytesRead < SIZEOF_UUID)
			bytesRead += await stream.ReadAsync(workingData, 0, SIZEOF_UUID - bytesRead);
		Guid id = new Guid(new Span<byte>(workingData, 0, SIZEOF_UUID));
		// Name
		bytesRead = 0;
		while (bytesRead < SIZEOF_NAME)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_NAME - bytesRead);
		string name = Encoding.ASCII.GetString(
			new Span<byte>(workingData, 0, Array.IndexOf(workingData, byte.MinValue))
		);
		//----Unknown----\\
		stream.Seek(0x8, SeekOrigin.Current);
		// Gameplay: Flags
		var flags = stream.ReadByte();
		//----Unknown----\\
		stream.Seek(0x3, SeekOrigin.Current);
		// Character
		bytesRead = 0;
		while (bytesRead < SIZEOF_CHARACTER)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_CHARACTER - bytesRead);
		var character = new Character(new Span<byte>(workingData, 0, 2));
		//----Unknown----\\
		stream.Seek(0x2, SeekOrigin.Current);
		// Gameplay: Difficulty
		var difficulty = stream.ReadByte();
		// -Gameplay
		var gameplay = new Gameplay((byte)difficulty, (byte)flags);
		//----Unknown----\\
		stream.Seek(0x3, SeekOrigin.Current);
		// Items
		var items = new Item[COUNT_ITEMS];
		for (var i = 0; i < items.Length; ++i)
			items[i] = await Item.FromStream(stream);
		//----Unknown----\\
		return new Player(id, name, character, gameplay, items);
	}
	/* Properties */
	public readonly Guid Id;
	private string _Name;
	public string Name {
		// Min Length: 1 | Max length: 15 characters -- 16 = null termination
		get { return this._Name; }
		set {
			if (String.IsNullOrEmpty(value)) return;
			else if (value.Length < SIZEOF_NAME) this._Name = value;
			else this._Name = value.Substring(0, SIZEOF_NAME - 1);
		}
	}
	public readonly Character Character;
	public readonly Gameplay Gameplay;
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
	/* Class Properties */
	public  const byte MAXLENGTH_NAME   = SIZEOF_NAME - 1;
	private const byte BUFFER_SIZE      = 32;
	private const byte SIZEOF_UUID      = 16;
	private const byte SIZEOF_NAME      = 16;
	private const byte SIZEOF_CHARACTER = 2;
	private const byte COUNT_ITEMS      = 77;
}
