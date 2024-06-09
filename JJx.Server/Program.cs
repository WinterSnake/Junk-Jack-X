/*
	Junk Jack X Tools: Server

	Written By: Ryan Smith
*/
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ENet.Managed;

internal static class Program
{
	/* Static Methods */
	private static async Task Main(string[] args)
	{
		Console.WriteLine("JJx: Server");
		var address = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);
		ManagedENet.Startup();
		var server = new ENetHost(address, 16, 16);

		while (true)
		{
			var @event = server.Service(TimeSpan.FromMilliseconds(0));
			switch(@event.Type)
			{
				case ENetEventType.None: continue;
				case ENetEventType.Connect:
				{
					Console.WriteLine($"User connected @{@event.Peer.GetRemoteEndPoint()}");
				} break;
				case ENetEventType.Disconnect:
				{
					Console.WriteLine($"User disconnected @{@event.Peer.GetRemoteEndPoint()}");
				} break;
				case ENetEventType.Receive:
				{
					Console.WriteLine("Received data from user..");
					@event.Packet.Destroy();
				} break;
			}
		}
	}
}
