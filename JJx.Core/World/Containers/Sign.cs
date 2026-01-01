/*
	Junk Jack X: Core
	- [World]Sign

	Segment Breakdown:
	------------------------------------------------------------------
	Segment[0x0 : 0x1] = X            | Length: 2 (0x2) | Type: uint16
	Segment[0x2 : 0x3] = Y            | Length: 2 (0x2) | Type: uint16
	Segment[0x4 : 0x5] = Text Length  | Length: 2 (0x2) | Type: uint16
	------------------------------------------------------------------
	Size: 6+ (0x6)

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
