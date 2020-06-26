﻿namespace Miki.Framework.Commands
{
    using Microsoft.Extensions.DependencyInjection;
    using Miki.Framework.Arguments;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    public abstract class NodeContainer : Node
	{
		public List<Node> Children = new List<Node>();

		public NodeContainer(CommandMetadata metadata, Type t)
			: base(metadata, t)
		{
		}
		public NodeContainer(CommandMetadata metadata, NodeContainer parent, Type t)
			: base(metadata, parent, t)
		{
		}

		public virtual IEnumerable<NodeResult> FindCommands(IArgumentPack pack)
		{
			if (!pack.CanTake)
			{
				yield break;
			}

			var arg = pack.Peek().Unwrap();

			// Take if this module starts.
			var hasPrefix = Metadata.Identifiers != null
			                && Metadata.Identifiers.Count > 0
			                && Metadata.Identifiers.Any(x => string.Equals(x, arg, StringComparison.InvariantCultureIgnoreCase));
			
			if (hasPrefix)
			{
				pack.SetCursor(pack.Cursor + 1);
			}

			foreach(var child in Children)
			{
				if (child is NodeContainer nc)
				{
					foreach (var node in nc.FindCommands(pack))
					{
						yield return node;
					}
				}
				else
				{
					if (child.Metadata.Identifiers.Any(x => x == arg))
					{
						yield return new NodeResult(child, pack.Cursor + 1);
					}
				}
			}
			
			if (hasPrefix)
			{
				pack.SetCursor(pack.Cursor - 1);
			}
		}
	}
}
