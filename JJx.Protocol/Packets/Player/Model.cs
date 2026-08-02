/*
	Junk Jack X: Protocol
	- [Packet::Player]Model

	Written By: Ryan Smith
*/

using System;
using JJx.Core;
using JJx.Core.Serialization;

namespace JJx.Protocol.Packets;

[PacketOpcode(Opcode=JJxPacketOpcode.PlayerUpdateModel)]
public sealed class PlayerUpdateModelPacket : JJxPacket
{
	/* Constructor */
	public PlayerUpdateModelPacket(byte id, CharacterModel model)
	{
		this.Id = id;
		this.Model = model;
	}
	private PlayerUpdateModelPacket(CharacterModel model, byte id): this(id, model) { }
	/* Static Methods */
	internal static PlayerUpdateModelPacket Deserialize(ref JJxReader reader) => new(
		reader.ReadObject<CharacterModel>(JJxPacketRegistry.Default),
		reader.ReadUInt8()
	);
	internal static void Serialize(PlayerUpdateModelPacket packet, JJxWriter writer)
	{
		writer.Write(packet.Model, JJxPacketRegistry.Default);
		writer.Write(packet.Id);
	}
	/* Properties */
	public readonly byte Id;
	public readonly CharacterModel Model;
}

[PacketOpcode(Opcode=JJxPacketOpcode.PlayerUpdateItem)]
public sealed class PlayerUpdateItemPacket : JJxPacket
{
	/* Constructor */
	public PlayerUpdateItemPacket(byte id, Item item, sbyte slot = 0)
	{
		this.Id = id;
		this.Item = item;
		this.Slot = slot;
	}
	private PlayerUpdateItemPacket(Item item, sbyte slot, byte id): this(id, item, slot) { }
	/* Static Methods */
	internal static PlayerUpdateItemPacket Deserialize(ref JJxReader reader) => new(
		reader.ReadObject<Item>(JJxPacketRegistry.Default),
		reader.ReadInt8(),
		reader.ReadUInt8()
	);
	internal static void Serialize(PlayerUpdateItemPacket packet, JJxWriter writer)
	{
		writer.Write(packet.Item, JJxPacketRegistry.Default);
		writer.Write(packet.Slot);
		writer.Write(packet.Id);
	}
	/* Properties */
	public readonly byte Id;
	public readonly sbyte Slot;
	public readonly Item Item;
}

[PacketOpcode(Opcode=JJxPacketOpcode.PlayerUpdateEquipment)]
public sealed class PlayerUpdateEquipmentPacket : JJxPacket
{
	/* Constructor */
	public PlayerUpdateEquipmentPacket(byte id, sbyte slot, bool isVisual, Item item)
	{
		this.Id = id;
		this.Slot = slot;
		this.IsVisual = isVisual;
		this.Item = item;
	}
	private PlayerUpdateEquipmentPacket(Item item, bool isVisual, sbyte slot, byte id): this(id, slot, isVisual, item) { }
	/* Static Methods */
	internal static PlayerUpdateEquipmentPacket Deserialize(ref JJxReader reader) => new(
		reader.ReadObject<Item>(JJxPacketRegistry.Default),
		reader.ReadBool(),
		reader.ReadInt8(),
		reader.ReadUInt8()
	);
	internal static void Serialize(PlayerUpdateEquipmentPacket packet, JJxWriter writer)
	{
		writer.Write(packet.Item, JJxPacketRegistry.Default);
		writer.Write(packet.IsVisual);
		writer.Write(packet.Slot);
		writer.Write(packet.Id);
	}
	/* Properties */
	public readonly byte Id;
	public readonly sbyte Slot;
	public readonly bool IsVisual;
	public readonly Item Item;
}
