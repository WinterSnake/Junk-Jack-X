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
	Management    = (0x00 << 8),
	// Sub-Type: Management
	Login         = Management | 0x02,
	LoginSuccess  = Management | 0x03,
	LoginFailure  = Management | 0x0C,
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
				this.HandleMessage(@event);
				@event.Packet.Destroy();
			} break;
		}
	}
	protected virtual void HandleMessage(ENetEvent @event)
	{
		#if DEBUG
			Console.WriteLine($"Channel: {@event.ChannelId} | Flags: {@event.Packet.Flags}");
		#endif
		var header = (ProtocolHeader)((@event.Packet.Data[0] << 8) | (@event.Packet.Data[1] << 0));
		switch (header)
		{
			// Management \\
			case ProtocolHeader.Login:
			{
				var info = ClientInfo.Deserialize(@event.Packet.Data.Slice(2));
				this.OnLoginAttempt(@event.Peer, info);
			} break;
			case ProtocolHeader.LoginSuccess:
			{
				var @value = BitConverter.LittleEndian.GetUInt16(@event.Packet.Data, sizeof(ushort));
				Console.WriteLine($"Login Success Value: 0x{@value:X2}");
				this.OnLoginSuccess();
			} break;
			case ProtocolHeader.LoginFailure:
			{
				this.OnLoginFailed((LoginFailureReason)@event.Packet.Data[2]);
			} break;
			// UNKNOWN \\
			default:
			{
				Console.WriteLine($"Unknown header: 0x{(ushort)header:X4} | Size: {@event.Packet.Data.Length - 2}");
			} break;
		}
	}
	// Events
	public virtual void OnConnect(ENetPeer peer) { }
	public virtual void OnDisconnect(ENetPeer peer) { }
	public virtual void OnLoginAttempt(ENetPeer peer, ClientInfo info) { }
	public virtual void OnLoginSuccess() { }
	public virtual void OnLoginFailed(LoginFailureReason reason) { }
	/* Properties */
	protected ENetHost _Host;
}
