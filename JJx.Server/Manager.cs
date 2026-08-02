/*
	Junk Jack X: Server
	- Manager

	Written By: Ryan Smith
*/

using System;
using System.Collections.Generic;
using System.Net;
using ENet.Managed;
using JJx.Core;
using JJx.Protocol;
using JJx.Protocol.Extensions;
using JJx.Protocol.Packets;

public sealed class JJxServerManager
{
	/* Constructor */
	public JJxServerManager(JJxWorld world, IPEndPoint address, byte maxPlayers, byte channels = 0)
	{
		this.World = world;
		this._Host = new(address, maxPlayers, channels);
	}
	/* Instance Methods */
	public void Service(TimeSpan timeout)
	{
		var @event = this._Host.Service(timeout);
		switch (@event.Type)
		{
			case ENetEventType.None: break;
			case ENetEventType.Connect:
			{
				this._Pending.Add(@event.Peer, new JJxServerPeer.Builder(0xFF, @event.Peer));
			} break;
			case ENetEventType.Disconnect:
			{
			} break;
			case ENetEventType.Receive:
			{
				Console.WriteLine($"[Packet@{@event.Peer.GetRemoteEndPoint()}] Channel: {@event.ChannelId} ; User data: {@event.Peer.UserData} ; Flags: {@event.Packet.Flags}");
				var packet = JJxPacketSerializer.Deserialize(@event.Packet.Data, JJxPacketRegistry.Server);
				this.OnPacket(@event.Peer, packet);
				@event.Packet.Destroy();
			} break;
		}
	}
	private void OnPacket(ENetPeer peer, JJxPacket packet)
	{
		switch (packet)
		{
			case LoginRequestPacket login:
			{
				var jjxPeer = this._Pending[peer];
				jjxPeer.Name = login.Name;
				jjxPeer.Version = login.Version;
				jjxPeer.Send(new LoginSuccessPacket());
			} break;
			case WorldInfoRequestPacket worldInfo:
			{
				var jjxPeer = this._Pending[peer];
				Console.WriteLine($"[Player:{jjxPeer.Name}] Get world");
				(var sizeInBytes, var segmentIter) = this.World.Blocks.Compress();
				jjxPeer.Send(new WorldInfoResponsePacket(
					this.World.Size, this.World.Spawn, this.World.Player,
					0, DayPhase.Day, false, Weather.None, this.World.Planet,
					Difficulty.Normal, this.World.Planet, Season.None, Gamemode.Creative,
					this.World.SizeBounds, this.World.SkyBounds, WorldInfoResponsePacket.UNKNOWN, sizeInBytes
				));
				jjxPeer.Send(WorldSkylinePacket.Compress(this.World.Skyline));
				foreach (var segment in segmentIter)
					jjxPeer.Send(segment);
			} break;
			case WorldProgressPacket progress:
			{
				var jjxPeer = this._Pending[peer];
				jjxPeer.Progress = progress.ProgressAsFloat();
				Console.WriteLine($"[Player:{jjxPeer.Name}] Progress: {jjxPeer.Progress}");
			} break;
			case PlayerListRequestPacket playerList:
			{
				var jjxPeer = this._Pending[peer];
				foreach ((var enetPeer, var builder) in this._Pending)
				{
					var isSelf = jjxPeer == builder;
					jjxPeer.Send(new PlayerListEntryPacket(builder.Id, isSelf, builder.Name));
				}
				jjxPeer.Send(new WorldTimePacket(DayPhase.Day, 0));
			} break;
			case PlayerReadyPacket playerReady:
			{
				var jjxPeer = this._Pending[peer];
				Console.WriteLine($"[Player:{jjxPeer.Name}] Health: {playerReady.Health}/{playerReady.MaxHealth}");
			} break;
		}
	}
	/* Properties */
	public readonly JJxWorld World;
	private readonly ENetHost _Host;
	private readonly Dictionary<ENetPeer, JJxServerPeer.Builder> _Pending = new();
}
