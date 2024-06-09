/*
	Junk Jack X: Protocol
	- Client Info

	Written By: Ryan Smith
*/
using System;
using System.Text;

namespace JJx.Protocol;

public sealed class ClientInfoMessage
{
	/* Constructor */
	public ClientInfoMessage(byte id, string name, JJx.Version version)
	{
		this.Id = id;
		this.Name = name;
		this.Version = version;
	}
	/* Instance Methods */
	public override string ToString()
	{
		return $"ClientInfo[Id:{this.Id}, Name: {this.Name}, Version: {this.Version}]";
	}
	/* Static Methods */
	public static ClientInfoMessage FromBuffer(ReadOnlySpan<byte> buffer)
	{
		var id = buffer[0];
		var name = Encoding.ASCII.GetString(
			buffer.Slice(1, 32)
				  .Slice(0, buffer.Slice(1, 32)
					  			  .IndexOf(Byte.MinValue)
				  )
		);
		var version = (JJx.Version)BitConverter.ToUInt32(buffer.Slice(33, 4));
		return new ClientInfoMessage(id, name, version);
	}
	/* Properties */
	public readonly byte Id;
	public readonly string Name;
	public readonly JJx.Version Version;
}
