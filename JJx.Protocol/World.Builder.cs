/*
	Junk Jack X: Protocol
	- [Builders]World

	Written By: Ryan Smith
*/

using System;
using JJx.Protocol.Packets;

namespace JJx.Protocol;

public sealed class JJxWorldBuilder : IDisposable
{
	/* Constructor */
	public JJxWorldBuilder(
		(ushort, ushort) size, (ushort, ushort) spawn, (ushort, ushort) player
	)
	{
		this.Size = size;
		this.Spawn = spawn;
		this.Player = player;
	}
	/* Instance Methods */
	public void Build()
	{

	}
	public void Dispose()
	{

	}
	/* Static Methods */
	public static JJxWorldBuilder FromWorldInfo(WorldInfoResponsePacket packet) => new(
		packet.Size,
		packet.Spawn,
		packet.Player
	);
	/* Properties */
	public readonly (ushort Width, ushort Height) Size;
	public readonly (ushort X, ushort Y) Spawn;
	public readonly (ushort X, ushort Y) Player;
	public ushort[] Skyline = Array.Empty<ushort>();
}
