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
		var items = new Item[Player.COUNTOF_ITEMS];
		using (var inventoryChunk = this._Reader!.GetChunkStream(ArchiverChunkType.PlayerItems))
		{
			var reader = new JJxReader(inventoryChunk);
			for (var i = 0; i < items.Length; ++i)
				items[i] = reader.ReadObject<Item>();
		}
		this._Player.Items = items;
		this._AreItemsLoaded = true;
	}
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
	// Info
	public Guid Id => this._Player.Id;
	public Version Version => this._Player.Version;
	public string Name { get => this._Player.Name; set => this._Player.Name = value; }
	public Planet UnlockedPlanets { get => this._Player.UnlockedPlanets; set => this._Player.UnlockedPlanets = value; }
	public CharacterAppearance Appearance => this._Player.Appearance;
	public Ruleset Rules => this._Player.Rules;
	// Inventory
	private bool _AreItemsLoaded = false;
	public Item[] Items { get { this._LoadItems(); return this._Player.Items; } }
	public Span<Item> SurvivalHotbar { get { this._LoadItems(); return this._Player.SurvivalHotbar; } }
	public Span<Item> CreativeHotbar { get { this._LoadItems(); return this._Player.CreativeHotbar; } }
	public Span<Item> CraftingSlots { get { this._LoadItems(); return this._Player.CraftingSlots; } }
	public Span<Item> Inventory { get { this._LoadItems(); return this._Player.Inventory; } }
	public Span<Item> ArmorActual { get { this._LoadItems(); return this._Player.ArmorActual; } }
	public Span<Item> ArmorVisual { get { this._LoadItems(); return this._Player.ArmorVisual; } }
	public ref Item CraftSlot { get { this._LoadItems(); return ref this._Player.CraftSlot; } }
	public ref Item ArrowSlot { get { this._LoadItems(); return ref this._Player.ArrowSlot; } }
}
