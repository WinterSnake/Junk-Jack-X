/*
	Junk Jack X: Client
	- Manager

	Written By: Ryan Smith
*/

using System;
using System.Net;
using ENet.Managed;
using JJx.Core;
using JJx.Protocol;
using JJx.Protocol.Extensions;
using JJx.Protocol.Packets;

public sealed class JJxClientManager
{
	/* Constructor */
	public JJxClientManager(JJxPlayer player, byte channels = 0)
	{
		this.Player = player;
		this._Host = new(null, 1, channels);
	}
	/* Instance Methods */
	public JJxClientPeer Connect(IPEndPoint address, byte channels = 1)
		=> this._Peer = new(
			(byte)Random.Shared.Next(0, 255),
			this._Host.Connect(address, channels, 0)
		);
	public void Service(TimeSpan timeout)
	{
		var @event = this._Host.Service(timeout);
		switch (@event.Type)
		{
			case ENetEventType.None: break;
			case ENetEventType.Connect:
			{
				this.Peer.Send(new LoginRequestPacket(
					this.Peer.Id,
					this.Player.Name,
					this.Player.Version
				));
				this.OnConnected?.Invoke();
			} break;
			case ENetEventType.Disconnect:
			{
				this.OnDisconnected?.Invoke();
			} break;
			case ENetEventType.Receive:
			{
				Console.WriteLine($"[Packet] Channel: {@event.ChannelId} ; User data: {@event.Peer.UserData} ; Flags: {@event.Packet.Flags}");
				var packet = JJxPacketSerializer.Deserialize(@event.Packet.Data, JJxPacketRegistry.Client);
				this.OnPacket(packet);
				@event.Packet.Destroy();
			} break;
		}
	}
	private void OnPacket(JJxPacket packet)
	{
		switch (packet)
		{
			case LoginSuccessPacket loginSuccess:
			{
				this.Peer.Send(new WorldInfoRequestPacket());
				Console.WriteLine($"Login succeeded: {loginSuccess.Status}");
			} break;
			case LoginFailPacket loginFail:
			{
				Console.WriteLine($"Login failed: {loginFail.Code}");
				this.Peer.Disconnect();
			} break;
			case WorldInfoResponsePacket worldInfo:
			{
				this._Builder = JJxWorldBuilder.FromWorldInfo(worldInfo);
				Console.WriteLine($"Skyline applied");
			} break;
			case WorldSkylinePacket worldSkyline:
			{
				this.Builder.ApplySkyline(worldSkyline);
				Console.WriteLine($"Skyline applied");
			} break;
			case WorldCompressedSegmentPacket worldSegment:
			{
				var progressPacket = this.Builder.ApplyCompressedSegment(worldSegment);
				this.Peer.Send(progressPacket);
				if (this.Builder.AreSegmentsCompleted)
				{
					Console.WriteLine($"World segments finished");
					this.Peer.Send(new PlayerListRequestPacket());
				}
			} break;
			case PlayerListEntryPacket playerEntry:
			{
				if (playerEntry.IsSelf)
				{
					this.Peer.Id = playerEntry.Id;
					this.Peer.Send(new PlayerUpdateModelPacket(this.Peer.Id, this.Player.Model));
				}
				Console.WriteLine($"[Entry] Id: {playerEntry.Id} ; IsSelf: {playerEntry.IsSelf} ; Name: {playerEntry.Name}");
				Console.WriteLine($"Self Id: {this.Peer.Id}");
			} break;
			case WorldTimePacket worldTime:
			{
				Console.WriteLine($"Phase: {worldTime.DayPhase} ; Ticks: {worldTime.Ticks}");
				this.Peer.Send(new PlayerReadyPacket(5, 5));
			} break;
			case PlayerUpdateModelPacket updateModel:
			{
				Console.WriteLine($"[Update:Model {updateModel.Id}] {updateModel.Model}");
			} break;
			case PlayerUpdateItemPacket updateItem:
			{
				Console.WriteLine($"[Update:Item {updateItem.Id}] Item: {updateItem.Item} ; Slot: {updateItem.Slot}");
			} break;
			case PlayerUpdateEquipmentPacket updateEquipment:
			{
				Console.WriteLine($"[Update:Equipment {updateEquipment.Id}] Item: {updateEquipment.Item} ; Slot: {updateEquipment.Slot} ; IsVisual: {updateEquipment.IsVisual}");
			} break;
		}
	}
	/* Properties */
	public event Action? OnConnected;
	public event Action? OnDisconnected;
	public readonly JJxPlayer Player;
	private readonly ENetHost _Host;
	private JJxClientPeer? _Peer = null;
	public JJxClientPeer Peer => this._Peer!;
	private JJxWorldBuilder? _Builder = null;
	private JJxWorldBuilder Builder => this._Builder!;
	private JJxWorld? _World;
	public JJxWorld World => this._World!;
		
}
