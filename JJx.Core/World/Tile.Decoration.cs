/*
	Junk Jack X: Core
	- [World]Tile.Decoration

	Written By: Ryan Smith
*/
using System;

namespace JJx;

public partial class Tile
{
	/* Sub-Classes */
	public sealed class Decoration
	{
		/* Constructor */
		public Decoration(ushort id, byte icon = 0, bool isBackground = false)
		{
			this.Id = id;
			this.Icon = icon;
			this.IsBackground = isBackground;
		}
		/* Instance Methods */
		public ushort Pack()
		{
			var id = (ushort)((this.Icon << 12) | this.Id);
			if (this.IsBackground)
				id |= BACKGROUND_FLAG;
			return id;
		}
		/* Static Methods */
		public static Decoration Unpack(ushort id)
		{
			var isBackground = id >= BACKGROUND_FLAG;
			if (isBackground)
				id ^= BACKGROUND_FLAG;
			var icon = id & ICON_FLAG;
			if (icon > 0)
			{
				icon >>= 12;
				unchecked { id &= (ushort)~ICON_FLAG; }
			}
			return new Decoration(id, (byte)icon, isBackground);
		}
		/* Properties */
		public ushort Id;
		public byte Icon;
		public bool IsBackground;
		/* Class Properties */
		private const ushort BACKGROUND_FLAG = 0x8000;
		private const ushort ICON_FLAG       = 0xF000;
	}
}
