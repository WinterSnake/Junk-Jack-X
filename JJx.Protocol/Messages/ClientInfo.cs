/*
	Junk Jack X: Protocol
	- Messages:Client Info

	Written By: Ryan Smith
*/
using System;
using System.Text;
using JJx;

namespace JJx.Protocol;

public sealed class ClientInfo
{
	/* Constructor */
	public ClientInfo(byte id, string name, JJx.Version version)
	{
		this.Id = id;
		this.Name = name;
		this.Version = version;
	}
	/* Instance Methods */
	public ReadOnlySpan<byte> Serialize()
	{
		var buffer = new byte[32];
		return new ReadOnlySpan<byte>(buffer);
	}
	/* Static Methods */
	public static ClientInfo Deserialize(ReadOnlySpan<byte> buffer)
	{
		return new ClientInfo(0x00, "Temporary", JJx.Version.Latest);
	}
	/* Properties */
	public readonly byte Id;
	public readonly string Name;
	public readonly JJx.Version Version;
}
