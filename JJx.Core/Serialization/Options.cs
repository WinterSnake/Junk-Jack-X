/*
	Junk Jack X: Core
	- [Serialization]Options

	Written By: Ryan Smith
*/

using System;
using System.Collections.Generic;

namespace JJx.Core.Serialization;

public sealed class JJxSerializationOptions
{
	/* Constructor */
	static JJxSerializationOptions()
	{
		Default = new();
		Default.AddConverter<EnumFactoryConverter>();
		Default.AddConverter<ArchiverChunkConverter>(typeof(ArchiverChunk));
		Default.AddConverter<GuidConverter>(typeof(Guid));
		Default.AddConverter<DateTimeConverter>(typeof(DateTime));
		Default.AddConverter<ItemConverter>(typeof(Item));
		Default.AddConverter<EffectConverter>(typeof(Effect));
		Default.AddConverter<TileConverter>(typeof(Tile));
		// Containers
		Default.AddConverter<ChestConverter>(typeof(Chest));
		Default.AddConverter<ForgeConverter>(typeof(Forge));
		Default.AddConverter<SignConverter>(typeof(Sign));
		Default.AddConverter<StableConverter>(typeof(Stable));
		Default.AddConverter<LabConverter>(typeof(Lab));
		Default.AddConverter<ShelfConverter>(typeof(Shelf));
		Default.AddConverter<PlantConverter>();
		Default.AddConverter<BranchConverter>(typeof(Tree.Branch));
		Default.AddConverter<FruitConverter>(typeof(Fruit));
		Default.AddConverter<DecayConverter>(typeof(Decay));
		Default.AddConverter<LockConverter>(typeof(Lock));
		Default.AddConverter<EntityConverter>(typeof(Entity));
		Default._IsReadOnly = true;
	}
	/* Instance Methods */
	public void AddConverter<T>()
		where T: JJxConverter, new()
	{
		if (this._IsReadOnly)
			throw new InvalidOperationException("Tried to add a converter when in read-only mode");
		this._Converters.Add(new T());
	}
	public void AddConverter<T>(Type lookupType)
		where T: JJxConverter, new()
	{
		if (this._IsReadOnly)
			throw new InvalidOperationException("Tried to add a converter when in read-only mode");
		this._ConverterCache[lookupType] = new T();
	}
	public JJxConverter<T> GetConverter<T>()
	{
		if (this._ConverterCache.TryGetValue(typeof(T), out var cachedConverter))
			return (JJxConverter<T>)cachedConverter;
		var type = typeof(T);
		while (type != null)
		{
			foreach (var converter in this._Converters)
			{
				if (!converter.CanSupportType(type)) continue;
				JJxConverter actual = converter;
				if (converter is JJxConverterFactory factory)
					actual = factory.Build(type);
				this._ConverterCache[type] = actual;
				return (JJxConverter<T>)actual;
			}
			type = type.BaseType;
		}
		throw new InvalidOperationException($"JJx format does not support the type '{typeof(T).Name}'");
	}
	/* Properties */
	private bool _IsReadOnly = false;
	private readonly List<JJxConverter> _Converters = new();
	private readonly Dictionary<Type, JJxConverter> _ConverterCache = new();
	/* Class Properties */
	public static readonly JJxSerializationOptions Default;
}
