using Miki.Framework.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Commands
{
    public abstract class Node 
    {
        public CommandMetadata Metadata { get; }
        public NodeContainer Parent { get; }

        public List<ICommandRequirement> Requirements { get; protected set; }

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

        public override string ToString()
        {
            if(Parent == null
                || string.IsNullOrEmpty(Parent.ToString()))
            {
                return Metadata.Identifiers
                    .FirstOrDefault()
                    ?.ToLowerInvariant() ?? null;
            }
            return $"{Parent.ToString()}.{Metadata.Identifiers.FirstOrDefault().ToLowerInvariant()}";
        }
    }
}
