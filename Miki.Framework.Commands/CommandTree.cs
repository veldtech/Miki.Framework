using Miki.Framework.Arguments;
using Miki.Framework.Commands.Attributes;
using Miki.Framework.Commands.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Commands
{
    public class CommandTree
    {
        public NodeContainer Root { get; }

        public CommandTree()
        {
            Root = new NodeRoot();
        }

        public Node GetCommand(IArgumentPack pack)
        {
            return Root.FindCommand(pack);
        }
    }
}
