/*
	Junk Jack X: Protocol
	- User

	Written By: Ryan Smith
*/
using ENet.Managed;
using JJx;
using JJx.Protocol.Builders;

namespace JJx.Protocol;

public class User
{
	/* Constructors */
	public User(ENetPeer peer, byte id, string name)
	{
		this.Id = id;
		this.Player = new Player(name);
		this.Peer = peer;
	}
	public User(Player player)
	{
		this.Player = player;
	}
	/* Properties */
	public byte Id = 0xFF;
	public ENetPeer Peer { get; protected set; }
	public readonly Player Player;
	public float DownloadProgress;
}
