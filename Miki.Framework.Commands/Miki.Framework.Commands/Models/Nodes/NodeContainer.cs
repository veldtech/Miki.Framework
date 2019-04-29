using Miki.Framework.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miki.Framework.Commands
{
    public abstract class NodeContainer : Node
    {
        public List<Node> Children = new List<Node>();

        /// <summary>
        /// Instance object for reflection.
        /// </summary>
        internal object Instance { get; set; }

        public NodeContainer(CommandMetadata metadata)
            : base(metadata) {}
        public NodeContainer(CommandMetadata metadata, NodeContainer parent)
            : base(metadata, parent) {}

        public Node FindCommand(IArgumentPack pack)
        {
            if(!pack.CanTake)
            {
                return null;
            }

            var arg = pack.Peek()
                .ToLowerInvariant();
            foreach(var c in Children)
            {
                if(c.Metadata.Name.ToLowerInvariant() == arg
                    || c.Metadata.Aliases.Any(x => x == arg))
                {
                    return c;
                }
            }
            return null;
        }
    }
}
