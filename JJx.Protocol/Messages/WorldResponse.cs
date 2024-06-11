/*
	Junk Jack X: Protocol
	- [Message: Responses]World

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.IO.Compression;

namespace JJx.Protocol;

public sealed class WorldInfoResponseMessage
{
	/* Constructor */
	public WorldInfoResponseMessage(
		(ushort, ushort) size, (ushort, ushort) spawn, (ushort, ushort) player,
		uint ticks, Period time, bool isTimeRunning, Weather weather, Planet theme,
		Difficulty difficulty, Planet planet, Season season, Gamemode gamemode,
		SizeType worldSizeType, SizeType skySizeType, uint sizeInBytes
	)
	{
		this.Size = size;
		this.Spawn = spawn;
		this.Player = player;
		this.Ticks = ticks;
		this.Time = time;
		this.IsTimeRunning = isTimeRunning;
		this.Weather = weather;
		this.Theme = planet;
		this.Difficulty = difficulty;
		this.Planet = planet;
		this.Season = season;
		this.Gamemode = gamemode;
		this.WorldSizeType = worldSizeType;
		this.SkySizeType = skySizeType;
		this.WorldSizeBytes = sizeInBytes;
	}
	/* Instance Methods */
	public byte[] Serialize()
	{
		var buffer = new byte[SIZE + sizeof(ushort)];
		BitConverter.BigEndian.Write((ushort)MessageHeader.WorldInfoResponse, buffer);
		BitConverter.LittleEndian.Write(this.Size.Width, buffer, OFFSET_SIZE + sizeof(ushort));
		BitConverter.LittleEndian.Write(this.Size.Height, buffer, OFFSET_SIZE + (sizeof(ushort) * 2));
		BitConverter.LittleEndian.Write(this.Spawn.X, buffer, OFFSET_SPAWN + sizeof(ushort));
		BitConverter.LittleEndian.Write(this.Spawn.Y, buffer, OFFSET_SPAWN + (sizeof(ushort) * 2));
		BitConverter.LittleEndian.Write(this.Player.X, buffer, OFFSET_PLAYER + sizeof(ushort));
		BitConverter.LittleEndian.Write(this.Player.Y, buffer, OFFSET_PLAYER + (sizeof(ushort) * 2));
		BitConverter.LittleEndian.Write(this.Ticks, buffer, OFFSET_TICKS + sizeof(ushort));
		buffer[OFFSET_PERIOD + sizeof(ushort)] = (byte)this.Time;
		BitConverter.Write(this.IsTimeRunning, buffer, OFFSET_TICKINGFLAG + sizeof(ushort));
		buffer[OFFSET_WEATHER + sizeof(ushort)] = (byte)this.Weather;
		BitConverter.LittleEndian.Write((uint)this.Theme, buffer, OFFSET_THEME + sizeof(ushort));
		buffer[OFFSET_DIFFICULTY + sizeof(ushort)] = (byte)this.Difficulty;
		BitConverter.LittleEndian.Write((uint)this.Planet, buffer, OFFSET_PLANET + sizeof(ushort));
		buffer[OFFSET_SEASON + sizeof(ushort)] = (byte)this.Season;
		buffer[OFFSET_GAMEMODE + sizeof(ushort)] = (byte)this.Gamemode;
		buffer[OFFSET_WORLDSIZETYPE + sizeof(ushort)] = (byte)this.WorldSizeType;
		buffer[OFFSET_SKYSIZETYPE + sizeof(ushort)] = (byte)this.SkySizeType;
		BitConverter.LittleEndian.Write(this.WorldSizeBytes, buffer, OFFSET_SIZEINBYTES + sizeof(ushort));
		return buffer;
	}
	/* Static Methods */
	public static WorldInfoResponseMessage Deserialize(ReadOnlySpan<byte> buffer)
	{
		(ushort, ushort) size = (
			BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_SIZE),
			BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_SIZE + sizeof(ushort))
		);
		(ushort, ushort) spawn = (
			BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_SPAWN),
			BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_SPAWN + sizeof(ushort))
		);
		(ushort, ushort) player = (
			BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_PLAYER),
			BitConverter.LittleEndian.GetUInt16(buffer, OFFSET_PLAYER + sizeof(ushort))
		);
		uint ticks = BitConverter.LittleEndian.GetUInt32(buffer, OFFSET_TICKS);
		var period = (Period)buffer[OFFSET_PERIOD];
		var isRunning = BitConverter.GetBool(buffer, OFFSET_TICKINGFLAG);
		var weather = (Weather)buffer[OFFSET_WEATHER];
		var theme = (Planet)BitConverter.LittleEndian.GetUInt32(buffer, OFFSET_THEME);
		var difficulty = (Difficulty)buffer[OFFSET_DIFFICULTY];
		var planet = (Planet)BitConverter.LittleEndian.GetUInt32(buffer, OFFSET_PLANET);
		var season = (Season)buffer[OFFSET_DIFFICULTY];
		var gamemode = (Gamemode)buffer[OFFSET_DIFFICULTY];
		var worldSizeType = (SizeType)buffer[OFFSET_WORLDSIZETYPE];
		var skySizeType = (SizeType)buffer[OFFSET_SKYSIZETYPE];
		// UNKNOWN (Start: 32, Length: 4)
		var sizeInBytes = BitConverter.LittleEndian.GetUInt32(buffer, OFFSET_SIZEINBYTES);
		return new WorldInfoResponseMessage(
			size, spawn, player, ticks, period, isRunning, weather, theme, difficulty,
			planet, season, gamemode, worldSizeType, skySizeType, sizeInBytes
		);
	}
	/* Properties */
	public readonly (ushort Width, ushort Height) Size;
	public readonly (ushort X, ushort Y) Spawn;
	public readonly (ushort X, ushort Y) Player;
	public readonly uint Ticks;
	public readonly Period Time;
	public readonly bool IsTimeRunning;
	public readonly Weather Weather;
	public readonly Planet Theme;
	public readonly Difficulty Difficulty;
	public readonly Planet Planet;
	public readonly Season Season;
	public readonly Gamemode Gamemode;
	public readonly SizeType WorldSizeType;
	public readonly SizeType SkySizeType;
	public readonly uint WorldSizeBytes;
	/* Class Properties */
	private const byte SIZE                 = 40;
	private const byte OFFSET_SIZE          =  0;
	private const byte OFFSET_SPAWN         =  4;
	private const byte OFFSET_PLAYER        =  8;
	private const byte OFFSET_TICKS         = 12;
	private const byte OFFSET_PERIOD        = 16;
	private const byte OFFSET_TICKINGFLAG   = 17;
	private const byte OFFSET_WEATHER       = 18;
	private const byte OFFSET_THEME         = 19;
	private const byte OFFSET_DIFFICULTY    = 23;
	private const byte OFFSET_PLANET        = 24;
	private const byte OFFSET_SEASON        = 28;
	private const byte OFFSET_GAMEMODE      = 29;
	private const byte OFFSET_WORLDSIZETYPE = 30;
	private const byte OFFSET_SKYSIZETYPE   = 31;
	private const byte OFFSET_SIZEINBYTES   = 36;
}

