﻿using System.Collections.Generic;
using System.Linq;

namespace Miki.Framework.Commands
{
    using Miki.Framework.Arguments;
    using Miki.Framework.Commands.Nodes;

	public class CommandTree
	{
		public NodeContainer Root { get; }

        public CommandTree()
		{
			Root = new NodeRoot();
		}

        public Node GetCommand(IArgumentPack pack)
        {
	        var result = GetCommands(pack).FirstOrDefault();

	        if (result.Node == null)
	        {
		        return null;
	        }
	        
	        pack.SetCursor(result.CursorPosition);
	        return result.Node;
        }

        public Node GetCommand(string name)
        {
	        return GetCommand(new ArgumentPack(name.Split(' ')));
        }

		public IEnumerable<NodeResult> GetCommands(IArgumentPack pack)
		{
			return Root.FindCommands(pack);
		}

        public IEnumerable<NodeResult> GetCommands(string name)
        {
	        return GetCommands(new ArgumentPack(name.Split(' ')));
        }
	}
}
