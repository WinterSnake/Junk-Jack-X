/*
	Junk Jack X: World

	Sizes in blocks:
		[TINY]:   512 * 128
		[SMALL]:  {x} * {y}
		[NORMAL]: {x} * {y}
		[LARGE]:  {x} * {y}
		[HUGE]:   {x} * {y}

	Segment Breakdown:
	------------------------------------------------------------------------------------------------------------------------
	Segment[0x0   :   0x3]      = JJ World Header  | Length: 4   (0x04)  | Type: char[4]
	Segment[0x4   :  0xEF]      = UNKNOWN FOR NOW  | Length: 236 (0xEC)  | Type: ??? Possible Header/File Length/CRC
	Segment[0xF0  :  0xFF]      = UUID             | Length: 16  (0x10)  | Type: uuid
	Segment[0x100 : 0x107]      = UNKNOWN FOR NOW  | Length: 8   (0x8)   | Type: ??? Possible long/epoch/DateTime
	Segment[0x108 : 0x118]      = Name             | Length: 16  (0x10)  | Type: char*
	Segment[0x118 : 0x13B]      = UNKNOWN FOR NOW  | Length: 36  (0x24)  | Type: ???
	Segment[0x13C : 0x13D]      = Player.X         | Length: 2   (0x2)   | Type uint32              | Parent: Player.X
	Segment[0x13E : 0x13F]      = Player.Y         | Length: 2   (0x2)   | Type uint32              | Parent: Player.Y
	Segment[0x140 : 0x141]      = Spawn.X          | Length: 2   (0x2)   | Type uint32              | Parent: Spawn.X
	Segment[0x142 : 0x143]      = Spawn.Y          | Length: 2   (0x2)   | Type uint32              | Parent: Spawn.Y
	Segment[0x144 : 0x145]      = Planet           | Length: 2   (0x2)   | Type enum
	Segment[0x146 : 0x148]      = UNKNOWN FOR NOW  | Length: 3   (0x3)   | Type: ???
	Segment[0x149]              = Gamemode         | Length: 1   (0x1)   | Type: enum               | Parent: Gamemode
	Segment[0x14A : 0x1CF]      = UNKNOWN FOR NOW  | Length: 133 (0x86)  | Type: ???
	Segment[0x1D0 : 0x{size.X}] = Background Layer | Length: {size.x}    | Type: uint16_t[{size.x}] | Parent: Background
	------------------------------------------------------------------------------------------------------------------------

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace JJx;

public enum Gamemode: byte
{
	Survival = 0x0,
	Creative,
	Flat
}

public enum Planet : ushort
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
		(ushort X, ushort Y) spawn, (ushort X, ushort Y) player,
		ushort[] background
	)
	{
		this.Id = id;
		this._Name = name;
		this.Gamemode = gamemode;
		this.Planet = planet;
		this.Spawn = spawn;
		this.Player = player;
		this.Background = background;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var workingData = new byte[64];
		//----Unknown----\\
		stream.Seek(0xF0, SeekOrigin.Current);
		// Uuid
		var uuid = this.Id.ToByteArray();
		await stream.WriteAsync(uuid, 0, 0x10);
		//----Unknown----\\
		stream.Seek(0x8, SeekOrigin.Current);
		// Name
		var byteCount = Encoding.ASCII.GetBytes(this._Name, 0, this._Name.Length, workingData, 0);
		for (var i = byteCount; i < 0x10; ++i) { workingData[i] = 0; }
		await stream.WriteAsync(workingData, 0, 0x10);
		//----Unknown----\\
		stream.Seek(0x24, SeekOrigin.Current);
		// Player
		workingData[3] = (byte)((this.Player.Y & 0xFF00) >> 8);
		workingData[2] = (byte)((this.Player.Y & 0x00FF) >> 0);
		workingData[1] = (byte)((this.Player.X & 0xFF00) >> 8);
		workingData[0] = (byte)((this.Player.X & 0x00FF) >> 0);
		await stream.WriteAsync(workingData, 0, 0x4);
		// Spawn
		workingData[3] = (byte)((this.Spawn.Y & 0xFF00) >> 8);
		workingData[2] = (byte)((this.Spawn.Y & 0x00FF) >> 0);
		workingData[1] = (byte)((this.Spawn.X & 0xFF00) >> 8);
		workingData[0] = (byte)((this.Spawn.X & 0x00FF) >> 0);
		await stream.WriteAsync(workingData, 0, 0x4);
		// Planet
		workingData[1] = (byte)(((ushort)this.Planet & 0xFF00) >> 8);
		workingData[0] = (byte)(((ushort)this.Planet & 0x00FF) >> 0);
		await stream.WriteAsync(workingData, 0, 0x2);
		//----Unknown----\\
		stream.Seek(0x3, SeekOrigin.Current);
		// Gamemode
		stream.WriteByte((byte)this.Gamemode);
		//----Unknown----\\
		stream.Seek(0x86, SeekOrigin.Current);
		// Background Layer
		// TODO: USE FULL BUFFER
		foreach (var bg in this.Background)
		{
			workingData[1] = (byte)((bg & 0xFF00) >> 8);
			workingData[0] = (byte)((bg & 0x00FF) >> 0);
			await stream.WriteAsync(workingData, 0, 0x2);
		}
		//----Unknown----\\
	}
	/* Static Methods */
	public static async Task<World> FromStream(Stream stream)
	{
		var bytesRead = 0;
		var workingData = new byte[64];
		//----Unknown----\\
		stream.Seek(0xF0, SeekOrigin.Current);
		// Uuid
		bytesRead = 0;
		while (bytesRead < UUIDSIZE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, UUIDSIZE - bytesRead);
		Guid id = new Guid(new Span<byte>(workingData).Slice(0, UUIDSIZE));
		//----Unknown----\\
		stream.Seek(0x8, SeekOrigin.Current);
		// Name
		bytesRead = 0;
		while (bytesRead < NAMESIZE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, NAMESIZE - bytesRead);
		var name = Encoding.ASCII.GetString(
			new Span<byte>(workingData).Slice(0, Array.IndexOf(workingData, byte.MinValue))
		);
		//----Unknown----\\
		stream.Seek(0x24, SeekOrigin.Current);
		// Player
		bytesRead = 0;
		while (bytesRead < PLAYERPOSSIZE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, PLAYERPOSSIZE - bytesRead);
		var player = (
			(ushort)((workingData[1] << 8) | workingData[0]),
			(ushort)((workingData[3] << 8) | workingData[2])
		);
		// Spawn
		bytesRead = 0;
		while (bytesRead < SPAWNPOSSIZE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SPAWNPOSSIZE - bytesRead);
		var spawn = (
			(ushort)((workingData[1] << 8) | workingData[0]),
			(ushort)((workingData[3] << 8) | workingData[2])
		);
		// Planet
		bytesRead = 0;
		while (bytesRead < PLANETSIZE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, PLANETSIZE - bytesRead);
		var planet = (Planet)((workingData[1] << 8) | workingData[0]);
		//----Unknown----\\
		stream.Seek(0x3, SeekOrigin.Current);
		// Gamemode
		var gamemode = (Gamemode)stream.ReadByte();
		//----Unknown----\\
		stream.Seek(0x86, SeekOrigin.Current);
		// Background Layer
		bytesRead = 0;
		var background = new ushort[512];
		// Blocks
		//----Unknown----\\
		return new World(
			id, name, gamemode, planet, spawn, player, background
		);
	}
	/* Properties */
	public readonly Guid Id;
	private string _Name;
	public string Name {
		// Min Length: 1 | Max length: 15 characters -- 16 = null termination
		get { return this._Name; }
		set {
			if (String.IsNullOrEmpty(value)) return;
			else if (value.Length < 16) this._Name = value;
			else this._Name = value.Substring(0, 15);
		}
	}
	public Gamemode Gamemode;
	public (ushort X, ushort Y) Spawn;
	public (ushort X, ushort Y) Player;
	public Planet Planet;
	public readonly ushort[] Background;
	/* Class Properties */
	private const byte UUIDSIZE = 16;
	private const byte NAMESIZE = 16;
	private const byte PLAYERPOSSIZE = 4;
	private const byte SPAWNPOSSIZE = 4;
	private const byte PLANETSIZE = 2;
}
