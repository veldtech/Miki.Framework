using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Commands.Nodes
{
    public class NodeRoot : NodeContainer
    {
        public NodeRoot()
            : base(new CommandMetadata())
        { }
    }
}
