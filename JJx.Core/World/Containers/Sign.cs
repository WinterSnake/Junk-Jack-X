/*
	Junk Jack X: Core
	- [World]Sign

	Written By: Ryan Smith
*/

namespace JJx.Core;

public sealed class Sign
{
	/* Constructor */
	public Sign(ushort x, ushort y, string text) :this((x, y), text) { }
	public Sign((ushort, ushort) position, string text)
	{
		this.Position = position;
		this.Text = text;
	}
	/* Properties */
	public (ushort X, ushort Y) Position;
	public string Text;
}
