using System;

namespace Miki.Framework.Commands
{
    public class NodeModule : NodeContainer
	{
		public NodeModule(string id, IServiceProvider provider, Type t)
			: this(id, null, provider, t)
		{ }
		public NodeModule(string id, NodeContainer parent, IServiceProvider provider, Type t)
			: base(new CommandMetadata { Identifiers = new[] { id } }, parent, provider, t)
		{ }
	}
}