public sealed class WorldSkylineResponseMessage
{
	/* Constructor */
	public WorldSkylineResponseMessage(ushort[] skyline) { this.Skyline = skyline; }
	/* Instance Methods */
	public byte[] Serialize()
	{
		using var compressedStream = new MemoryStream();
		var buffer = new byte[this.Skyline.Length * sizeof(ushort)];
		// Write skyline to array
		for (var i = 0; i < this.Skyline.Length; ++i)
			BitConverter.LittleEndian.Write(this.Skyline[i], buffer, i * sizeof(ushort));
		// Compress skyline array
		using var decompressedStream = new MemoryStream(buffer);
		using (var compressionStream = new GZipStream(compressedStream, CompressionLevel.Optimal, true))
			decompressedStream.CopyTo(compressionStream);
		// Write header
		buffer = new byte[compressedStream.Length + sizeof(ushort)];
		BitConverter.BigEndian.Write((ushort)MessageHeader.WorldSkylineResponse, buffer);
		// Copy compressed skyline
		Array.Copy(compressedStream.GetBuffer(), 0, buffer, sizeof(ushort), compressedStream.Length);
		return buffer;
	}
	/* Static Methods */
	public unsafe static WorldSkylineResponseMessage Deserialize(ReadOnlySpan<byte> buffer)
	{
		using var decompressedStream = new MemoryStream();
		// Decompress skyline
		fixed (byte* bufferPin = buffer)
		{
			using var compressedStream = new UnmanagedMemoryStream(bufferPin, buffer.Length);
			using var decompressionStream = new GZipStream(compressedStream, CompressionMode.Decompress);
			decompressionStream.CopyTo(decompressedStream);
		}
		var decompressedSkyline = decompressedStream.GetBuffer();
		// Read skyline from array
		var skyline = new ushort[decompressedSkyline.Length / sizeof(ushort)];
		for (var i = 0; i < skyline.Length; ++i)
			skyline[i] = BitConverter.LittleEndian.GetUInt16(decompressedSkyline, i * sizeof(ushort));
		return new WorldSkylineResponseMessage(skyline);
	}
	/* Properties */
	public readonly ushort[] Skyline;
}

public sealed class WorldBlocksResponseMessage
{
	/* Constructor */
	public WorldBlocksResponseMessage(byte[] data) { this.Data = data; }
	/* Instance Methods */
	public byte[] Serialize()
	{
		var buffer = new byte[this.Data.Length + sizeof(uint) + sizeof(ushort)];
		BitConverter.BigEndian.Write((ushort)MessageHeader.WorldBlocksResponse, buffer);
		BitConverter.LittleEndian.Write((uint)this.Data.Length, buffer, sizeof(ushort));
		Array.Copy(this.Data, 0, buffer, sizeof(uint) + sizeof(ushort), this.Data.Length);
		return buffer;
	}
	/* Static Methods */
	public static WorldBlocksResponseMessage Deserialize(ReadOnlySpan<byte> buffer)
	{
		var bufferSize = BitConverter.LittleEndian.GetUInt32(buffer);
		var data = buffer.Slice(sizeof(uint), (int)bufferSize).ToArray();
		return new WorldBlocksResponseMessage(data);
	}
	/* Properties */
	public readonly byte[] Data;
}
