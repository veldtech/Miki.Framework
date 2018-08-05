using Miki.Discord.Common;
using Miki.Framework.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Commands
{
	/// <summary>
	/// Represents a basic node
	/// </summary>
	public abstract class Node
	{
		/// <summary>
		/// The node name
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// The node depth
		/// </summary>
		public int Depth { get; private set; }

		/// <summary>
		/// Child nodes
		/// </summary>
		protected List<Node> Nodes { get; private set; }

		public Node(string name)
		{
			Name = name;
		}

		public void AddChild(Node node)
		{
			node.Depth = Depth + 1;
			Nodes.Add(node);
		}

		/// <summary>
		/// Executes the node.
		/// </summary>
		public abstract Task ExecuteAsync(CommandContext context);

		/// <summary>
		/// Checks if the current node can be used in the current context
		/// </summary>
		public abstract Task<bool> IsCompatibleAsync(CommandContext context);

		/// <summary>
		/// Checks if the current node is enabled on the channel.
		/// </summary>
		public abstract Task<bool> IsEnabledAsync(CommandContext context);
    }
}
