/*
	Junk Jack X: Player

	Player class references both the player file and player stats file for creating, loading, editing, and saving.
	This file documents both the hex offsets (and length) and their C equivalent types.

	Segment Breakdown:
	------------------------------------------------------------------------------------------------------------------------
	Segment[0x0]   - Segment[0x3]   = JJ Character Header | Length: 4   (0x04)  | Type: char[4]
	Segment[0x4]   - Segment[0x47]  = UNKNOWN FOR NOW     | Length: 71  (0x47)  | Type: ??? Possible Header/File Length/CRC
	Segment[0x48]  - Segment[0x57]  = UUID                | Length: 16  (0x10)  | Type: uuid
	Segment[0x58]  - Segment[0x67]  = Name                | Length: 16  (0x10)  | Type: char*
	Segment[0x68]  - Segment[0x6F]  = UNKNOWN FOR NOW     | Length: 8   (0x8)   | Type: ???
	Segment[0x70]                   = Gameplay Flags      | Length: 1   (0x1)   | Type: enum flag       | Parent: Gameplay.Flags
	Segment[0x71]  - Segment[0x73]  = UNKNOWN FOR NOW     | Length: 3   (0x3)   | Type: ???
	Segment[0x74]                   = Hair Color          | Length: 1   (0x1)   | Type: enum            | Parent: Character
	Segment[0x75]                   = Gender/Skin/Hair    | Length: 1   (0x1)   | Type: bitfield        | Parent: Character
	Segment[0x76]  - Segment[0x77]  = UNKNOWN FOR NOW     | Length: 2   (0x2)   | Type: ???
	Segment[0x78]                   = Gameplay Difficulty | Length: 1   (0x1)   | Type: enum            | Parent: Gameplay.Difficulty
	Segment[0x79]  - Segment[0x7C]  = UNKNOWN FOR NOW     | Length: 3   (0x3)   | Type: ???
	Segment[0x7D]  - Segment[0xF3]  = Hotbar: Survival    | Length: 120 (0x78)  | Type: struct Item[10] | Parent: HotbarSurvival
	Segment[0xF4]  - Segment[0x16B] = Hotbar: Creative    | Length: 120 (0x78)  | Type: struct Item[10] | Parent: HotbarCreative
	Segment[0x16C] - Segment[0x16B] = Crafting Slots      | Length: 108 (0x6C)  | Type: struct Item[9]  | Parent: CraftSlots
	Segment[0x1D8] - Segment[0x387] = Hotbar: Survival    | Length: 432 (0x1B0) | Type: struct Item[36] | Parent: Inventory
	Segment[0x388] - Segment[0x3FF] = UNKNOWN FOR NOW     | Length: 120 (0x78)  | Type: ???
	Segment[0x400] - Segment[0x40B] = Craft Slot          | Length: 12  (0xC)   | Type: struct Item     | Parent: CraftSlot
	Segment[0x40C] - Segment[0x417] = Arrow Slot          | Length: 12  (0xC)   | Type: struct Item     | Parent: ArrowSlow
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
	public Player(string name)
	{
		this.Id = Guid.NewGuid();
		this.Name = name;
		this.Character = new Character(0, 0);
		this.Gameplay = new Gameplay(0, 0);
	}
	private Player(Guid id, string name, Character character, Gameplay gameplay)
	{
		this.Id = id;
		this._Name = name;
		this.Character = character;
		this.Gameplay = gameplay;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var byteCount = 0;
		var workingData = new byte[64];
		//----Unknown----\\
		stream.Seek(0x48, SeekOrigin.Current);
		// Uuid
		var uuid = this.Id.ToByteArray();
		await stream.WriteAsync(uuid, 0, 0x10);
		// Name
		byteCount = Encoding.ASCII.GetBytes(this._Name, 0, this._Name.Length, workingData, 0);
		for (var i = byteCount; i < 0x10; ++i) { workingData[i] = 0; }
		await stream.WriteAsync(workingData, 0, 0x10);
		//----Unknown----\\
		stream.Seek(0x8, SeekOrigin.Current);
		// Gameplay: Flags
		stream.WriteByte((byte)this.Gameplay.Flags);
		//----Unknown----\\
		stream.Seek(0x3, SeekOrigin.Current);
		// Features
		this.Character.ToByteArray(ref workingData);
		await stream.WriteAsync(workingData, 0, 0x2);
		//----Unknown----\\
		stream.Seek(0x2, SeekOrigin.Current);
		// Gameplay: Difficulty
		stream.WriteByte((byte)this.Gameplay.Difficulty);
		// Hotbar: Survival
		stream.Seek(0x3, SeekOrigin.Current);
		for (var i = 0; i < this.HotbarSurvival.Length; ++i) { await this.HotbarSurvival[i].ToStream(stream); }
		// Hotbar: Creative
		for (var i = 0; i < this.HotbarCreative.Length; ++i) { await this.HotbarCreative[i].ToStream(stream); }
		// Crafting Slots
		for (var i = 0; i < this.CraftSlots.Length; ++i) { await this.CraftSlots[i].ToStream(stream); }
		// Inventory
		for (var i = 0; i < this.Inventory.Length; ++i) { await this.Inventory[i].ToStream(stream); }
		//----Unknown----\\
		stream.Seek(0x78, SeekOrigin.Current);
		// Craft Slot
	}
	/* Static Methods */
	public static async Task<Player> FromStream(Stream stream)
	{
		var workingData = new byte[64];
		//----Unknown----\\
		stream.Seek(0x48, SeekOrigin.Current);
		// Uuid
		await stream.ReadAsync(workingData, 0, 0x10);
		Guid id = new Guid(new Span<byte>(workingData).Slice(0, 0x10));
		// Name
		await stream.ReadAsync(workingData, 0, 0x10);
		string name = Encoding.ASCII.GetString(
			new Span<byte>(workingData).Slice(0, Array.IndexOf(workingData, byte.MinValue))
		);
		//----Unknown----\\
		stream.Seek(0x8, SeekOrigin.Current);
		// Gameplay: Flags
		var flags = stream.ReadByte();
		//----Unknown----\\
		stream.Seek(0x3, SeekOrigin.Current);
		// Features
		await stream.ReadAsync(workingData, 0, 0x02);
		var character = new Character(workingData[0], workingData[1]);
		//----Unknown----\\
		stream.Seek(0x2, SeekOrigin.Current);
		// Gameplay: Difficulty
		var difficulty = stream.ReadByte();
		// -Gameplay
		var gameplay = new Gameplay((byte)difficulty, (byte)flags);
		var player = new Player(id, name, character, gameplay);
		//----Unknown----\\
		stream.Seek(0x3, SeekOrigin.Current);
		// Hotbar: Survival
		for (var i = 0; i < player.HotbarSurvival.Length; ++i) { player.HotbarSurvival[i] = await Item.FromStream(stream); }
		// Hotbar: Creative
		for (var i = 0; i < player.HotbarCreative.Length; ++i) { player.HotbarCreative[i] = await Item.FromStream(stream); }
		// Crafting Slots
		for (var i = 0; i < player.CraftSlots.Length; ++i) { player.CraftSlots[i] = await Item.FromStream(stream); }
		// Inventory
		for (var i = 0; i < player.Inventory.Length; ++i) { player.Inventory[i] = await Item.FromStream(stream); }
		//----Unknown----\\
		stream.Seek(0x78, SeekOrigin.Current);
		// Craft Slot
		player.CraftSlot = await Item.FromStream(stream);
		// Arrow Slot
		player.ArrowSlot = await Item.FromStream(stream);
		return player;
	}
	/* Properties */
	public Guid Id { get; init; }
	private string _Name;
	public string Name {
		// Min Length: 1 | Max length: 15 characters -- 16 = null termination
		get { return this._Name; }
		set {
			if (value.Length < 16) this._Name = value;
			else this._Name = value.Substring(0, 15);
		}
	}
	public Character Character;
	public Gameplay Gameplay;
	public readonly Item[] HotbarSurvival = new Item[10];
	public readonly Item[] HotbarCreative = new Item[10];
	public readonly Item[] Inventory = new Item[36];
	public readonly Item[] CraftSlots = new Item[9];
	public Item CraftSlot;
	public Item ArrowSlot;
}
