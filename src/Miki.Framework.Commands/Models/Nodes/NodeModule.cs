namespace Miki.Framework.Commands
{
    using System;

    public class NodeModule : NodeContainer
	{
		public NodeModule(string id, IServiceProvider provider, Type t)
			: this(id, (NodeContainer) null, t)
		{ }
		public NodeModule(string id, NodeContainer parent, Type t)
			: base(new CommandMetadata { Identifiers = new[] { id } }, parent, t)
		{ }
	}
}
