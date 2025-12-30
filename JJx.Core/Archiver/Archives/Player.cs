/*
	Junk Jack X: Core
	- [Archive]Player

	Written By: Ryan Smith
*/

using System;
using JJx.Core.Serialization;

namespace JJx.Core;

public sealed class PlayerArchive : IArchive
{
	/* Constructor */
	public PlayerArchive(Player player)
		:this(player, null) { }
	private PlayerArchive(Player player, IArchiveReader? reader)
	{
		this._Player = player;
		this._Reader = reader;
	}
	/* Instance Methods */
	public void Dispose() => this._Reader?.Dispose();
	/* Static Methods */
	internal static PlayerArchive Load(IArchiveReader reader)
	{
		Player player;
		using (var infoChunk = reader.GetChunkStream(ArchiverChunkType.PlayerInfo))
		{
			var streamReader = new JJxReader(infoChunk);
			var guid = streamReader.ReadObject<Guid>();
			var name = streamReader.ReadString(16);
			var version = streamReader.ReadObject<Version>();
			var unlockedPlanets = streamReader.ReadObject<Planet>();
			var ruleFlags = streamReader.ReadObject<Ruleset.GameplayOptions>();
			var appearance = CharacterAppearance.Unpack(streamReader.ReadUInt16());
			streamReader.Skip(2); // Unknown
			var difficulty = streamReader.ReadObject<Difficulty>();
			var ruleset = new Ruleset(difficulty, ruleFlags);
			streamReader.Skip(3); // Unknown
			player = new(guid, name, version, unlockedPlanets, appearance, ruleset);
		}
		return new(player, reader);
	}
	/* Properties */
	private readonly IArchiveReader? _Reader;
	private readonly Player _Player;
}
