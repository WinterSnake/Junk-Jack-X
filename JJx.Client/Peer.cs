/*
	Junk Jack X: Client
	- Peer

	Written By: Ryan Smith
*/

using System.Buffers;
using ENet.Managed;
using JJx.Protocol;
using JJx.Protocol.Packets;

public sealed class JJxClientPeer
{
	/* Constructor */
	public JJxClientPeer(byte id, ENetPeer peer)
	{
		this.Id = id;
		this._Peer = peer;
	}
	/* Instance Methods */
	public void Send(JJxPacket packet, byte channel = 0)
	{
		JJxPacketSerializer.Serialize(packet, this._Buffer, JJxPacketRegistry.Client);
		this._Peer.Send(channel, this._Buffer.WrittenSpan, ENetPacketFlags.Reliable);
		this._Buffer.Clear();
	}
	public void Disconnect(uint data = 0) =>  this._Peer.Disconnect(data);
	/* Properties */
	public byte Id;
	private readonly ENetPeer _Peer;
	private readonly ArrayBufferWriter<byte> _Buffer = new();
}
