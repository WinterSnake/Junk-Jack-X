/*
	Junk Jack X: Core
	- Player

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
	:<Craftbooks>
	:<Achievements>
	:<Status>
	Segment[0x548 : 0x53B] = Health                 | Length:   4   (0x4) | Type: float32
	Segment[0x53C : 0x54C] = Effects                | Length:  16  (0x10) | Type: struct Effect[4]
	-----------------------------------------------------------------------------------------------------------------------------

	Written By: Ryan Smith
*/
using System;
using System.Threading.Tasks;

namespace JJx;

public class Player
{
	/* Constructor */
	public Player(string name)
	{
		// Info
		this.Id = Guid.NewGuid();
		this.Name = name;
	}
	private Player(Guid id, string name)
	{
		this.Id = id;
		this._Name = name;
	}
	/* Instance Methods */
	/* Static Methods */
	public static async Task<Player> FromStream(ArchiverStream stream)
	{
		if (stream.Type != ArchiverStreamType.Player || !stream.CanRead)
			throw new ArgumentException($"Expected readable player stream, either incorrect stream type (Type:{stream.Type}) or not readable (Readable:{stream.CanRead})");
		var bytesRead = 0;
		var buffer = new byte[SIZEOF_BUFFER];
		/// Info
		while (bytesRead < buffer.Length)
			bytesRead += await stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
		// UUID
		var id = new Guid(new Span<byte>(buffer, Player.OFFSET_UUID, Player.SIZEOF_UUID));
		// Name
		var name = JJx.BitConverter.GetString(buffer, Player.OFFSET_NAME);
		return new Player(id, name);
	}
	/* Properties */
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
	/* Class Properties */
	private const byte OFFSET_UUID   =  0;
	private const byte OFFSET_NAME   = 16;
	private const byte SIZEOF_UUID   = 16;
	private const byte SIZEOF_NAME   = 16;
	private const byte SIZEOF_BUFFER = 32;
}
