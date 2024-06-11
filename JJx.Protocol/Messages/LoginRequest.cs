/*
	Junk Jack X: Protocol
	- [Message: Requests]Login

	Written By: Ryan Smith
*/
using System;
using System.Text;
using JJx;

namespace JJx.Protocol;

public sealed class LoginRequestMessage
{
	/* Constructor */
	public LoginRequestMessage(byte id, string name, JJx.Version version)
	{
		this.Id = id;
		this.Name = name;
		this.Version = version;
	}
	/* Instance Methods */
	public override string ToString() => $"Login Request[Id: {this.Id:X}, Name: {this.Name}(Length={this.Name.Length}), Version: {this.Version}]";
	public byte[] Serialize()
	{
		var buffer = new byte[SIZE + sizeof(ushort)];
		// Header
		BitConverter.BigEndian.Write((ushort)MessageHeader.LoginRequest, buffer);
		// Id
		buffer[OFFSET_ID + sizeof(ushort)] = this.Id;
		// Name
		BitConverter.Write(this.Name, buffer, OFFSET_NAME + sizeof(ushort), SIZEOF_NAME);
		// Version
		BitConverter.LittleEndian.Write((uint)this.Version, buffer, OFFSET_VERSION + sizeof(ushort));
		return buffer;
	}
	/* Static Methods */
	public static LoginRequestMessage Deserialize(ReadOnlySpan<byte> buffer)
	{
		var id = buffer[OFFSET_ID];
		var name = BitConverter.GetString(buffer, OFFSET_NAME);
		var version = (JJx.Version)BitConverter.LittleEndian.GetUInt32(buffer, OFFSET_VERSION);
		return new LoginRequestMessage(id, name, version);
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
