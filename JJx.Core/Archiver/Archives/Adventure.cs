/*
	Junk Jack X: Core
	- [Archive]Adventure

	Written By: Ryan Smith
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using JJx.Core.Serialization;

namespace JJx.Core;

public sealed class AdventureArchive : IArchive
{
	/* Constructor */
	public AdventureArchive(Adventure adventure) :this(adventure, null) => this._ArePortalsLoaded = true;
	private AdventureArchive(Adventure adventure, IArchiveReader? reader)
	{
		this._Adventure = adventure;
		this._Reader = reader;
	}
	/* Instance Methods */
	public void Dispose() => this._Reader?.Dispose();
	// Reading
	private void _Load()
	{
		this._LoadPortals();
		this._Reader!.Dispose();
	}
	private void _LoadPortals()
	{
		if (this._ArePortalsLoaded) return;
		using (var chunk = this._Reader!.GetChunkStream(ArchiverChunkType.AdventurePortals))
		{
			var reader = new JJxReader(chunk);
			var count = reader.ReadInt32();
			this._Adventure.Portals.EnsureCapacity(count);
			for (var i = 0; i < count; ++i)
				this._Adventure.Portals.Add(reader.ReadObject<Portal>());
		}
		this._ArePortalsLoaded = true;
	}
	// Writing
	public void Write(IArchiveWriter writer)
	{
		if (!this.IsFullyLoaded) this._Load();
		Stream chunk;
		// Info
		chunk = writer.WriteChunk(ArchiverChunkType.WorldInfo);
		{
			var streamWriter = new JJxWriter(chunk);
			streamWriter.Write(this.Id);
			streamWriter.Write(this.LastPlayed);
			streamWriter.Write(this.Version);
			streamWriter.Write(this.Name, SIZEOF_NAME);
			streamWriter.Write(this.Author, SIZEOF_AUTHOR);
			streamWriter.Write(this.Size.Width);
			streamWriter.Write(this.Size.Height);
			streamWriter.Write((ushort)0);
			streamWriter.Write((ushort)0);
			streamWriter.Write((ushort)0);
			streamWriter.Write((ushort)0);
			streamWriter.Write(this.Planet);
			streamWriter.Write((byte)0);
			streamWriter.Write(Gamemode.Adventure);
			streamWriter.Write(this.SizeBounds);
			streamWriter.Write(this.SkyBounds);
			streamWriter.Skip(4); // Unknown (likely padding)
			streamWriter.Skip(sizeof(uint) * 32); // Padding
		}
		// Portal
		chunk = writer.WriteChunk(ArchiverChunkType.AdventurePortals);
		{
			var streamWriter = new JJxWriter(chunk);
			streamWriter.Write(this.Portals.Count);
			foreach (var portal in CollectionsMarshal.AsSpan(this.Portals))
				streamWriter.Write(portal);
		}
	}
	/* Static Methods */
	internal static AdventureArchive Load(IArchiveReader reader, bool eagerLoad = false)
	{
		Adventure adventure;
		using (var chunk = reader.GetChunkStream(ArchiverChunkType.WorldInfo))
		{
			var streamReader = new JJxReader(chunk);
			var guid = streamReader.ReadObject<Guid>();
			var lastPlayed = streamReader.ReadObject<DateTime>();
			var version = streamReader.ReadObject<Version>();
			var name = streamReader.ReadString(32);
			var author = streamReader.ReadString(16);
			var size = (
				streamReader.ReadUInt16(),
				streamReader.ReadUInt16()
			);
			// Player [No-Op]
			streamReader.ReadUInt16();
			streamReader.ReadUInt16();
			// Spawn [No-Op]
			streamReader.ReadUInt16();
			streamReader.ReadUInt16();
			var planet = streamReader.ReadObject<Planet>();
			// Season [No-Op]
			streamReader.ReadUInt8();
			// Gamemode [No-Op]
			streamReader.ReadUInt8();
			var sizeBounds = streamReader.ReadObject<MapBounds>();
			var skyBounds = streamReader.ReadObject<MapBounds>();
			streamReader.Skip(4); // Unknown (likely padding)
			streamReader.Skip(sizeof(uint) * 32); // Padding
			adventure = new(
				guid, version, lastPlayed, name, author,
				planet, size, sizeBounds, skyBounds
			);
		}
		var archive = new AdventureArchive(adventure, reader);
		if (eagerLoad) archive._Load();
		return archive;
	}
	/* Properties */
	internal bool IsFullyLoaded => true;
	private readonly IArchiveReader? _Reader;
	internal readonly Adventure _Adventure;
	// Info
	public Guid Id => this._Adventure.Id;
	public Version Version => this._Adventure.Version;
	public DateTime LastPlayed { get => this._Adventure.LastPlayed; set => this._Adventure.LastPlayed = value; }
	public string Name { get => this._Adventure.Name; set => this._Adventure.Name = value; }
	public string Author { get => this._Adventure.Author; set => this._Adventure.Author = value; }
	public Planet Planet { get => this._Adventure.Planet; set => this._Adventure.Planet = value; }
	public MapBounds SizeBounds { get => this._Adventure.SizeBounds; set => this._Adventure.SizeBounds = value; }
	public MapBounds SkyBounds { get => this._Adventure.SkyBounds; set => this._Adventure.SkyBounds = value; }
	public (ushort Width, ushort Height) Size => this._Adventure.Size;
	// Portal
	private bool _ArePortalsLoaded = false;
	public List<Portal> Portals { get { this._LoadPortals(); return this._Adventure.Portals; } }
	/* Class Properties */
	internal const int SIZEOF_NAME   = 32;
	internal const int SIZEOF_AUTHOR = 16;
}
