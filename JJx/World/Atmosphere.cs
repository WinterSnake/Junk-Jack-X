/*
	Junk Jack X: World
	- Atmosphere

	Written By: Ryan Smith
*/
using System;

namespace JJx;

[Flags]
public enum Planet : uint
{
	Terra  = 0x00000001,
	Seth   = 0x00000002,
	Alba   = 0x00000004,
	Xeno   = 0x00000008,
	Magmar = 0x00000010,
	Cryo   = 0x00000020,
	Yuca   = 0x00000040,
	Lilith = 0x00000080,
	Thetis = 0x00000100,
	Mykon  = 0x00000200,
	Umbra  = 0x00000400,
	Tor    = 0x00000800,
	All    = Terra | Seth | Alba | Xeno | Magmar | Cryo | Yuca | Lilith | Thetis | Mykon | Umbra | Tor
}

[Flags]
public enum Season : byte
{
	Spring = 0x1,
	Summer = 0x2,
	Autumn = 0x4,
	Winter = 0x8,
	None   = 0xF
}

public enum InitSize : byte
{
	Tiny = 0x0,  // Size:  512 * 128
	Small,       // Size:  768 * 256
	Normal,      // Size: 1024 * 256
	Large,       // Size: 2048 * 384
	Huge,        // Size: 4096 * 512
	Custom
}
