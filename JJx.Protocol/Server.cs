/*
	Junk Jack X: Protocol
	- [Connection]Server

	Written By: Ryan Smith
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using ENet.Managed;
using JJx;

namespace JJx.Protocol;

public class Server
{
	/* Constructor */
	public Server(World world, IPEndPoint address, byte maxPlayers = 16)
	{
		this._Host = new ENetHost(address, maxPlayers, CHANNEL_COUNT);
		this.World = world;
		this.MaxPlayers = maxPlayers;
		this.Users = new List<User>(maxPlayers);
	}
	/* Instance Methods */
	public void ProcessEvent()
	{
		var @event = this._Host.Service(TimeSpan.FromMilliseconds(0));
		switch (@event.Type)
		{
			case ENetEventType.None: return;
			case ENetEventType.Connect: this.OnConnect(@event.Peer); break;
			case ENetEventType.Disconnect: this.OnDisconnect(@event.Peer); break;
			case ENetEventType.Receive:
			{
				this.ProcessMessage(@event);
				@event.Packet.Destroy();
			} break;
		}
	}
	protected void ProcessMessage(ENetEvent @event)
	{
		var header = (MessageHeader)((@event.Packet.Data[0] << 8) | (@event.Packet.Data[1] << 0));
		#if DEBUG
			Console.WriteLine($"Peer: {@event.Peer.GetRemoteEndPoint()} | Channel: {@event.ChannelId} | Flags: {@event.Packet.Flags} | Data: {@event.Data} | Size: {@event.Packet.Data.Length - 2} | Data={BitConverter.ToString(@event.Packet.Data.Slice(2))}");
		#endif
		switch (header)
		{
			// Management \\
			case MessageHeader.LoginRequest:
			{
				var request = LoginRequestMessage.Deserialize(@event.Packet.Data.Slice(2));
				this.OnLoginRequest(@event.Peer, request);
			} break;
			case MessageHeader.WorldRequest:
			{
				var request = WorldRequestMessage.Deserialize(@event.Packet.Data.Slice(2));
				this.OnWorldRequest(@event.Peer);
			} break;
			case MessageHeader.WorldProgress:
			{
				var status = WorldProgressMessage.Deserialize(@event.Packet.Data.Slice(2));
				this.OnWorldProgress(@event.Peer, status);
			} break;
			case MessageHeader.ListRequest:
			{
				var request = ListRequestMessage.Deserialize(@event.Packet.Data.Slice(2));
				this.OnListRequest(@event.Peer);
			} break;
			default:
			{
				Console.WriteLine($"Unknown header: 0x{(ushort)header:X4} | Size: {@event.Packet.Data.Length - 2} | Data: {BitConverter.ToString(@event.Packet.Data.Slice(2))}");
			} break;
		}
	}
	private User GetUserFromPeer(ENetPeer peer)
	{
		for (var i = 0; i < this.Users.Count; ++i)
			if (this.Users[i].Peer == peer)
				return this.Users[i];
		throw new ArgumentException($"Unknown peer {peer.GetRemoteEndPoint()} in users list");
	}
	// Events
	protected virtual void OnConnect(ENetPeer peer) { }
	protected virtual void OnDisconnect(ENetPeer peer)
	{
		// Remove peer from user list
		for (var i = 0; i < this.Users.Count; ++i)
			if (this.Users[i].Peer == peer)
				this.Users.Remove(this.Users[i]);
	}
	protected virtual void OnLoginRequest(ENetPeer peer, LoginRequestMessage info)
	{
		LoginResponseMessage response;
		if (info.Version != this.World.Version)
			response = new LoginResponseMessage(LoginFailureReason.IncorrectVersion);
		else if (this.Users.Count >= this.MaxPlayers)
			response = new LoginResponseMessage(LoginFailureReason.ServerFull);
		else
		{
			// Get user Id
			byte userId = 0;
			for (var i = 0; i < this.Users.Count; ++i)
				if (this.Users[i].Id != userId++)
					break;
			Console.WriteLine($"Request Id: 0x{info.Id:X2} | New user Id: 0x{userId:X2}, Name: \"{info.Name}\"");
			var user = new User(peer, userId, info.Name);
			this.Users.Add(user);
			response = new LoginResponseMessage();
		}
		peer.Send(channelId: 0, response.Serialize(), ENetPacketFlags.Reliable);
	}
	protected virtual void OnListRequest(ENetPeer peer)
	{
		foreach (var user in this.Users)
		{
			var response = new ListResponseMessage(user.Id, user.Peer == peer, user.Player.Name);
			peer.Send(channelId: 0, response.Serialize(), ENetPacketFlags.Reliable);
		}
	}
	protected virtual void OnWorldRequest(ENetPeer peer)
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
	protected virtual void OnWorldProgress(ENetPeer peer, WorldProgressMessage worldProgress)
	{
		var user = this.GetUserFromPeer(peer);
		user.DownloadProgress = worldProgress.Percent;
	}
	// Events: Gameplay
	/* Properties */
	public readonly byte MaxPlayers;
	public readonly World World;
	public bool IsTimeRunning = true;
	public Difficulty Difficulty = Difficulty.Normal;
	protected readonly ENetHost _Host;
	protected readonly List<User> Users;
	/* Class Properties */
	private const byte CHANNEL_COUNT = 16;
	private const ushort MAX_CHUNK_SIZE = 1024;
}
