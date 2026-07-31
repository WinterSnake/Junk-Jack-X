/*
	Junk Jack X: Protocol
	- [Extensions]JJxWorldBuilder

	Written By: Ryan Smith
*/

using JJx.Protocol;
using JJx.Protocol.Packets;

public static class JJxWorldBuilderExtensions
{
	/* Static Methods */
	public static void DecompressSkyline(this JJxWorldBuilder builder, WorldSkylinePacket packet)
		=> builder.Skyline = packet.Decompress(builder.Size.Width);
}
