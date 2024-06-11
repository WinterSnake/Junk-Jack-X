/*
	Junk Jack X Tools: Server

	Written By: Ryan Smith
*/
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ENet.Managed;
using JJx;
using JJx.Protocol;

internal static class Program
{
	/* Static Methods */
	private static async Task Main(string[] args)
	{
		ManagedENet.Startup();
		var world = await World.Load("./data/worlds/Tiny-Empty.dat");
		var address = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);
		Console.WriteLine($"JJx: Server (running @{address})");
		var server = new JJx.Protocol.Server(world, address, 16);
		while (true)
		{
			server.ProcessEvent();
		}
	}
}
