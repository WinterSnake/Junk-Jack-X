/*
	Junk Jack X: Protocol
	- Message Headers

	Written By: Ryan Smith
*/

namespace JJx.Protocol;

internal enum MessageHeader : ushort
{
	// Primary Type
	Management = (0x00 << 8),
	WorldData  = (0x03 << 8),
	// Sub-Type: Management
	LoginRequest         = Management | 0x02,
	LoginSuccess         = Management | 0x03,
	ListRequest          = Management | 0x06,
	ListResponse         = Management | 0x07,
	WorldRequest         = Management | 0x09,
	WorldProgress        = Management | 0x0A,
	LoginFailure         = Management | 0x0C,
	// Sub-Type: World-Data
	WorldInfoResponse    = WorldData | 0x43,
	WorldBlocksResponse  = WorldData | 0x47,
	WorldSkylineResponse = WorldData | 0x4C,
}

