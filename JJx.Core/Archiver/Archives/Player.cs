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
	private void _LoadItems()
	{
		if (this._AreItemsLoaded) return;
		using (var inventoryChunk = this._Reader!.GetChunkStream(ArchiverChunkType.PlayerItems))
		{
			var reader = new JJxReader(inventoryChunk);
			var items = new Item[Player.COUNTOF_ITEMS];
			for (var i = 0; i < items.Length; ++i)
				items[i] = reader.ReadObject<Item>();
			this._Player._Items = items;
		}
		this._AreItemsLoaded = true;
	}
	private void _LoadStatus()
	{
		if (this._IsStatusLoaded) return;
		using (var inventoryChunk = this._Reader!.GetChunkStream(ArchiverChunkType.PlayerStatus))
		{
			var reader = new JJxReader(inventoryChunk);
			this._Player.Health = reader.ReadFloat32() * 10.0f;
			var effects = new Effect[Player.COUNTOF_EFFECTS];
			for (var i = 0; i < effects.Length; ++i)
				effects[i] = reader.ReadObject<Effect>();
			this._Player._Effects = effects;
		}
		this._IsStatusLoaded = true;
	}
	/* Static Methods */
	internal static PlayerArchive Load(IArchiveReader reader, bool eagerLoad = false)
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
		var archive = new PlayerArchive(player, reader);
		if (eagerLoad)
		{
			archive._LoadItems();
			archive._LoadStatus();
		}
		return archive;
	}
	/* Properties */
	public bool IsFullyLoaded => this._AreItemsLoaded && this._IsStatusLoaded;
	private readonly IArchiveReader? _Reader;
	internal readonly Player _Player;
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
	// Status
	private bool _IsStatusLoaded = false;
	public float Health {
		get { this._LoadStatus(); return this._Player.Health; }
		set { this._LoadStatus(); this._Player.Health = value; }
	}
	public Span<Effect> Effects { get { this._LoadStatus(); return this._Player.Effects; } }
}
