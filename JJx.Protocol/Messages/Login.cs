/*
	Junk Jack X: Protocol
	- [Message Data]Login Status

	Written By: Ryan Smith
*/

namespace JJx.Protocol;

public enum LoginFailureReason : byte
{
	ServerFull = 0x00,
	IncorrectVersion = 0x01,
}

public sealed class LoginResponse
{
	/* Constructors */
	public LoginResponse() { }
	public LoginResponse(LoginFailureReason reason) { this.FailureReason = reason; }
	/* Instance Methods */
	public byte[] Serialize()
	{
		byte[] buffer;
		if (this.FailureReason == null)
		{
			buffer = new byte[sizeof(ushort) * 2];
			BitConverter.BigEndian.Write((ushort)ProtocolHeader.LoginSuccess, buffer);
			BitConverter.LittleEndian.Write((ushort)1, buffer, sizeof(ushort));
		}
		else
		{
			buffer = new byte[sizeof(ushort) + sizeof(byte)];
			BitConverter.BigEndian.Write((ushort)ProtocolHeader.LoginFailure, buffer);
			buffer[sizeof(ushort)] = (byte)this.FailureReason;
		}
		return buffer;
	}
	/* Properties */
	public readonly LoginFailureReason? FailureReason = null;
}
