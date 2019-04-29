using Miki.Framework.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Commands
{
    public abstract class Node 
    {
        public CommandMetadata Metadata { get; }
        public NodeContainer Parent { get; }

        public Node(CommandMetadata metadata)
        {
            Metadata = metadata;
        }
        public Node(CommandMetadata metadata, NodeContainer parent)
            : this(metadata)
        {
            if(parent == null)
            {
                throw new InvalidOperationException("Parent cannot be null when explicitly set up.");
            }
            Parent = parent;
        }

        public abstract Task RunAsync(IContext e);

        public override string ToString()
        {
            if(Parent == null
                || string.IsNullOrEmpty(Parent.ToString()))
            {
                return Metadata.Name.ToLowerInvariant();
            }
            return $"{Parent.ToString()}.{Metadata.Name.ToLowerInvariant()}";
        }
    }
}
