/*
	Junk Jack X: Server

	Written By: Ryan Smith
*/
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using ENet.Managed;

using JJx;

internal static class Program
{
	/* Static Methods */
	private static async Task Main(string[] args)
	{
		Console.WriteLine("JJx: Server");
		ManagedENet.Startup();
		var address = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);
		Console.WriteLine($"JJx: Server (running @{address})");
		var serverHost = new ENetHost(address, Program.MAX_PLAYER_COUNT, Program.CHANNEL_COUNT);
		while (true)
		{
			var @event = serverHost.Service(TimeSpan.FromMilliseconds(0));
			switch (@event.Type)
			{
				case ENetEventType.None: break;
				case ENetEventType.Connect:
				{
					Console.WriteLine($"Peer connected: {@event.Peer.GetRemoteEndPoint()}");
				} break;
				case ENetEventType.Disconnect:
				{
					Console.WriteLine($"Peer disconnected: {@event.Peer.GetRemoteEndPoint()}");
				} break;
				case ENetEventType.Receive:
				{
					Program.ProcessEvent(@event);
					@event.Packet.Destroy();
				} break;
			}
		}
	}
	private static void ProcessEvent(ENetEvent @event)
	{

	}
	/* Class Properties */
	private const byte MAX_PLAYER_COUNT =   16;
	private const byte CHANNEL_COUNT    =    1;
	private const ushort MAX_CHUNK_SIZE = 1024;
}
