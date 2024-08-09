/*
	Junk Jack X: Core
	- [World]Tile.Block

	Written By: Ryan Smith
*/
using System;

namespace JJx;

public partial class Tile
{
	/* Sub-Classes */
	public sealed class Block
	{
		/* Constructor */
		public Block(ushort id, byte icon = 0)
		{
			this.Id = id;
			this.Icon = icon;
		}
		/* Instance Methods */
		internal ushort Pack() => (ushort)((this.Icon << 12) | this.Id);
		/* Static Methods */
		internal static Block Unpack(ushort id)
		{
			var icon = id & ICON_FLAG;
			if (icon > 0)
			{
				icon >>= 12;
				unchecked { id &= (ushort)~ICON_FLAG; }
			}
			return new Block(id, (byte)icon);
		}
		/* Properties */
		public ushort Id;
		public byte Icon;
		/* Class Properties */
		private const ushort ICON_FLAG = 0xF000;
	}
}
