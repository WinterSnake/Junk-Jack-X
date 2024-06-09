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
	}
	public override void OnDisconnect(ENetPeer peer)
	{
	}
	public override void OnClientInfo(ClientInfo info)
	{
		Console.WriteLine(info);
	}
	/* Properties */
	public event Action<ClientInfo> OnClientInfoEvent;
	/* Class Properties */
	private const byte CHANNEL_COUNT = 16;
}
