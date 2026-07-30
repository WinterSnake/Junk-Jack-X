/*
	Junk Jack X: Protocol
	- [Packet] Registry

	Written By: Ryan Smith
*/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using JJx.Core.Serialization;
using JJx.Protocol.Metadata;

namespace JJx.Protocol.Packets;

internal delegate TPacket DeserializeFunc<out TPacket>(ref JJxReader reader) where TPacket: JJxPacket;

public sealed class JJxPacketRegistry
{
	/* Constructors */
	public JJxPacketRegistry()
	{
		this._Serializers = new();
		this._Deserializers = new();
	}
	/* Instance Methods */
	internal void RegisterSerializer<T>(Action<T, JJxWriter> serializeFunc)
		where T: JJxPacket
	{
		var packetType = typeof(T);
		this._Serializers.Add(
			packetType, (
				_GetOpcodeAttr(packetType),
				(packet, writer) => serializeFunc((T)packet, writer)
			)
		);
	}
	internal void RegisterDeserializer<T>(DeserializeFunc<T> deserializeFunc)
		where T: JJxPacket
	{
		var packetType = typeof(T);
		this._Deserializers.Add(_GetOpcodeAttr(packetType), deserializeFunc);
	}
	internal bool TryGetSerializer(Type type, [NotNullWhen(true)]out (JJxPacketOpcode Opcode, Action<JJxPacket, JJxWriter> SerializeFunc) packetInfo)
	{
		packetInfo = default;
		return this._Serializers.TryGetValue(type, out packetInfo);
	}
	internal bool TryGetDeserializer(JJxPacketOpcode opcode, [NotNullWhen(true)]out DeserializeFunc<JJxPacket>? deserializeFunc)
	{
		deserializeFunc = default;
		return this._Deserializers.TryGetValue(opcode, out deserializeFunc);
	}
	// Helpers
	private JJxPacketOpcode _GetOpcodeAttr(Type packetType)
		=> packetType.GetCustomAttribute<PacketOpcodeAttribute>()!.Opcode;
	/* Properties */
	private readonly Dictionary<Type, (JJxPacketOpcode Opcode, Action<JJxPacket, JJxWriter> SerializeFunc)> _Serializers;
	private readonly Dictionary<JJxPacketOpcode, DeserializeFunc<JJxPacket>> _Deserializers;
	/* Class Properties */
	public static JJxPacketRegistry Client => JJxClientRegistry.Registry;
	public static JJxPacketRegistry Server => JJxServerRegistry.Registry;
}
