/*
	Junk Jack X: World
	- Adventure

	Segment Breakdown:
	------------------------------------------------------------------------------------------------------------------------
	Segment[0x0   :   0x3]      = JJ World Header  | Length: 4   (0x04)  | Type: char[4]
	Segment[0x4   :  0x23]      = UNKNOWN FOR NOW  | Length: 31  (0x1F)  | Type: ??? Possible Header/File Length/CRC
	Segment[0x24  :  0x33]      = UUID             | Length: 16  (0x10)  | Type: uuid
	Segment[0x34  :  0x3C]      = UNKNOWN FOR NOW  | Length: 8   (0x8)   | Type: ??? Possible long/epoch/DateTime
	Segment[0x3D  :  0x4B]      = Name             | Length: 16  (0x10)  | Type: char*
	Segment[0x4C  : 0x---]      = UNKNOWN FOR NOW  | Length: ?   (0x?)   | Type: ???
	------------------------------------------------------------------------------------------------------------------------

	Written By: Ryan Smith
*/
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace JJx;

public sealed class Adventure
{
	/* Constructors */
	public Adventure(string name)
	{
		this.Id = Guid.NewGuid();
		this.Name = name;
	}
	private Adventure(Guid id, string name)
	{
		this.Id = id;
		this._Name = name;
	}
	/* Instance Methods */
	public async Task ToStream(Stream stream)
	{
		var byteCount = 0;
		var workingData = new byte[BUFFERSIZE];
		//----Unknown----\\
		stream.Seek(0x1F, SeekOrigin.Current);
		// Uuid
		var uuid = this.Id.ToByteArray();
		await stream.WriteAsync(uuid, 0, uuid.Length);
		//----Unknown----\\
		stream.Seek(0x8, SeekOrigin.Current);
		// Name
		byteCount = Encoding.ASCII.GetBytes(this._Name, 0, this._Name.Length, workingData, 0);
		for (var i = byteCount; i < NAMESIZE; ++i) { workingData[i] = 0; }
		await stream.WriteAsync(workingData, 0, NAMESIZE);
		//----Unknown----\\
	}
	/* Static Methods */
	public static async Task<Adventure> FromStream(Stream stream)
	{
		var bytesRead = 0;
		var workingData = new byte[BUFFERSIZE];
		//----Unknown----\\
		stream.Seek(0x24, SeekOrigin.Current);
		Console.WriteLine(stream.Position.ToString("x2"));
		// Uuid
		bytesRead = 0;
		while (bytesRead < UUIDSIZE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, UUIDSIZE - bytesRead);
		Guid id = new Guid(new Span<byte>(workingData).Slice(0, UUIDSIZE));
		//----Unknown----\\
		stream.Seek(0x8, SeekOrigin.Current);
		// Name
		bytesRead = 0;
		while (bytesRead < NAMESIZE)
			bytesRead += await stream.ReadAsync(workingData, bytesRead, NAMESIZE - bytesRead);
		var name = Encoding.ASCII.GetString(
			new Span<byte>(workingData).Slice(0, Array.IndexOf(workingData, byte.MinValue))
		);
		//----Unknown----\\
		return new Adventure(id, name);
	}
	/* Properties */
	public readonly Guid Id;
	private string _Name;
	public string Name {
		// Min Length: 1 | Max length: 15 characters -- 16 = null termination
		get { return this._Name; }
		set {
			if (String.IsNullOrEmpty(value)) return;
			else if (value.Length < NAMESIZE) this._Name = value;
			else this._Name = value.Substring(0, NAMESIZE - 1);
		}
	}
	/* Class Properties */
	private const byte BUFFERSIZE = 32;
	private const byte UUIDSIZE = 16;
	private const byte NAMESIZE = 16;
}
