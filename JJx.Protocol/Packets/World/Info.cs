/*
	Junk Jack X: Protocol
	- [Packet::World]Info

	Written By: Ryan Smith
*/

using System;
using JJx.Core;
using JJx.Core.Serialization;

namespace JJx.Protocol.Packets;

[PacketOpcode(Opcode=JJxPacketOpcode.WorldInfoResponse)]
public sealed class WorldInfoResponsePacket : JJxPacket
{
	/* Constructor */
	public WorldInfoResponsePacket(
		(ushort, ushort) size, (ushort, ushort) spawn, (ushort, ushort) player,
		uint ticks, DayPhase dayPhase, bool isTimeTicking, Weather weather, Planet theme,
		Difficulty difficulty, Planet planet, Season season, Gamemode gamemode,
		MapBounds sizeBounds, MapBounds skyBounds, uint unknown, uint worldSizeInBytes
	)
	{
		this.Size = size;
		this.Spawn = spawn;
		this.Player = player;
		this.Ticks = ticks;
		this.DayPhase = dayPhase;
		this.IsTimeTicking = isTimeTicking;
		this.Weather = weather;
		this.Theme = theme;
		this.Difficulty = difficulty;
		this.Planet = planet;
		this.Season = season;
		this.Gamemode = gamemode;
		this.SizeBounds = sizeBounds;
		this.SkyBounds = skyBounds;
		this.Unknown = unknown;
		this.WorldSizeInBytes = worldSizeInBytes;
	}
	/* Static Methods */
	internal static WorldInfoResponsePacket Deserialize(ref JJxReader reader) => new(
		(reader.ReadUInt16(), reader.ReadUInt16()),
		(reader.ReadUInt16(), reader.ReadUInt16()),
		(reader.ReadUInt16(), reader.ReadUInt16()),
		reader.ReadUInt32(),
		reader.ReadObject<DayPhase>(JJxPacketRegistry.Default),
		reader.ReadBool(),
		reader.ReadObject<Weather>(JJxPacketRegistry.Default),
		reader.ReadObject<Planet>(JJxPacketRegistry.Default),
		reader.ReadObject<Difficulty>(JJxPacketRegistry.Default),
		reader.ReadObject<Planet>(JJxPacketRegistry.Default),
		reader.ReadObject<Season>(JJxPacketRegistry.Default),
		reader.ReadObject<Gamemode>(JJxPacketRegistry.Default),
		reader.ReadObject<MapBounds>(JJxPacketRegistry.Default),
		reader.ReadObject<MapBounds>(JJxPacketRegistry.Default),
		reader.ReadUInt32(),
		reader.ReadUInt32()
	);
	internal static void Serialize(WorldInfoResponsePacket packet, JJxWriter writer)
	{
		writer.Write(packet.Size.Width);
		writer.Write(packet.Size.Height);
		writer.Write(packet.Spawn.X);
		writer.Write(packet.Spawn.Y);
		writer.Write(packet.Player.X);
		writer.Write(packet.Player.Y);
		writer.Write(packet.Ticks);
		writer.Write(packet.DayPhase, JJxPacketRegistry.Default);
		writer.Write(packet.IsTimeTicking);
		writer.Write(packet.Weather, JJxPacketRegistry.Default);
		writer.Write(packet.Theme, JJxPacketRegistry.Default);
		writer.Write(packet.Difficulty, JJxPacketRegistry.Default);
		writer.Write(packet.Planet, JJxPacketRegistry.Default);
		writer.Write(packet.Season, JJxPacketRegistry.Default);
		writer.Write(packet.Gamemode, JJxPacketRegistry.Default);
		writer.Write(packet.SizeBounds, JJxPacketRegistry.Default);
		writer.Write(packet.SkyBounds, JJxPacketRegistry.Default);
		writer.Write(packet.Unknown);
		writer.Write(packet.WorldSizeInBytes);
	}
	/* Properties */
	public readonly (ushort Width, ushort Height) Size;
	public readonly (ushort X, ushort Y) Spawn;
	public readonly (ushort X, ushort Y) Player;
	public readonly uint Ticks;
	public readonly DayPhase DayPhase;
	public readonly bool IsTimeTicking;
	public readonly Weather Weather;
	public readonly Planet Theme;
	public readonly Difficulty Difficulty;
	public readonly Planet Planet;
	public readonly Season Season;
	public readonly Gamemode Gamemode;
	public readonly MapBounds SizeBounds;
	public readonly MapBounds SkyBounds;
	public readonly uint Unknown;
	public readonly uint WorldSizeInBytes;
	/* Class Properties */
	public const uint UNKNOWN = 0x7FFD6C78;
}
