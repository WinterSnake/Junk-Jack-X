/*
	Junk Jack X: Protocol
	- [Message: Responses]Login

	Written By: Ryan Smith
*/
using System;

namespace JJx.Protocol;

public enum LoginFailureReason : byte
{
	ServerFull = 0x00,
	IncorrectVersion = 0x01,
}

public sealed class LoginResponseMessage
{
	/* Constructors */
	public LoginResponseMessage() { }
	public LoginResponseMessage(LoginFailureReason reason) { this.FailureReason = reason; }
	/* Instance Methods */
	public byte[] Serialize()
	{
		byte[] buffer;
		if (this.FailureReason == null)
		{
			buffer = new byte[sizeof(ushort) * 2];
			BitConverter.BigEndian.Write((ushort)MessageHeader.LoginSuccess, buffer);
			BitConverter.LittleEndian.Write((ushort)1, buffer, sizeof(ushort));
		}
		else
		{
			buffer = new byte[sizeof(ushort) + sizeof(byte)];
			BitConverter.BigEndian.Write((ushort)MessageHeader.LoginFailure, buffer);
			buffer[sizeof(ushort)] = (byte)this.FailureReason;
		}
		return buffer;
	}
	/* Static Methods */
	public static LoginResponseMessage Deserialize(ReadOnlySpan<byte> buffer)
	{
		// Login Failure
		if (buffer.Length == 1)
			return new LoginResponseMessage((LoginFailureReason)buffer[0]);
		// Login Success
		#if DEBUG
			Console.WriteLine($"LoginResponse.Data={BitConverter.ToString(buffer)}");
		#endif
		return new LoginResponseMessage();
	}
	/* Properties */
	public readonly LoginFailureReason? FailureReason = null;
}
