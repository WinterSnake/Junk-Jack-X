/*
	Junk Jack X: Utilities
	- Segment

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Threading.Tasks;

namespace JJx.Utils;

internal class Segment
{
	/* Constructors */
	internal Segment(Type id, uint location, uint size)
	{
		this.Id = id;
		this.Location = location;
		this.Size = size;
	}
	/* Static Methods */
	public async Task ToStream(Stream stream)
	{
		var workingData = new byte[SIZE];
		Utils.ByteConverter.Write(new Span<byte>(workingData), (uint)this.Id, 0);
		Utils.ByteConverter.Write(new Span<byte>(workingData), this.Location, 4);
		Utils.ByteConverter.Write(new Span<byte>(workingData), this.Size,     8);
		await stream.WriteAsync(workingData, 0, workingData.Length);
	}
	/* Instance Methods */
	public static async Task<Segment> FromStream(Stream stream)
	{
		var bytesRead = 0;
		var workingData = new byte[SIZE];
		while (bytesRead < SIZE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, SIZE - bytesRead);
		var id       = Utils.ByteConverter.GetUInt32(new Span<byte>(workingData),  0);
		var location = Utils.ByteConverter.GetUInt32(new Span<byte>(workingData),  4);
		var size     = Utils.ByteConverter.GetUInt32(new Span<byte>(workingData),  8);
		if (!Enum.IsDefined(typeof(Type), id))
		{
			throw new ArgumentException($"Unknown segment id: '{id:X8}'");
		}
		return new Segment((Type)id, location, size);
	}
	/* Properties */
	public readonly Type Id;
	public readonly uint Location;
	public readonly uint Size;
	/* Class Properties */
	private const byte SIZE = 12;
	/* Sub-Classes */
	public enum Type
	{
		
	}
}
