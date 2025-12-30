/*
	Junk Jack X: Core
	- [World]Atmosphere

	Written By: Ryan Smith
*/
using System;

namespace JJx.Core;

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
