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
		var loginResponse = new LoginResponse();
		Console.WriteLine(System.BitConverter.ToString(loginResponse.Serialize()));
		peer.Send(channelId: 0, loginResponse.Serialize(), ENetPacketFlags.Reliable);
		Console.WriteLine($"Accepting peer @{peer.GetRemoteEndPoint()}");
	}
	protected void DeclineLogin(ENetPeer peer, LoginFailureReason reason)
	{
		Console.WriteLine($"Declining peer @{peer.GetRemoteEndPoint()} for: {reason}");
		var loginResponse = new LoginResponse(reason);
		peer.Send(channelId: 0, loginResponse.Serialize(), ENetPacketFlags.Reliable);
	}
	// Events
	public override void OnLoginAttempt(ENetPeer peer, ClientInfo info)
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
