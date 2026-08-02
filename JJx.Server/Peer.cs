/*
	Junk Jack X: Server
	- Peer

	Written By: Ryan Smith
*/

using System;
using System.Buffers;
using ENet.Managed;
using JJx.Core;
using JJx.Protocol;
using JJx.Protocol.Packets;

public sealed class JJxServerPeer
{
	/* Constructor */
	private JJxServerPeer(byte id, ENetPeer peer, ArrayBufferWriter<byte> buffer)
	{
		this.Id = id;
		this._Peer = peer;
		this._Buffer = buffer;
	}
	/* Instance Methods */
	public void Send(JJxPacket packet, byte channel = 0)
	{
		JJxPacketSerializer.Serialize(packet, this._Buffer, JJxPacketRegistry.Server);
		this._Peer.Send(channel, this._Buffer.WrittenSpan, ENetPacketFlags.Reliable);
		this._Buffer.Clear();
	}
	/* Properties */
	public readonly byte Id;
	private readonly ENetPeer _Peer;
	private readonly ArrayBufferWriter<byte> _Buffer;
	/* Sub-Classes */
	public sealed class Builder
	{
		/* Constructor */
		public Builder(byte id, ENetPeer peer)
		{
			this.Id = id;
			this._Peer = peer;
		}
		/* Instance Methods */
		public JJxServerPeer Build()
		{
			return new(this.Id, this._Peer, this._Buffer);
		}
		public bool IsSelf(ENetPeer peer) => this._Peer == peer;
		public void Send(JJxPacket packet, byte channel = 0)
		{
			JJxPacketSerializer.Serialize(packet, this._Buffer, JJxPacketRegistry.Server);
			this._Peer.Send(channel, this._Buffer.WrittenSpan, ENetPacketFlags.Reliable);
			this._Buffer.Clear();
		}
		/* Properties */
		public byte Id { get; set; }
		public float Progress;
		public string Name = String.Empty;
		public JJxVersion Version;
		private readonly ENetPeer _Peer;
		private readonly ArrayBufferWriter<byte> _Buffer = new();
	}
}
