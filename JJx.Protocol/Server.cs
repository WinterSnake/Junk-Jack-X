/*
	Junk Jack X: Protocol
	- Connection:Server

	Written By: Ryan Smith
*/
using System;
using System.Net;
using ENet.Managed;

namespace JJx.Protocol;

public class Server : Connection
{
	/* Constructor */
	public Server(IPEndPoint address, int maxPlayers): base(maxPlayers, Server.CHANNEL_COUNT, address)
	{

	}
	/* Instance Methods */
	// Events
	public override void OnConnect(ENetPeer peer)
	{
		Console.WriteLine($"Peer[Address]: {peer.GetRemoteEndPoint()}");
	}
	public override void OnDisconnect(ENetPeer peer)
	{
	}
	/* Class Properties */
	private const byte CHANNEL_COUNT = 16;
}
