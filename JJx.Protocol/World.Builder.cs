/*
	Junk Jack X: Protocol
	- [Builders]World

	Written By: Ryan Smith
*/

using System;
using System.Buffers;
using System.Diagnostics;
using JJx.Protocol.Packets;

namespace JJx.Protocol;

public sealed class JJxWorldBuilder : IDisposable
{
	/* Constructor */
	public JJxWorldBuilder(
		(ushort width, ushort) size, (ushort, ushort) spawn, (ushort, ushort) player,
		uint worldSizeInBytes
	)
	{
		this.Size = size;
		this.Spawn = spawn;
		this.Player = player;
		this._Skyline = new ushort[size.width];
		this._CompressedData = ArrayPool<byte>.Shared.Rent((int)worldSizeInBytes);
		this._CompressedMemory = this._CompressedData.AsMemory(0, (int)worldSizeInBytes);
	}
	/* Instance Methods */
	public void Build()
	{

	}
	public void Dispose() => ArrayPool<byte>.Shared.Return(this._CompressedData);
	public float PushToCompressedBuffer(ReadOnlySpan<byte> buffer)
	{
		Debug.Assert(this.CurrentSizeInBytes + buffer.Length <= this.TotalSizeInBytes);
		buffer.CopyTo(this._CompressedMemory.Span.Slice(this.CurrentSizeInBytes));
		this.CurrentSizeInBytes += buffer.Length;
		return (float)this.CurrentSizeInBytes / (float)this.TotalSizeInBytes;
	}
	/* Static Methods */
	public static JJxWorldBuilder FromWorldInfo(WorldInfoResponsePacket packet) => new(
		packet.Size,
		packet.Spawn,
		packet.Player,
		packet.WorldSizeInBytes
	);
	/* Properties */
	public readonly (ushort Width, ushort Height) Size;
	public readonly (ushort X, ushort Y) Spawn;
	public readonly (ushort X, ushort Y) Player;
	public Span<ushort> Skyline => this._Skyline;
	private readonly ushort[] _Skyline;
	private readonly byte[] _CompressedData;
	private readonly Memory<byte> _CompressedMemory;
	public int CurrentSizeInBytes { get; private set; } = 0;
	public int TotalSizeInBytes => this._CompressedMemory.Length;
}
