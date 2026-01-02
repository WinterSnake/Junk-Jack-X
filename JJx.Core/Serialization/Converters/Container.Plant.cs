/*
	Junk Jack X: Core
	- [Serialization]Converter - Plant

	Written By: Ryan Smith
*/

using System.IO;

namespace JJx.Core.Serialization;

internal sealed class PlantConverter : JJxConverter<Plant>
{
	/* Instance Methods */
	public override Plant Read(ref JJxReader reader)
	{
		var id = reader.ReadUInt32();
		var position = (reader.ReadUInt16(), reader.ReadUInt16());
		var isTree = reader.ReadUInt32();
		var isCrop = reader.ReadUInt32();
		// Tree
		if (isTree == 1 && isCrop == 0)
		{
			var tree = new Tree(position, id);
			reader.ReadSpan(tree.Unknown);
			tree.Branches.EnsureCapacity(tree.Unknown[2]);
			for (var i = 0; i < tree.Unknown[2]; ++i)
				tree.Branches.Add(reader.ReadObject<Tree.Branch>());
			return tree;
		}
		// Crop
		else if (isTree == 0 && isCrop == 1)
			return new Crop(position, id);
		throw new InvalidDataException($"Unknown/unhandled plant in converter: [{id}] isTree: {isTree} | isCrop: {isCrop}");
	}
	public override void Write(in Plant @value, JJxWriter writer)
	{
		writer.Write(@value.Id);
		writer.Write(@value.Position.X);
		writer.Write(@value.Position.Y);
		switch (@value)
		{
			case Crop:
			{
				writer.Write((uint)0);
				writer.Write((uint)1);
			} break;
			case Tree tree:
			{
				writer.Write((uint)1);
				writer.Write((uint)0);
				writer.Write(tree.Unknown);
				foreach (var branch in tree.Branches)
					writer.Write(branch);
			} break;
			default: throw new InvalidDataException($"Unknown/unhandled plant in converter: {@value.GetType()}");
		}
	}
}

public sealed class BranchConverter : JJxConverter<Tree.Branch>
{
	/* Instance Methods */
	public override Tree.Branch Read(ref JJxReader reader)
	{
		var branch = new Tree.Branch();
		reader.ReadSpan(branch.Unknown);
		return branch;
	}
	public override void Write(in Tree.Branch @value, JJxWriter writer)
	{
		writer.Write(@value.Unknown);
	}
}
