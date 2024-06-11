/*
	Junk Jack X: World
	- Atmosphere

	Written By: Ryan Smith
*/
using System;

namespace JJx;

public enum SizeType : byte
{
	Tiny   = 0x0,  // Size:  512 * 128
	Small  = 0x1,  // Size:  768 * 256
	Normal = 0x2,  // Size: 1024 * 256
	Large  = 0x3,  // Size: 2048 * 384
	Huge   = 0x4,  // Size: 4096 * 512
	Custom = 0x5,
}

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
	All    = Terra | Seth | Alba | Xeno | Magmar | Cryo | Yuca | Lilith | Thetis | Mykon | Umbra | Tor,
}

[Flags]
public enum Season : byte
{
	Spring = 0x1,
	Summer = 0x2,
	Autumn = 0x4,
	Winter = 0x8,
	None   = 0xF,
}

public enum Period : byte
{
	None  = 0x0,
	Day   = 0x1,
	Dusk  = 0x2,
	Night = 0x4,
	Dawn  = 0x8,
}

public enum Weather : byte
{
	None     = 0x0,
	Rain     = 0x1,
	Snow     = 0x2,
	AcidRain = 0x3,
}
