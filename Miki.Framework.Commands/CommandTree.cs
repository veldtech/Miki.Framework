
using Miki.Framework.Arguments;
using Miki.Framework.Commands.Nodes;

namespace Miki.Framework.Commands
{
	public class CommandTree
	{
		public NodeContainer Root { get; }

		public CommandTree()
		{
			Root = new NodeRoot();
		}

		public Node GetCommand(IArgumentPack pack)
		{
			return Root.FindCommand(pack);
		}
	}
}
