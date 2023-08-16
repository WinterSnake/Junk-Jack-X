/*
	Junk Jack X: World

	Segment Breakdown:
	----------------------------------------------------------------------------------------------------------------------------------------------------------------------
	Segment[0x0       :        0x3] = JJ World Header                | Length: 4   (0x4)  | Type: char[4]
	Segment[0x4       :        0x5] = JJ Type Header                 | Length: 2   (0x2)  | Type: uint16 <01 00: world | 02 00: adventure>
	Segment[0x6       :        0x9] = UNKNOWN FOR NOW                | Length: 10  (0xA)  | Type: ???
	Segment[0x10      :       0x13] = World Info Location            | Length: 4   (0x4)  | Type: uint32
	Segment[0x14      :       0x17] = World Info Size                | Length: 4   (0x4)  | Type: uint32
	Segment[0x18      :       0x1B] = UNKNOWN FOR NOW                | Length: 4   (0x4)  | Type: ???
	Segment[0x1C      :       0x1F] = Background Location            | Length: 4   (0x4)  | Type: uint32
	Segment[0x20      :       0x23] = Background Size                | Length: 4   (0x4)  | Type: uint32
	Segment[0x24      :       0x27] = UNKNOWN FOR NOW                | Length: 4   (0x4)  | Type: ???
	Segment[0x28      :       0x2B] = Compressed World Location      | Length: 4   (0x4)  | Type: uint32
	Segment[0x2C      :       0x2F] = Compressed World Size          | Length: 4   (0x4)  | Type: uint32
	Segment[0x30      :       0x4B] = UNKNOWN FOR NOW                | Length: 28  (0x1C) | Type: ???
	Segment[0x4C      :       0x4F] = Chest Footer Location          | Length: 4   (0x4)  | Type: uint32
	Segment[0x50      :       0x53] = Chest Footer Size              | Length: 4   (0x4)  | Type: uint32
	Segment[0x54      :       0x57] = UNKNOWN FOR NOW                | Length: 4   (0x4)  | Type: ???
	Segment[0x58      :       0x5B] = Forge Footer Location          | Length: 4   (0x4)  | Type: uint32
	Segment[0x5C      :       0x5F] = Forge Footer Size              | Length: 4   (0x4)  | Type: uint32
	Segment[0x60      :       0x63] = UNKNOWN FOR NOW                | Length: 4   (0x4)  | Type: ???
	Segment[0x64      :       0x67] = Sign Footer Location           | Length: 4   (0x4)  | Type: uint32
	Segment[0x68      :       0x6B] = Sign Footer Size               | Length: 4   (0x4)  | Type: uint32
	Segment[0x6C      :       0x6F] = UNKNOWN FOR NOW                | Length: 4   (0x4)  | Type: ???
	Segment[0x7C      :       0x7F] = Lab Footer Location            | Length: 4   (0x4)  | Type: uint32
	Segment[0x80      :       0x83] = Lab Footer Size                | Length: 4   (0x4)  | Type: uint32
	Segment[0x6C      :       0x6F] = UNKNOWN FOR NOW                | Length: 4   (0x4)  | Type: ???
	Segment[0xD4      :       0xDF] = Entity Footer Location         | Length: 4   (0x4)  | Type: uint32
	Segment[0xE0      :       0xE3] = Entity Footer Size             | Length: 4   (0x4)  | Type: uint32
	Segment[0xE4      :       0xEF] = UNKNOWN FOR NOW                | Length: 12  (0xC)  | Type: ???
	Segment[0xF0      :       0xFF] = UUID                           | Length: 16  (0x10) | Type: uuid
	Segment[0x100     :      0x107] = UNKNOWN FOR NOW                | Length: 8   (0x8)  | Type: ??? Possible long/epoch/DateTime
	Segment[0x108     :      0x118] = Name                           | Length: 16  (0x10) | Type: char*
	Segment[0x118     :      0x137] = UNKNOWN FOR NOW                | Length: 32  (0x20) | Type: ???
	Segment[0x138     :      0x139] = World.Width                    | Length: 2   (0x2)  | Type uint32                    | Parent: Blocks.Width
	Segment[0x13A     :      0x13B] = World.Height                   | Length: 2   (0x2)  | Type uint32                    | Parent: Blocks.Height
	Segment[0x13C     :      0x13D] = Player.X                       | Length: 2   (0x2)  | Type uint32                    | Parent: Player.X
	Segment[0x13E     :      0x13F] = Player.Y                       | Length: 2   (0x2)  | Type uint32                    | Parent: Player.Y
	Segment[0x140     :      0x141] = Spawn.X                        | Length: 2   (0x2)  | Type uint32                    | Parent: Spawn.X
	Segment[0x142     :      0x143] = Spawn.Y                        | Length: 2   (0x2)  | Type uint32                    | Parent: Spawn.Y
	Segment[0x144     :      0x145] = Planet                         | Length: 2   (0x2)  | Type enum
	Segment[0x146     :      0x147] = UNKNOWN FOR NOW                | Length: 2   (0x2)  | Type: ???
	Segment[0x148]                  = Season                         | Length: 1   (0x1)  | Type: enum                     | Parent: Season
	Segment[0x149]                  = Gamemode                       | Length: 1   (0x1)  | Type: enum                     | Parent: Gamemode
	Segment[0x14A]                  = World Size                     | Length: 1   (0x1)  | Type: enum                     | Parent: Size
	Segment[0x14B]                  = Sky Size                       | Length: 1   (0x1)  | Type: enum                     | Parent: Size
	Segment[0x14C     :      0x1CF] = UNKNOWN FOR NOW                | Length: 131 (0x84) | Type: ???
	Segment[{bg loc}  :  {bg size}] = Background Layer               | Length: {bg size}  | Type: uint16_t[{bg size}]      | Parent: Background
	Segment[{wld loc} : {wld size}] = Compressed World Data {Blocks} | Length: {wld size} | Type: struct Block[{wld size}] | Parent: Blocks

	{{END OF COMPRESSED WORLD}}
	Segment[0x0 : 0x1] = Time ???                       | Length: 2   (0x2)  | Type: ???
	Segment[0x2 : 0x3] = UNKNOWN FOR NOW                | Length: 2   (0x2)  | Type: ???
	Segment[0x4]       = Skybox {Day/Night} ???         | Length: 1   (0x1)  | Type: ???
	Segment[0x5 : 0xB] = UNKNOWN FOR NOW                | Length: 2   (0x2)  | Type: ???
	Segment[0xC]       = Weather                        | Length: 1   (0x1)  | Type: ???

	{{CHEST FOOTER}}
	Segment[0x0 : 0x3] = # of chests   | Length: 4   (0x4)  | Type: uint32
	<ChestContainer[]>

	{{FORGE FOOTER}}
	Segment[0x0 : 0x3] = # of forges   | Length: 4   (0x4)  | Type: uint32
	<ForgeContainer[]>

	{{SIGN FOOTER}}
	Segment[0x0 : 0x3] = # of signs    | Length: 4   (0x4)  | Type: uint32
	<Sign[]>

	{{LAB FOOTER}}
	Segment[0x0 : 0x3] = # of labs     | Length: 4   (0x4)  | Type: uint32
	<LabContainer[]>

	{{ENTITY FOOTER}}
	Segment[0x0 : 0x3] = # of entities | Length: 4   (0x4)  | Type: uint32
	<Entity[]>
	----------------------------------------------------------------------------------------------------------------------------------------------------------------------

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace JJx;

public enum Gamemode : byte
{
	Survival = 0x0,
	Creative,
	Flat
}

public enum Season : byte
{
	Spring = 0x1,
	Summer = 0x2,
	Autumn = 0x4,
	Winter = 0x8,
	None   = 0xF
}

public enum Planet : ushort
	// TODO: Convert to byte enum
{
	Terra  = 0x0001,
	Seth   = 0x0002,
	Alba   = 0x0004,
	Xeno   = 0x0008,
	Magmar = 0x0010,
	Cryo   = 0x0020,
	Yuca   = 0x0040,
	Lilith = 0x0080,
	Thetis = 0x0100,
	Mykon  = 0x0200,
	Umbra  = 0x0400,
	Tor    = 0x0800
}

public enum Weather: byte
{
	None,
	Rain,
	Snow,
	AcidRain
}

public enum Size : byte
{
	Tiny = 0x0,  // Size: 512 * 128
	Small,       // Size: x * y
	Normal,      // Size: x * y
	Large,       // Size: x * y
	Huge         // Size: x * y
}

public sealed class World
{
	/* Constructors */
	public World(string name, Gamemode gamemode, Planet planet)
	{
		this.Id = Guid.NewGuid();
		this.Name = name;
		this.Gamemode = gamemode;
		this.Planet = planet;
		this.Spawn = (0, 0);
		this.Player = (0, 0);
	}
	private World(
		Guid id, string name, Gamemode gamemode, Planet planet,
		(ushort X, ushort Y) spawn, (ushort X, ushort Y) player
	)
	{
		this.Id = id;
		this._Name = name;
		this.Gamemode = gamemode;
		this.Planet = planet;
		this.Spawn = spawn;
		this.Player = player;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
		// TODO: Use full buffer before WriteAynsc
		// TODO: Ensure BitConverter.GetBytes<T> forces little endian
	{
		byte[] bytes;
		var workingData = new byte[BUFFER_SIZE];
		//----Unknown----\\
		stream.Seek(0xF0, SeekOrigin.Current);
		// Uuid
		var uuid = this.Id.ToByteArray();
		await stream.WriteAsync(uuid, 0, uuid.Length);
		//----Unknown----\\
		stream.Seek(0x8, SeekOrigin.Current);
		// Name
		var byteCount = Encoding.ASCII.GetBytes(this._Name, 0, this._Name.Length, workingData, 0);
		for (var i = byteCount; i < SIZEOF_NAME; ++i)
			workingData[i] = 0;
		await stream.WriteAsync(workingData, 0, SIZEOF_NAME);
		//----Unknown----\\
		stream.Seek(0x24, SeekOrigin.Current);
		// Player
		// -X
		bytes = BitConverter.GetBytes(this.Player.X);
		Array.Copy(bytes, 0, workingData, 0, bytes.Length);
		// -Y
		bytes = BitConverter.GetBytes(this.Player.Y);
		Array.Copy(bytes, 0, workingData, 2, bytes.Length);
		await stream.WriteAsync(workingData, 0, SIZEOF_PLAYERPOSITION);
		// Spawn
		// -X
		bytes = BitConverter.GetBytes(this.Spawn.X);
		Array.Copy(bytes, 0, workingData, 0, bytes.Length);
		// -Y
		bytes = BitConverter.GetBytes(this.Spawn.Y);
		Array.Copy(bytes, 0, workingData, 2, bytes.Length);
		await stream.WriteAsync(workingData, 0, SIZEOF_SPAWNPOSITION);
		// Planet
		workingData[1] = (byte)(((ushort)this.Planet & 0xFF00) >> 8);
		workingData[0] = (byte)(((ushort)this.Planet & 0x00FF) >> 0);
		await stream.WriteAsync(workingData, 0, SIZEOF_PLANET);
		//----Unknown----\\
		stream.Seek(0x3, SeekOrigin.Current);
		// Gamemode
		stream.WriteByte((byte)this.Gamemode);
		//----Unknown----\\
	}
	/* Static Methods */
	public static async Task<World> FromStream(Stream stream)
		// TODO: Ensure BitConverter.To<T> forces little endian
	{
		var bytesRead = 0;
		var workingData = new byte[BUFFER_SIZE];
		//----Unknown----\\
		stream.Seek(0xF0, SeekOrigin.Current);
		// Uuid
		bytesRead = 0;
		while (bytesRead < SIZEOF_UUID)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_UUID - bytesRead);
		Guid id = new Guid(new Span<byte>(workingData).Slice(0, SIZEOF_UUID));
		//----Unknown----\\
		stream.Seek(0x8, SeekOrigin.Current);
		// Name
		bytesRead = 0;
		while (bytesRead < SIZEOF_NAME)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_NAME - bytesRead);
		var name = Encoding.ASCII.GetString(
			new Span<byte>(workingData).Slice(0, Array.IndexOf(workingData, byte.MinValue))
		);
		//----Unknown----\\
		stream.Seek(0x24, SeekOrigin.Current);
		// Player
		bytesRead = 0;
		while (bytesRead < SIZEOF_PLAYERPOSITION)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_PLAYERPOSITION - bytesRead);
		var playerPosition = (
			BitConverter.ToUInt16(new Span<byte>(workingData).Slice(0, 2)),
			BitConverter.ToUInt16(new Span<byte>(workingData).Slice(2, 2))
		);
		// Spawn
		bytesRead = 0;
		while (bytesRead < SIZEOF_SPAWNPOSITION)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_SPAWNPOSITION - bytesRead);
		var spawnPosition = (
			BitConverter.ToUInt16(new Span<byte>(workingData).Slice(0, 2)),
			BitConverter.ToUInt16(new Span<byte>(workingData).Slice(2, 2))
		);
		// Planet
		bytesRead = 0;
		while (bytesRead < SIZEOF_PLANET)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZEOF_PLANET - bytesRead);
		var planet = (Planet)((workingData[1] << 8) | workingData[0]);
		//----Unknown----\\
		stream.Seek(0x3, SeekOrigin.Current);
		// Gamemode
		var gamemode = (Gamemode)stream.ReadByte();
		//----Unknown----\\
		return new World(id, name, gamemode, planet, spawnPosition, playerPosition);
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
	public Gamemode Gamemode;
	public (ushort X, ushort Y) Spawn;
	public (ushort X, ushort Y) Player;
	public Planet Planet;
	public readonly ushort[] Background;
	public readonly Block[] Blocks;
		/*
			TODO: Memory Mapped File Implementation
			https://learn.microsoft.com/en-us/dotnet/api/system.io.memorymappedfiles.memorymappedfile?view=net-7.0
			https://learn.microsoft.com/en-us/dotnet/api/system.io.memorymappedfiles.memorymappedviewstream?view=net-7.0

			Implement Background AND Block array as memmory mapped file with a viewer and getblock/setblock implementation
			
			Separate Blocks class with x/y indexer class
			https://stackoverflow.com/questions/6111049/2d-array-property
		*/
	/* Class Properties */
	public  const byte MAXLENGTH_NAME        = SIZEOF_NAME - 1;
	private const byte BUFFER_SIZE           = 64;
	private const byte SIZEOF_UUID           = 16;
	private const byte SIZEOF_NAME           = 16;
	private const byte SIZEOF_PLAYERPOSITION = 4;
	private const byte SIZEOF_SPAWNPOSITION  = 4;
	private const byte SIZEOF_PLANET         = 2;
}
