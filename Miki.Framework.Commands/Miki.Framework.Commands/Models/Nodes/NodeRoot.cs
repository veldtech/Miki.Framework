using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Miki.Framework.Events;

namespace Miki.Framework.Commands.Nodes
{
    public class NodeRoot : NodeContainer
    {
        public NodeRoot()
            : base(new CommandMetadata())
        { }

        public override async Task RunAsync(IContext e)
        {
            foreach(var c in Children)
            {
                await c.RunAsync(e);
            }
        }
    }
}
