/*
	Junk Jack X: Core
	- [Archive]Player

	Written By: Ryan Smith
*/

using System;
using System.IO;
using JJx.Core.Serialization;

namespace JJx.Core;

public sealed class PlayerArchive : IArchive
{
	/* Constructor */
	public PlayerArchive(Player player) :this(player, null)
	{
		this._AreItemsLoaded = true;
		this._IsCraftbookLoaded = true;
		this._AreAchievementsLoaded = true;
		this._IsStatusLoaded = true;
	}
	private PlayerArchive(Player player, IArchiveReader? reader)
	{
		this._Player = player;
		this._Reader = reader;
	}
	/* Instance Methods */
	public void Dispose() => this._Reader?.Dispose();
	// Reading
	private void _Load()
	{
		this._LoadItems();
		this._LoadCraftbook();
		this._LoadAchievements();
		this._LoadStatus();
		this._Reader!.Dispose();
	}
	private void _LoadItems()
	{
		if (this._AreItemsLoaded) return;
		using (var chunk = this._Reader!.GetChunkStream(ArchiverChunkType.PlayerItems))
		{
			var reader = new JJxReader(chunk);
			foreach (ref var item in this._Player.Items)
				item = reader.ReadObject<Item>();
		}
		this._AreItemsLoaded = true;
	}
	private void _LoadCraftbook()
	{
		if (this._IsCraftbookLoaded) return;
		using (var chunk = this._Reader!.GetChunkStream(ArchiverChunkType.PlayerCraftbooks))
		{
			var reader = new JJxReader(chunk);
			reader.ReadSpan(this._Player.Craftbook);
		}
		this._IsCraftbookLoaded = true;
	}
	private void _LoadAchievements()
	{
		if (this._AreAchievementsLoaded) return;
		using (var chunk = this._Reader!.GetChunkStream(ArchiverChunkType.PlayerAchievements))
		{
			var reader = new JJxReader(chunk);
			reader.ReadSpan(this._Player.Achievements);
		}
		this._AreAchievementsLoaded = true;
	}
	private void _LoadStatus()
	{
		if (this._IsStatusLoaded) return;
		using (var chunk = this._Reader!.GetChunkStream(ArchiverChunkType.PlayerStatus))
		{
			var reader = new JJxReader(chunk);
			this._Player.Health = reader.ReadFloat32() * 10.0f;
			foreach (ref var effect in this._Player.Effects)
				effect = reader.ReadObject<Effect>();
		}
		this._IsStatusLoaded = true;
	}
	// Writing
	public void Write(IArchiveWriter writer)
	{
		if (!this.IsFullyLoaded) this._Load();
		Stream chunk;
		// Info
		chunk = writer.WriteChunk(ArchiverChunkType.PlayerInfo);
		{
			var streamWriter = new JJxWriter(chunk);
			streamWriter.Write(this.Id);
			streamWriter.Write(this.Name, SIZEOF_NAME);
			streamWriter.Write(this.Version);
			streamWriter.Write(this.UnlockedPlanets);
			streamWriter.Write(this.Rules.Flags);
			streamWriter.Write(this.Appearance.Pack());
			streamWriter.Skip(2); // Unknown
			streamWriter.Write(this.Rules.Difficulty);
			streamWriter.Skip(3); // Unknown
		}
		// Items
		chunk = writer.WriteChunk(ArchiverChunkType.PlayerItems);
		{
			var streamWriter = new JJxWriter(chunk);
			foreach (ref var item in this.Items)
				streamWriter.Write(item);
		}
		// Craftbook
		chunk = writer.WriteChunk(ArchiverChunkType.PlayerCraftbooks);
		{
			var streamWriter = new JJxWriter(chunk);
			foreach (var data in this.Craftbook)
				streamWriter.Write(data);
		}
		// Achievements
		chunk = writer.WriteChunk(ArchiverChunkType.PlayerAchievements, version: 1);
		{
			var streamWriter = new JJxWriter(chunk);
			foreach (var data in this.Achievements)
				streamWriter.Write(data);
		}
		// Status
		chunk = writer.WriteChunk(ArchiverChunkType.PlayerStatus);
		{
			var streamWriter = new JJxWriter(chunk);
			streamWriter.Write(this.Health / 10.0f);
			foreach (ref var effect in this.Effects)
				streamWriter.Write(effect);
		}
	}
	/* Static Methods */
	internal static PlayerArchive Load(IArchiveReader reader, bool eagerLoad = false)
	{
		Player player;
		using (var chunk = reader.GetChunkStream(ArchiverChunkType.PlayerInfo))
		{
			var streamReader = new JJxReader(chunk);
			var guid = streamReader.ReadObject<Guid>();
			var name = streamReader.ReadString(SIZEOF_NAME);
			var version = streamReader.ReadObject<Version>();
			var unlockedPlanets = streamReader.ReadObject<Planet>();
			var ruleFlags = streamReader.ReadObject<Ruleset.GameplayOptions>();
			var appearance = CharacterAppearance.Unpack(streamReader.ReadUInt16());
			streamReader.Skip(2); // Unknown
			var difficulty = streamReader.ReadObject<Difficulty>();
			streamReader.Skip(3); // Unknown
			var ruleset = new Ruleset(difficulty, ruleFlags);
			player = new(guid, name, version, unlockedPlanets, appearance, ruleset);
		}
		var archive = new PlayerArchive(player, reader);
		if (eagerLoad) archive._Load();
		return archive;
	}
	/* Properties */
	private readonly IArchiveReader? _Reader;
	internal readonly Player _Player;
	internal bool IsFullyLoaded => this._AreItemsLoaded &&
								   this._IsCraftbookLoaded &&
								   this._AreAchievementsLoaded &&
								   this._IsStatusLoaded;
	// Info
	public Guid Id => this._Player.Id;
	public Version Version => this._Player.Version;
	public string Name { get => this._Player.Name; set => this._Player.Name = value; }
	public Planet UnlockedPlanets { get => this._Player.UnlockedPlanets; set => this._Player.UnlockedPlanets = value; }
	public CharacterAppearance Appearance => this._Player.Appearance;
	public Ruleset Rules => this._Player.Rules;
	// Inventory
	private bool _AreItemsLoaded = false;
	public Span<Item> Items { get { this._LoadItems(); return this._Player.Items; } }
	public Span<Item> SurvivalHotbar { get { this._LoadItems(); return this._Player.SurvivalHotbar; } }
	public Span<Item> CreativeHotbar { get { this._LoadItems(); return this._Player.CreativeHotbar; } }
	public Span<Item> CraftingSlots { get { this._LoadItems(); return this._Player.CraftingSlots; } }
	public Span<Item> Inventory { get { this._LoadItems(); return this._Player.Inventory; } }
	public Span<Item> ArmorActual { get { this._LoadItems(); return this._Player.ArmorActual; } }
	public Span<Item> ArmorVisual { get { this._LoadItems(); return this._Player.ArmorVisual; } }
	public ref Item CraftSlot { get { this._LoadItems(); return ref this._Player.CraftSlot; } }
	public ref Item ArrowSlot { get { this._LoadItems(); return ref this._Player.ArrowSlot; } }
	// Craftbook
	private bool _IsCraftbookLoaded = false;
	public Span<byte> Craftbook { get { this._LoadCraftbook(); return this._Player.Craftbook; } }
	// Achievements
	private bool _AreAchievementsLoaded = false;
	public Span<byte> Achievements { get { this._LoadAchievements(); return this._Player.Achievements; } }
	// Status
	private bool _IsStatusLoaded = false;
	public float Health {
		get { this._LoadStatus(); return this._Player.Health; }
		set { this._LoadStatus(); this._Player.Health = value; }
	}
	public Span<Effect> Effects { get { this._LoadStatus(); return this._Player.Effects; } }
	/* Class Properties */
	internal const int SIZEOF_NAME = 16;
}
