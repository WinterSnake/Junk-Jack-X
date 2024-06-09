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
	public override string ToString() => $"ClientInfo[Id: {this.Id:X}, Name: {this.Name}(Length={this.Name.Length}), Version: {this.Version}]";
	public byte[] Serialize()
	{
		var buffer = new byte[ClientInfo.SIZE + 2];
		// Header
		var headerBytes = BitConverter.GetBytes((ushort)ProtocolHeader.ManagementLogin);
		Array.Reverse(headerBytes);
		Array.Copy(headerBytes, 0, buffer, 0, headerBytes.Length);
		// Id
		buffer[ClientInfo.OFFSET_ID + 2] = this.Id;
		// Name
		var nameBytes = Encoding.ASCII.GetBytes(this.Name);
		Array.Copy(nameBytes, 0, buffer, ClientInfo.OFFSET_NAME + 2, nameBytes.Length);
		// Version
		var versionBytes = BitConverter.GetBytes((uint)this.Version);
		Array.Copy(versionBytes, 0, buffer, ClientInfo.OFFSET_VERSION + 2, versionBytes.Length);
		return buffer;
	}
	/* Static Methods */
	public static ClientInfo Deserialize(ReadOnlySpan<byte> buffer)
	{
		if (buffer.Length != ClientInfo.SIZE)
			throw new Exception();
		var id = buffer[ClientInfo.OFFSET_ID];
		var name = Encoding.ASCII.GetString(
			buffer.Slice(ClientInfo.OFFSET_NAME, ClientInfo.SIZEOF_NAME)
				  .Slice(0, buffer.Slice(ClientInfo.OFFSET_NAME, ClientInfo.SIZEOF_NAME)
					  			  .IndexOf((byte)0)
			)
		);
		var version = (JJx.Version)(BitConverter.ToUInt32(buffer.Slice(ClientInfo.OFFSET_VERSION, 4)));
		return new ClientInfo(id, name, version);
	}
	/* Properties */
	public readonly byte Id;
	public readonly string Name;
	public readonly JJx.Version Version;
	/* Class Properties */
	private const byte SIZE           = 37;
	private const byte OFFSET_ID      =  0;
	private const byte OFFSET_NAME    =  1;
	private const byte OFFSET_VERSION = 33;
	private const byte SIZEOF_NAME    = 32;
}
