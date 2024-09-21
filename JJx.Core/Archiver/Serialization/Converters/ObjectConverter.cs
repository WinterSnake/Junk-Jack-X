/*
	Junk Jack X: Core
	- [Serializer.Converters]JJx Object

	Written By: Ryan Smith
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace JJx.Serialization;

// Attributes
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple=false)]
internal sealed class JJxObjectAttribute : Attribute
{
	/* Constructor */
	public JJxObjectAttribute(ulong size = 0, bool @static = true)
	{
		this.Size = size;
		this.Static = @static;
	}
	/* Properties */
	public ulong Size;
	public readonly bool Static;
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple=false)]
internal class JJxDataAttribute : Attribute
{
	/* Constructor */
	public JJxDataAttribute(ulong position)
	{
		this.Position = position;
	}
	/* Properties */
	public readonly ulong Position;
}

// Converters
internal sealed class JJxObjectConverterFactory : JJxConverterFactory
{
	/* Instance Methods */
	#nullable enable
	public override JJxConverter Build(Type type, params object?[]? properties)
	{
		Debug.Assert(properties == null, "JJxObjectConverterFactory.Build should have null properties");
		var objectData = type.GetCustomAttributes(typeof(JJxObjectAttribute), false)[0] as JJxObjectAttribute;
		Type typeConverter = typeof(JJxObjectConverter<>).MakeGenericType(type);
		return base._CreateConverter(
			type, typeConverter,
			// Parameters
			(typeof(ulong), objectData!.Size),
			(typeof(bool), objectData!.Static)
		);
	}
	#nullable disable
	public override bool CanConvert(Type type) => type.GetCustomAttributes(typeof(JJxObjectAttribute), false).Length == 1;
	/* Properties */
	public override bool Cache => true;
}

// TODO: Cache
internal sealed class JJxObjectConverter<T> : JJxConverter<T>
{
	/* Constructor */
	public JJxObjectConverter(ulong size, bool @static)
	{
		if (!@static) this._Buffer = null;
		else if (size > 0) this._Buffer = new byte[size];
		else this._Buffer = Array.Empty<byte>();
	}
	/* Instance Methods */
	public override T Read(JJxReader reader)
	{
		var obj = RuntimeHelpers.GetUninitializedObject(this.Type);
		// Get unsorted fields and sort them
		var fieldsUnsorted = this.Type.GetFields(
			BindingFlags.Public
			| BindingFlags.NonPublic
			| BindingFlags.Instance
		);
		var fieldsSorted = new FieldInfo[fieldsUnsorted.Length];
		for (var i = 0; i < fieldsSorted.Length; ++i)
		{
			var field = fieldsUnsorted[i];
			var dataAttributes = (JJxDataAttribute[])field.GetCustomAttributes(typeof(JJxDataAttribute), false);
			if (dataAttributes.Length == 0) continue;
			var dataAttribute = dataAttributes[0];
			fieldsSorted[dataAttribute.Position] = field;
		}
		// TODO: Pull into inline buffer
		// TODO: Calculate size for non-set static objects appropriately
		var startPosition = reader.BaseStream.Position;
		// Read values and write to object
		foreach (var field in fieldsSorted)
		{
			if (field == null) break;
			// Primitive handling
			if (field.FieldType.IsPrimitive)
			{
				var typeCode = Type.GetTypeCode(field.FieldType);
				switch (typeCode)
				{
					case TypeCode.Boolean: field.SetValue(obj, reader.GetBool());   break;
					case TypeCode.Byte:    field.SetValue(obj, reader.GetUInt8());  break;
					case TypeCode.UInt16:  field.SetValue(obj, reader.GetUInt16()); break;
					case TypeCode.UInt32:  field.SetValue(obj, reader.GetUInt32()); break;
					default: throw new ArgumentException($"Unhandled primitive typecode '{typeCode}' in JJxObjectConverter");
				}
				continue;
			}
			// Non-primitive handling
			var converter = JJxConverter.GetTypeConverter(field.FieldType);
			var readMethod = converter.GetType().GetMethod("Read");
			field.SetValue(obj, readMethod.Invoke(converter, new[] { (object)reader }));
		}
		var calculatedLength = reader.BaseStream.Position - startPosition;
		// HACK: Compute calculated length to subtract is less than buffer -- required until using internal buffer
		var remainingLength = (int)(this.Size - calculatedLength);
		if (remainingLength > 0)
			reader.GetBytes(remainingLength);
		return (T)obj;
	}
	public override void Write(JJxWriter writer, object @value) => throw new NotImplementedException($"JJxObjectConverter does not implement a Write");
	/* Properties */
	public bool Static => this._Buffer != null;
	public long Size => this._Buffer?.Length ?? 0;
	#nullable enable
	private byte[]? _Buffer;
	#nullable disable
}
