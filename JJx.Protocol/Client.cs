/*
	Junk Jack X: Protocol
	- Connection:Client

	Written By: Ryan Smith
*/
using System;
using System.Net;
using ENet.Managed;
using JJx;

namespace JJx.Protocol;

public class Client : Connection
{
	/* Constructor */
	public Client(Player player): base(Client.PEER_COUNT, Client.CHANNEL_COUNT, null)
	{
		this.Player = player;
		this.Id = (byte)Random.Shared.Next(0, 255);
	}
	/* Instance Methods */
	public void Connect(IPEndPoint address)
	{
		this.Peer = this._Host.Connect(address, Client.CHANNEL_COUNT, 0);
	}
	// Events
	public override void OnConnect(ENetPeer peer)
	{
		// Send ClientInfo
		var clientInfo = new ClientInfo(this.Id, this.Player.Name, this.Player.Version);
		this.Peer.Send(channelId: 0, clientInfo.Serialize(), ENetPacketFlags.Reliable);
	}
	public override void OnDisconnect(ENetPeer peer)
	{
	}
	public override void OnClientAccepted() => Console.WriteLine("Client accepted..");
	/* Properties */
	public readonly Player Player;
	public readonly byte Id;
	protected ENetPeer Peer { get; private set; }
	/* Class Properties */
	private const int PEER_COUNT = 1;
	private const byte CHANNEL_COUNT = 1;
}
