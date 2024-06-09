/*
	Junk Jack X: Protocol
	- Connection

	Written By: Ryan Smith
*/
using System;
using System.Net;
using ENet.Managed;

namespace JJx.Protocol;

internal enum ProtocolHeader : ushort
{
	// Primary Type
	Management = (0x00 << 8),
	// Sub-Type: Management
	ManagementLogin = Management | (0x02 << 0),
}

public abstract class Connection
{
	/* Constructor */
	#nullable enable
	public Connection(int peers, byte channels, IPEndPoint? address)
	{
		this._Host = new ENetHost(address, peers, channels);
	}
	#nullable disable
	/* Instance Methods */
	public void ProcessEvent()
	{
		var @event = this._Host.Service(TimeSpan.FromMilliseconds(0));
		switch (@event.Type)
		{
			case ENetEventType.None: return;
			case ENetEventType.Connect:
			{
				this.OnConnect(@event.Peer);
			} break;
			case ENetEventType.Disconnect:
			{
				this.OnDisconnect(@event.Peer);
			} break;
			case ENetEventType.Receive:
			{
				Console.WriteLine($"Channel: {@event.ChannelId} | Flags: {@event.Packet.Flags}");
				var header = (ProtocolHeader)((@event.Packet.Data[0] << 8) | (@event.Packet.Data[1] << 0));
				switch (header)
				{
					case ProtocolHeader.ManagementLogin:
					{
						Console.WriteLine("Login management");
					} break;
					default:
					{
						Console.WriteLine($"Unknown header: {(ushort)header:X2}");
					} break;
				}
				@event.Packet.Destroy();
			} break;
		}
	}
	// Events
	public abstract void OnConnect(ENetPeer peer);
	public abstract void OnDisconnect(ENetPeer peer);
	/* Properties */
	protected ENetHost _Host;
}
