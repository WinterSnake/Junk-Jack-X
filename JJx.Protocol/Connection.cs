/*
	Junk Jack X: Protocol
	- Connection

	Written By: Ryan Smith
*/
using System;
using System.Net;
using ENet.Managed;

namespace JJx.Protocol;

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
			Console.WriteLine($"Channel: {@event.ChannelId} | Flags: {@event.Packet.Flags} | Size: {@event.Packet.Data.Length - 2}");
		#endif
		var header = (MessageHeader)((@event.Packet.Data[0] << 8) | (@event.Packet.Data[1] << 0));
		switch (header)
		{
			// Management \\
			case MessageHeader.LoginRequest:
			{
				var loginRequest = LoginRequestMessage.Deserialize(@event.Packet.Data.Slice(2));
				this.OnLoginRequest(@event.Peer, loginRequest);
			} break;
			case MessageHeader.LoginSuccess:
			{
				var @value = BitConverter.LittleEndian.GetUInt16(@event.Packet.Data, sizeof(ushort));
				Console.WriteLine($"Login Success Value: 0x{@value:X2}");
				this.OnLoginSuccess();
			} break;
			case MessageHeader.LoginFailure:
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
	public virtual void OnLoginRequest(ENetPeer peer, LoginRequestMessage info) { }
	public virtual void OnLoginSuccess() { }
	public virtual void OnLoginFailed(LoginFailureReason reason) { }
	/* Properties */
	protected ENetHost _Host;
}
