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
	ManagementAccept = Management | (0x03 << 0),
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
				Console.WriteLine($"Channel: {@event.ChannelId} | Flags: {@event.Packet.Flags} | Size: {@event.Packet.Data.Length - 2}");
				var header = (ProtocolHeader)((@event.Packet.Data[0] << 8) | (@event.Packet.Data[1] << 0));
				switch (header)
				{
					case ProtocolHeader.ManagementLogin:
					{
						var info = ClientInfo.Deserialize(@event.Packet.Data.Slice(2));
						this.OnClientInfo(@event.Peer, info);
					} break;
					case ProtocolHeader.ManagementAccept:
					{
						Console.WriteLine($"[0]{@event.Packet.Data[2]:X}");
						Console.WriteLine($"[1]{@event.Packet.Data[3]:X}");
						this.OnClientAccepted();
					} break;
					default:
					{
						Console.WriteLine($"Unknown header: {header}");
					} break;
				}
				@event.Packet.Destroy();
			} break;
		}
	}
	// Events
	public virtual void OnConnect(ENetPeer peer) { }
	public virtual void OnDisconnect(ENetPeer peer) { }
	public virtual void OnClientAccepted() { }
	public virtual void OnClientInfo(ENetPeer peer, ClientInfo info) { }
	/* Properties */
	protected ENetHost _Host;
}
