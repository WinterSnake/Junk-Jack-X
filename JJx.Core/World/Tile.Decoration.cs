/*
	Junk Jack X: Core
	- [World]Tile.Decoration

	Written By: Ryan Smith
*/
namespace JJx;

public partial class Tile
{
	public sealed class Decoration
	{
		/* Constructor */
		internal Decoration(ushort id)
		{
			this.IsBackground = id >= BACKGROUND_FLAG;
			if (this.IsBackground)
				id ^= BACKGROUND_FLAG;
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
		public Decoration(ushort id, byte icon, bool isBackground)
		{
			this.Id = id;
			this.Icon = icon;
			this.IsBackground = isBackground;
		}
		/* Properties */
		public ushort Id;
		public byte Icon;
		public bool IsBackground;
		internal ushort Packed {
			get {
				var id = (ushort)((this.Icon << 12) | this.Id);
				if (this.IsBackground)
					id |= BACKGROUND_FLAG;
				return id;
			}
		}
		/* Class Properties */
		private const ushort BACKGROUND_FLAG = 0x8000;
		private const ushort ICON_FLAG       = 0xF000;
	}
}
