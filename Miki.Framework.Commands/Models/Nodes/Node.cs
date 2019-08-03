using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Commands
{
    public abstract class Node 
    {
        public CommandMetadata Metadata { get; }
        public NodeContainer Parent { get; }

        public IReadOnlyCollection<Attribute> Attributes => _type.GetCustomAttributes(false)
                .OfType<Attribute>()
                .ToList();

        private MemberInfo _type { get; }

        public Node(CommandMetadata metadata, MemberInfo type)
        {
            Metadata = metadata;
            _type = type;
        }
        public Node(CommandMetadata metadata, NodeContainer parent, MemberInfo type)
            : this(metadata, type)
        {
            Parent = parent ?? throw new InvalidOperationException("Parent cannot be null when explicitly set up.");
        }

        public override string ToString()
        {
            if (Metadata.Identifiers == null
                || Metadata.Identifiers.Count() == 0)
            {
                return null;
            }

            if (Parent != null
                && !string.IsNullOrEmpty(Parent.ToString()))
            {
                return $"{Parent.ToString()}.{Metadata.Identifiers.FirstOrDefault().ToLowerInvariant()}";
            }

            return Metadata.Identifiers
                .FirstOrDefault()
                .ToLowerInvariant();
        }
    }
}
