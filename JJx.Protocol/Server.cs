/*
	Junk Jack X: Protocol
	- Connection:Server

	Written By: Ryan Smith
*/
using System;
using System.Net;
using ENet.Managed;
using JJx;

namespace JJx.Protocol;

public class Server : Connection
{
	/* Constructor */
	public Server(World world, IPEndPoint address, int maxPlayers): base(maxPlayers, CHANNEL_COUNT, address)
	{
		this.World = world;
	}
	/* Instance Methods */
	// Events
	public override void OnClientInfo(ENetPeer peer, ClientInfo info)
	{
		Console.WriteLine(info);
		if (info.Version == this.World.Version)
		{

		}
	}
	/* Properties */
	public readonly World World;
	/* Class Properties */
	private const byte CHANNEL_COUNT = 16;
}
