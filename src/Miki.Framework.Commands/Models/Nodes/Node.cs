namespace Miki.Framework.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    
	/// <summary>
	/// Represents an internal command node.
	/// </summary>
    public abstract class Node
	{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Node.Metadata'
		public CommandMetadata Metadata { get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Node.Metadata'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Node.Parent'
		public NodeContainer Parent { get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Node.Parent'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Node.Attributes'
		public IReadOnlyCollection<Attribute> Attributes => Type.GetCustomAttributes<Attribute>(false)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Node.Attributes'
				.ToList();

		private MemberInfo Type { get; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Node.Node(CommandMetadata, MemberInfo)'
		protected Node(CommandMetadata metadata, MemberInfo type)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Node.Node(CommandMetadata, MemberInfo)'
		{
			Metadata = metadata;
			Type = type;
		}
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Node.Node(CommandMetadata, NodeContainer, MemberInfo)'
        protected Node(CommandMetadata metadata, NodeContainer parent, MemberInfo type)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Node.Node(CommandMetadata, NodeContainer, MemberInfo)'
			: this(metadata, type)
		{
			Parent = parent ?? throw new InvalidOperationException(
                         "Parent cannot be null when explicitly set up.");
		}

		/// <inheritdoc/>
		public override string ToString()
		{
            if(Metadata.Identifiers == null
               || !Metadata.Identifiers.Any())
			{
				return null;
			}

            var commandId = Metadata.Identifiers.FirstOrDefault();
            if(commandId == null)
            {
				return null;
            }

			if(Parent != null
				&& !string.IsNullOrEmpty(Parent.ToString()))
			{
				return $"{Parent}.{commandId.ToLowerInvariant()}";
			}
            return commandId.ToLowerInvariant();
		}
	}
}
