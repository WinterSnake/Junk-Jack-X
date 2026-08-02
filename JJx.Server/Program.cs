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
var world = Archiver.LoadWorld("../docs/saves/worlds/Terra-F.dat");
var address = new IPEndPoint(IPAddress.Loopback, 12345);
var manager = new JJxServerManager(world, address, 16);
Console.WriteLine($"Listening @ {address}");

// Process
while (!Console.KeyAvailable)
{
	manager.Service(TimeSpan.Zero);
}

ManagedENet.Shutdown();
