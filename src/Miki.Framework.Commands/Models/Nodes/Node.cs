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
		public CommandMetadata Metadata { get; }
		public NodeContainer Parent { get; }

        public IReadOnlyCollection<Attribute> Attributes => Type.GetCustomAttributes<Attribute>(false)
				.ToList();

		private MemberInfo Type { get; }

		protected Node(CommandMetadata metadata, MemberInfo type)
		{
			Metadata = metadata;
			Type = type;
		}

        protected Node(CommandMetadata metadata, NodeContainer parent, MemberInfo type)
            : this(metadata, type)
        {
            Parent = parent ?? throw new InvalidOperationException("Parent cannot be null when explicitly set up.");
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
