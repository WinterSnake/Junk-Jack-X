/*
	Junk Jack X: Protocol
	- [Connection]Server

	Written By: Ryan Smith
*/
using System;
using System.IO;
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
	protected override void OnLoginRequest(ENetPeer peer, LoginRequestMessage info)
	{
		if (info.Version != this.World.Version)
		{
			this.DeclineLogin(peer, LoginFailureReason.IncorrectVersion);
			return;
		}
		this.AcceptLogin(peer);
	}
	protected override void OnListRequest(ENetPeer peer)
	{
		Console.WriteLine($"Player[{peer.GetRemoteEndPoint()}] is requesting peer list");
	}
	protected override void OnWorldRequest(ENetPeer peer)
	{
		using var blockStream = new MemoryStream();
		this.World.TileMap.ToStream(blockStream).Wait();
		var blockBuffer = blockStream.GetBuffer();
		// Info
		var infoResponse = new WorldInfoResponseMessage(
			this.World.Size, this.World.Spawn, this.World.Player, this.World.Ticks, this.World.Time,
			this.IsTimeRunning, this.World.Weather, this.World.Planet, this.Difficulty, this.World.Planet,
			this.World.Season, this.World.Gamemode, this.World.WorldSizeType, this.World.SkySizeType, (uint)blockBuffer.Length
		);

		peer.Send(channelId: 0, infoResponse.Serialize(), ENetPacketFlags.Reliable);
		// Skyline
		var skylineResponse = new WorldSkylineResponseMessage(this.World.Skyline);
		peer.Send(channelId: 0, skylineResponse.Serialize(), ENetPacketFlags.Reliable);
		// Blocks
		var bytesSent = 0;
		do
		{
			var blockInternalBuffer = new byte[MAX_CHUNK_SIZE < blockBuffer.Length - bytesSent ? MAX_CHUNK_SIZE : blockBuffer.Length - bytesSent];
			Array.Copy(blockInternalBuffer, 0, blockBuffer, bytesSent, blockInternalBuffer.Length);
			var blockResponse = new WorldBlocksResponseMessage(blockInternalBuffer);
			peer.Send(channelId: 0, blockResponse.Serialize(), ENetPacketFlags.Reliable);
			bytesSent += blockInternalBuffer.Length;
		} while(bytesSent < blockBuffer.Length);
	}
	protected override void OnWorldProgress(ENetPeer peer, WorldProgressMessage worldProgress)
	{
		Console.WriteLine($"Player[{peer.GetRemoteEndPoint()}] is at {worldProgress.Percent * 100.0f}%");
	}
	/* Properties */
	public Difficulty Difficulty = Difficulty.Normal;
	public bool IsTimeRunning = true;
	public readonly World World;
	/* Class Properties */
	private const byte CHANNEL_COUNT = 16;
	private const ushort MAX_CHUNK_SIZE = 1024;
}
