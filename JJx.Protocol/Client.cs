/*
	Junk Jack X: Protocol
	- [Connection]Client

	Written By: Ryan Smith
*/
using System;
using System.Net;
using ENet.Managed;
using JJx;
using JJx.Protocol.Builders;

namespace JJx.Protocol;

public class Client : User
{
	/* Constructor */
	public Client(Player player): base(player)
	{
		this._Host = new ENetHost(null, PEER_COUNT, CHANNEL_COUNT);
	}
	/* Instance Methods */
	public void Connect(IPEndPoint address) => this.Peer = this._Host.Connect(address, CHANNEL_COUNT, 0);
	public void ProcessEvent()
	{
		var @event = this._Host.Service(TimeSpan.FromMilliseconds(0));
		switch (@event.Type)
		{
			case ENetEventType.None: return;
			case ENetEventType.Connect: this.OnConnect(); break;
			case ENetEventType.Disconnect: this.OnDisconnect(); break;
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
			case MessageHeader.LoginSuccess:
			{
				var response = LoginResponseMessage.Deserialize(@event.Packet.Data.Slice(2));
				this.OnLoginSuccess();
			} break;
			case MessageHeader.ListResponse:
			{
				var response = ListResponseMessage.Deserialize(@event.Packet.Data.Slice(2));
				this.OnListResponse(response);
			} break;
			case MessageHeader.LoginFailure:
			{
				var response = LoginResponseMessage.Deserialize(@event.Packet.Data.Slice(2));
				this.OnLoginFailure(response.FailureReason.Value);
			} break;
			// World-Data \\
			case MessageHeader.WorldInfoResponse:
			{
				var response = WorldInfoResponseMessage.Deserialize(@event.Packet.Data.Slice(2));
				this.OnWorldInfo(response);
			} break;
			case MessageHeader.WorldTime:
			{
				var status = WorldTimeMessage.Deserialize(@event.Packet.Data.Slice(2));
				this.OnWorldTime(status);
			} break;
			case MessageHeader.WorldBlocksResponse:
			{
				var response = WorldBlocksResponseMessage.Deserialize(@event.Packet.Data.Slice(2));
				this.OnWorldBlocks(response);
			} break;
			case MessageHeader.WorldSkylineResponse:
			{
				var response = WorldSkylineResponseMessage.Deserialize(@event.Packet.Data.Slice(2));
				this.OnWorldSkyline(response);
			} break;
			default:
			{
				Console.WriteLine($"Unknown header: 0x{(ushort)header:X4} | Size: {@event.Packet.Data.Length - 2} | Data: {BitConverter.ToString(@event.Packet.Data.Slice(2))}");
			} break;
		}
	}
	// Events
	protected virtual void OnConnect()
	{
		var request = new LoginRequestMessage(this.Id, this.Player.Name, this.Player.Version);
		this.Peer.Send(channelId: 0, request.Serialize(), ENetPacketFlags.Reliable);
	}
	protected virtual void OnDisconnect() { }
	protected virtual void OnLoginSuccess()
	{
		var request = new WorldRequestMessage();
		this.Peer.Send(channelId: 0, request.Serialize(), ENetPacketFlags.Reliable);
	}
	protected virtual void OnLoginFailure(LoginFailureReason reason)
	{
		Console.WriteLine($"Failed to connect to server: {reason}");
	}
	protected virtual void OnListResponse(ListResponseMessage listResponse)
	{
		Console.WriteLine($"Id: {listResponse.Id}, Name: {listResponse.Name}");
		if (!listResponse.IsConnectedPlayer)
			return;
		this.Id = listResponse.Id;
		Console.WriteLine($"My new Id: 0x{this.Id:X2}");
	}
	protected virtual void OnWorldInfo(WorldInfoResponseMessage worldInfo)
	{
		this._World = new(worldInfo);
	}
	protected virtual void OnWorldSkyline(WorldSkylineResponseMessage worldSkyline)
	{
		this._World.Skyline = worldSkyline;
	}
	protected virtual void OnWorldBlocks(WorldBlocksResponseMessage worldBlocks)
	{
		var percent = this._World.AddToBlockBuffer(worldBlocks);
		var progress = new WorldProgressMessage(percent);
		this.Peer.Send(channelId: 0, progress.Serialize(), ENetPacketFlags.Reliable);
		if (!this._World.IsReady)
			return;
		var request = new ListRequestMessage();
		this.Peer.Send(channelId: 0, request.Serialize(), ENetPacketFlags.Reliable);
	}
	// Events: Gameplay
	protected virtual void OnWorldTime(WorldTimeMessage worldTime)
	{
		Console.WriteLine($"Time: {worldTime.Time}, Ticks: {worldTime.Ticks}");
	}
	/* Properties */
	protected readonly ENetHost _Host;
	private WorldBuilder _World;
	/* Class Properties */
	private const int PEER_COUNT = 1;
	private const byte CHANNEL_COUNT = 16;
}
