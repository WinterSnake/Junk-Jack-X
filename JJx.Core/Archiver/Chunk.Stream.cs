/*
   Junk Jack X: Core
   - [Archiver]Chunk Streams

	Written By: Ryan Smith
*/

using System;
using System.IO;

namespace JJx.Core;

internal sealed class ChunkReaderStream : Stream
{
	/* Constructor */
	public ChunkReaderStream(Stream parent, ref readonly ArchiverChunk chunk)
	{
		this.Offset = chunk.Offset;
		this._Parent = parent;
		this._Length = chunk.Length;
	}
	/* Instance Methods */
    protected override void Dispose(bool disposing) { }
    public override void Flush() => this._Parent.Flush();
    public override int Read(byte[] buffer, int offset, int count) => this.Read(buffer.AsSpan(offset, count));
    public override int Read(Span<byte> buffer)
    {
		var remaining = this.Length - this.Position;
		if (remaining <= 0)
			return 0;
		var toRead = Math.Min((int)remaining, buffer.Length);
		var read = this._Parent.Read(buffer[..toRead]);
		this._Position += read;
		return read;
	}
    public override void Write(byte[] buffer, int offset, int count)
		=> throw new InvalidOperationException("Tried writing to a read-only chunk stream.");
    public override void SetLength(long value) => throw new NotSupportedException("Chunk stream does not support setting length");
    public override long Seek(long offset, SeekOrigin origin)
	{
		var position = origin switch
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
    public override bool CanRead => true;
    public override bool CanWrite => false;
    public override bool CanSeek => this._Parent.CanSeek;
    public override long Length => this._Length;
    public override long Position {
		get => this._Position;
		set {
			if (value < 0 || value > this.Length)
				throw new ArgumentOutOfRangeException(nameof(value));
			this._Parent.Position = this.Offset + value;
			this._Position = value;
		}
	}
	public readonly long Offset;
	private readonly Stream _Parent;
	private readonly long _Length;
	private long _Position = 0;
}

internal sealed class ChunkWriterStream : Stream
{
    /* Constructor */
	public ChunkWriterStream(Stream parent) => this._Parent = parent;
    /* Instance Methods */
    protected override void Dispose(bool disposing) => this._Parent.Dispose();
    public override void Flush() => this._Parent.Flush();
    public override int Read(byte[] buffer, int offset, int count)
		=> throw new InvalidOperationException("Tried reading to a write-only chunk stream.");
    public override void Write(byte[] buffer, int offset, int count) => this._Parent.Write(buffer, offset, count);
    public override void Write(ReadOnlySpan<byte> buffer) => this._Parent.Write(buffer);
    public override void SetLength(long value) => throw new NotSupportedException("Chunk stream does not support setting length");
    public override long Seek(long offset, SeekOrigin origin)
	{
		var position = this._Parent.Seek(offset, origin);
		if (position > this._Parent.Length)
			this._Parent.SetLength(position);
		return position;
	}
	/* Properties */
    public override bool CanRead => false;
    public override bool CanWrite => true;
    public override bool CanSeek => this._Parent.CanSeek;
    public override long Length => this._Parent.Length;
    public override long Position { get => this._Parent.Position; set => this._Parent.Position = value; }
	private readonly Stream _Parent;
}
