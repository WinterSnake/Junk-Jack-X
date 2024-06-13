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

public class Server
{
	/* Constructor */
	public Server(World world, IPEndPoint address, int maxPlayers)
	{
		this._Host = new ENetHost(address, maxPlayers, CHANNEL_COUNT);
		this.World = world;
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
		switch (header)
		{
			// Management \\
			case MessageHeader.LoginRequest:
			{
				var loginRequest = LoginRequestMessage.Deserialize(@event.Packet.Data.Slice(2));
				this.OnLoginRequest(@event.Peer, loginRequest);
			} break;
			case MessageHeader.WorldRequest:
			{
				var worldRequest = WorldRequestMessage.Deserialize(@event.Packet.Data.Slice(2));
				this.OnWorldRequest(@event.Peer);
			} break;
			case MessageHeader.WorldProgress:
			{
				var worldProgress = WorldProgressMessage.Deserialize(@event.Packet.Data.Slice(2));
				this.OnWorldProgress(@event.Peer, worldProgress);
			} break;
			case MessageHeader.ListRequest:
			{
				var listRequest = ListRequestMessage.Deserialize(@event.Packet.Data.Slice(2));
				this.OnListRequest(@event.Peer);
			} break;
			default:
			{
				Console.WriteLine($"Unknown header: 0x{(ushort)header:X4} | Size: {@event.Packet.Data.Length - 2} | Data: {BitConverter.ToString(@event.Packet.Data.Slice(2))}");
			} break;
		}
	}
	// Events
	protected virtual void OnConnect(ENetPeer peer) { }
	protected virtual void OnDisconnect(ENetPeer peer) { }
	protected virtual void OnLoginRequest(ENetPeer peer, LoginRequestMessage info)
	{
		LoginResponseMessage response;
		if (info.Version != this.World.Version)
			response = new LoginResponseMessage(LoginFailureReason.IncorrectVersion);
		else
			response = new LoginResponseMessage();
		peer.Send(channelId: 0, response.Serialize(), ENetPacketFlags.Reliable);
	}
	protected virtual void OnListRequest(ENetPeer peer)
	{
		Console.WriteLine($"Player[{peer.GetRemoteEndPoint()}] is requesting peer list");
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
		Console.WriteLine($"Player[{peer.GetRemoteEndPoint()}] is at {worldProgress.Percent * 100.0f}%");
	}
	/* Properties */
	protected readonly ENetHost _Host;
	public Difficulty Difficulty = Difficulty.Normal;
	public bool IsTimeRunning = true;
	public readonly World World;
	/* Class Properties */
	private const byte CHANNEL_COUNT = 16;
	private const ushort MAX_CHUNK_SIZE = 1024;
}
