/*
	Junk Jack X: Core
	- [World]Lock

	Written By: Ryan Smith
*/

namespace JJx.Core;

public sealed class Lock
{
	/* Constructor */
	public Lock(ushort x, ushort y, byte radius) :this((x, y), radius) { }
	public Lock((ushort, ushort) position, byte radius)
	{
		this.Position = position;
		this.Radius = radius;
	}
	/* Properties */
	public (ushort X, ushort Y) Position;
	public byte Radius;
}
