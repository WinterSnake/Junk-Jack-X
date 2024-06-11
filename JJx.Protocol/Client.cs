/*
	Junk Jack X: Protocol
	- [Connection]Client

	Written By: Ryan Smith
*/
using System;
using System.Net;
using ENet.Managed;
using JJx;

namespace JJx.Protocol;

public class Client : Connection
{
	/* Constructor */
	public Client(Player player): base(PEER_COUNT, CHANNEL_COUNT, null)
	{
		this.Player = player;
		this.Id = (byte)Random.Shared.Next(0, 255);
	}
	/* Instance Methods */
	public void Connect(IPEndPoint address) => this.ServerPeer = this._Host.Connect(address, CHANNEL_COUNT, 0);
	protected void RequestLogin()
	{
		var request = new LoginRequestMessage(this.Id, this.Player.Name, this.Player.Version);
		this.ServerPeer.Send(channelId: 0, request.Serialize(), ENetPacketFlags.Reliable);
	}
	public void RequestWorld()
	{
		var request = new WorldRequestMessage();
		this.ServerPeer.Send(channelId: 0, request.Serialize(), ENetPacketFlags.Reliable);
	}
	// Events
	protected override void OnConnect(ENetPeer peer) => this.RequestLogin();
	protected override void OnLoginSuccess() => this.RequestWorld();
	protected override void OnLoginFailed(LoginFailureReason reason) => Console.WriteLine($"Failed to connect to server: {reason}");
	protected override void OnWorldInfo(WorldInfoResponseMessage worldInfo)
	{
		this._World = new(worldInfo);
	}
	protected override void OnWorldSkyline(WorldSkylineResponseMessage worldSkyline)
	{
		this._World.Skyline = worldSkyline;
	}
	protected override void OnWorldBlocks(WorldBlocksResponseMessage worldBlocks)
	{
		this._World.AddToBlockBuffer(worldBlocks);
		if (!this._World.IsReady)
			return;
		Console.WriteLine("World is finished and ready to decompress/build");
	}
	/* Properties */
	public readonly Player Player;
	public readonly byte Id;
	protected ENetPeer ServerPeer { get; private set; }
	private WorldBuilder _World;
	/* Class Properties */
	private const int PEER_COUNT = 1;
	private const byte CHANNEL_COUNT = 16;
}
