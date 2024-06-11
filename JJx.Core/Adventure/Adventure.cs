/*
	Junk Jack X: Core
	- Adventure

	Segment Breakdown:
	-----------------------------------------------------------------------------------------------------------------------------
	:<Info>
	-----------------------------------------------------------------------------------------------------------------------------

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;

namespace JJx;

public sealed class Adventure
{
	/* Constructor */
	public Adventure()
	{

	}
	private Adventure(Guid id)
	{
		// Info
		this.Id = id;
	}
	/* Instance Methods */
	public async Task Save(string fileName)
	{
		using var writer = await ArchiverStream.Writer(fileName, ArchiverStreamType.Adventure);
		await this.ToStream(writer);
	}
	public async Task ToStream(ArchiverStream stream)
	{
		var buffer = new byte[SIZEOF_BUFFER];
		if (stream.Type != ArchiverStreamType.Adventure || !stream.CanWrite)
			throw new ArgumentException($"Expected writeable adventure stream, either incorrect stream type (Type:{stream.Type}) or not writeable (Writeable:{stream.CanWrite})");
	}
	/* Static Methods */
	public static async Task<Adventure> Load(string fileName)
	{
		using var reader = await ArchiverStream.Reader(fileName);
		return await Adventure.FromStream(reader);
	}
	public static async Task<Adventure> FromStream(ArchiverStream stream)
	{
		var bytesRead = 0;
		var buffer = new byte[SIZEOF_BUFFER];
		if (stream.Type != ArchiverStreamType.Adventure || !stream.CanRead)
			throw new ArgumentException($"Expected readable adventure stream, either incorrect stream type (Type:{stream.Type}) or not readable (Readable:{stream.CanRead})");
		return new Adventure();
	}
	/* Properties */
	// Info
	public readonly Guid Id = Guid.NewGuid();
	/* Class Properties */
	private const byte SIZEOF_BUFFER = 56;
}
