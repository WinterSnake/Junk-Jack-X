/*
	Junk Jack X: Core
	- [World]Atmosphere

	Written By: Ryan Smith
*/

using System;

namespace JJx.Core;

public enum MapBounds : byte
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
	Terra  = 1 <<  0,
	Seth   = 1 <<  1,
	Alba   = 1 <<  2,
	Xeno   = 1 <<  3,
	Magmar = 1 <<  4,
	Cryo   = 1 <<  5,
	Yuca   = 1 <<  6,
	Lilith = 1 <<  7,
	Thetis = 1 <<  8,
	Mykon  = 1 <<  9,
	Umbra  = 1 << 10,
	Tor    = 1 << 11,
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

public enum Weather : byte
{
	None     = 0x0,
	Rain     = 0x1,
	Snow     = 0x2,
	AcidRain = 0x3,
}

[Flags]
public enum DayPhase : byte
{
	None  = 0x0,
	Day   = 0x1,
	Dusk  = 0x2,
	Night = 0x4,
	Dawn  = 0x8,
}
