/*
	Junk Jack X: Core
	- [World]Tile.Block

	Written By: Ryan Smith
*/
namespace JJx;

public partial class Tile
{
	public sealed class Block
	{
		/* Constructor */
		internal Block(ushort id)
		{
			this.Icon = (byte)(id & ICON_FLAG);
			if (this.Icon > 0)
			{
				this.Icon >>= 12;
				unchecked {
					id &= (ushort)~ICON_FLAG;
				}
			}
			this.Id = id;
		}
		public Block(ushort id, byte icon)
		{
			this.Id = id;
			this.Icon = icon;
		}
		/* Properties */
		public ushort Id;
		public byte Icon;
		internal ushort Packed => (ushort)((this.Icon << 12) | this.Id);
		/* Class Properties */
		private const ushort ICON_FLAG = 0xF000;
	}
}
