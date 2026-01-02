/*
	Junk Jack X: Core
	- [World]Plant - Crop

	Written By: Ryan Smith
*/

public sealed class Crop : Plant
{
	/* Constructor */
	public Crop(ushort x, ushort y, uint id) :this((x, y), id) { }
	public Crop((ushort, ushort) position, uint id) : base(position, id) { }
}
