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
	protected void AcceptLogin(ENetPeer peer)
	{
		Console.WriteLine($"Accepting peer @{peer.GetRemoteEndPoint()}");
		var loginResponse = new LoginResponseMessage();
		peer.Send(channelId: 0, loginResponse.Serialize(), ENetPacketFlags.Reliable);
	}
	protected void DeclineLogin(ENetPeer peer, LoginFailureReason reason)
	{
		Console.WriteLine($"Declining peer @{peer.GetRemoteEndPoint()} for: {reason}");
		var loginResponse = new LoginResponseMessage(reason);
		peer.Send(channelId: 0, loginResponse.Serialize(), ENetPacketFlags.Reliable);
	}
	// Events
	public override void OnLoginRequest(ENetPeer peer, LoginRequestMessage info)
	{
		if (info.Version != this.World.Version)
		{
			this.DeclineLogin(peer, LoginFailureReason.IncorrectVersion);
			return;
		}
		this.AcceptLogin(peer);
	}
	/* Properties */
	public readonly World World;
	/* Class Properties */
	private const byte CHANNEL_COUNT = 16;
}
