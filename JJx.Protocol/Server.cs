/*
	Junk Jack X: Protocol
	- Server

	Written By: Ryan Smith
*/
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ENetSharp;

namespace JJx.Protocol;

public class Server : Host
{
	/* Constructors */
	public Server(IPEndPoint address, ushort maxPlayers = 4): base(address, maxPlayers, 1)
	{

	}
	/* Instance Methods */
	/* Properties */
	/* Class Properties */
}
