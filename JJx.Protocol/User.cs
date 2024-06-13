/*
	Junk Jack X: Protocol
	- User

	Written By: Ryan Smith
*/
using ENet.Managed;

namespace JJx.Protocol;

public class User
{
	/* Constructors */
	public User(ENetPeer peer)
	{
		this._Peer = peer;
	}
	public User(Player player)
	{
		this.Player = player;
	}
	/* Properties */
	public byte Id = 0xFF;
	public ENetPeer _Peer { get; protected set; }
	public Player Player { get; private set; }
}
