/*
	Junk Jack X: Core
	- [Archiver]Chunk - Stream

	Written By: Ryan Smith
*/

using System;
using System.IO;

namespace JJx.Core;

public sealed class ChunkStream : Stream
{
	/* Constructor */
	internal ChunkStream()
	{
		this._Parent = new MemoryStream();
		this._IsWriting = true;
	}
	internal ChunkStream(Stream parent, ref readonly ArchiverChunk chunk)
	{
		this.Offset = chunk.Offset;
		this._Parent = parent;
		this._IsWriting = false;
		this._Length = chunk.Length;
	}
    /* Instance Methods */
    protected override void Dispose(bool disposing)
    {
		base.Dispose(disposing);
		if (this._IsWriting) this._Parent.Dispose();
    }
	// Read
    public override int Read(byte[] buffer, int offset, int count) => this.Read(buffer.AsSpan(offset, count));
    public override int Read(Span<byte> buffer)
    {
		if (this._IsWriting)
			return this._Parent.Read(buffer);
		var remaining = this.Length - this.Position;
		if (remaining <= 0)
			return 0;
		var toRead = Math.Min((int)remaining, buffer.Length);
		var read = this._Parent.Read(buffer.Slice(0, toRead));
		this._Position += read;
		return read;
    }
	// Write
    public override void Write(byte[] buffer, int offset, int count) => this.Write(buffer.AsSpan(offset, count));
    public override void Write(ReadOnlySpan<byte> buffer)
    {
		if (!this._IsWriting)
			throw new InvalidOperationException("Tried writing to a read-only chunk stream");
		this._Parent.Write(buffer);
    }
	// Stream
    public override void Flush() => this._Parent.Flush();
    public override void SetLength(long value) => throw new NotSupportedException("Chunk stream does not support setting length");
    public override long Seek(long offset, SeekOrigin origin)
    {
		long position;
		if (this._IsWriting)
		{
			position = this._Parent.Seek(offset, origin);
			if (position > this._Parent.Length)
				this._Parent.SetLength(position);
			return position;
		}
		position = origin switch
		{
			SeekOrigin.Begin => offset,
			SeekOrigin.Current => this.Position + offset,
			SeekOrigin.End => this.Length + offset,
			_ => throw new ArgumentOutOfRangeException(nameof(origin)),
		};
		if (position < 0 || position > this.Length)
			throw new IOException("Attempted to move position outside of chunk stream range");
		this.Position = position;
		return position;
    }
    /* Properties */
	public readonly long Offset;
	private readonly long _Length;
	private readonly Stream _Parent;
	private readonly bool _IsWriting;
	private long _Position = 0;
	// Stream
    public override bool CanRead => true;
    public override bool CanWrite => this._IsWriting;
    public override bool CanSeek => true;
    public override long Position {
		get => this._IsWriting ? this._Parent.Position : this._Position;
		set {
			if (this._IsWriting) this._Parent.Position = value;
			else if (value < 0 || value > this._Length)
				throw new ArgumentOutOfRangeException(nameof(value));
			else
			{
				this._Parent.Position = this.Offset + value;
				this._Position = value;
			}
		}
	}
    public override long Length => this._IsWriting ? this._Parent.Length : this._Length;
}
