/*
	Junk Jack X: Protocol
	- World Builder

	Written By: Ryan Smith
*/
using System;

namespace JJx.Protocol;

public sealed class WorldBuilder
{
	/* Constructor */
	public WorldBuilder(WorldInfoResponseMessage info)
	{
		this.Info = info;
		this._BlockBuffer = new byte[info.WorldSizeBytes];
	}
	/* Instance Methods */
	public void AddToBlockBuffer(WorldBlocksResponseMessage worldBlocks)
	{
		Array.Copy(worldBlocks.Data, 0, this._BlockBuffer, this._BlockBufferCursor, worldBlocks.Data.Length);
		this._BlockBufferCursor += worldBlocks.Data.Length;
		if (this._BlockBufferCursor == this._BlockBuffer.Length)
			this.IsReady = true;
	}
	public void Build()
	{
		if (!this.IsReady)
			return;
	}
	/* Properties */
	public bool IsReady { get; private set; } = false;
	private readonly WorldInfoResponseMessage Info;
	public WorldSkylineResponseMessage Skyline;  // TODO: Ensure skyline.length = info.size.width
	private byte[] _BlockBuffer;
	private int _BlockBufferCursor = 0;
}
