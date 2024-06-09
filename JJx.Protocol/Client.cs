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
	}
	/* Instance Methods */
	public void Connect(IPEndPoint address)
	{
		this.Peer = this._Host.Connect(address, Client.CHANNEL_COUNT, 0);
	}
	// Events
	public override void OnConnect(ENetPeer peer)
	{
	}
	public override void OnDisconnect(ENetPeer peer)
	{
	}
	/* Properties */
	public readonly Player Player;
	protected ENetPeer Peer { get; private set; }
	/* Class Properties */
	private const int PEER_COUNT = 1;
	private const byte CHANNEL_COUNT = 1;
}
