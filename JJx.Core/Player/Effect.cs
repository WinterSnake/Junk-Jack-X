/*
	Junk Jack X: Core
	- [Player]Effect

	Written By: Ryan Smith
*/

namespace JJx.Core;

public record struct Effect
{
	/* Constructor */
	public Effect(ushort id, ushort duration)
	{
		this.Id = id;
		this.Duration = duration;
	}
	/* Properties */
	public ushort Id;
	public ushort Duration;
}
