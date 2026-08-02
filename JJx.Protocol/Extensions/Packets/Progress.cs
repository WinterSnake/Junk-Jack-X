/*
	Junk Jack X: Protocol
	- [Extensions]Packets

	Written By: Ryan Smith
*/

using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using CommunityToolkit.HighPerformance;
using JJx.Protocol.Packets;

namespace JJx.Protocol.Extensions;

public static partial class PacketExtensions
{
	/* Static Methods */
	public static float ProgressAsFloat(this WorldProgressPacket packet)
		=> (float)packet.Progress / 100.0f;
}
