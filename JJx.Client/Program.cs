/*
	Junk Jack X: Client

	Written By: Ryan Smith
*/

using System;
using System.Net;
using ENet.Managed;
using JJx.Core;

ManagedENet.Startup();

// Setup
var player = Archiver.LoadPlayer("../docs/saves/players/Winter.dat");
var address = new IPEndPoint(IPAddress.Loopback, 12345);
var manager = new JJxClientManager(player);
manager.OnConnected += () => Console.WriteLine("Connected to server!");
var peer = manager.Connect(address);

while (!Console.KeyAvailable)
{
	manager.Service(TimeSpan.Zero);
}

peer.Disconnect();

ManagedENet.Shutdown();
