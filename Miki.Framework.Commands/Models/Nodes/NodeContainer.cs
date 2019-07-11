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
        public object Instance { get; }

        public NodeContainer(CommandMetadata metadata, Type t)
            : base(metadata, t)
        {
        }
        public NodeContainer(CommandMetadata metadata, NodeContainer parent, IServiceProvider provider, Type t)
            : base(metadata, parent, t)
        {
            if (t != null)
            {
                Instance = CommandTreeHelpers.CreateInstance(t, provider);
            }
        }

        public virtual Node FindCommand(IArgumentPack pack)
        {
            if(!pack.CanTake)
            {
                return null;
            }

            var arg = pack.Peek()
                .ToLowerInvariant();

            foreach(var c in Children)
            {
                if (c is NodeContainer nc)
                {
                    var foundNode = nc.FindCommand(pack);
                    if (foundNode != null)
                    {
                        return foundNode;
                    }
                }
                else
                {
                    if (c.Metadata.Identifiers.Any(x => x == arg))
                    {
                        pack.Take();
                        return c;
                    }
                }
            }
            return null;
        }
    }
}
