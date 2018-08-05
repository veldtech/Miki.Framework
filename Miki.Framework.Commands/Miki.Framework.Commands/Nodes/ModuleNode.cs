using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Miki.Discord.Common;
using Miki.Framework.Events;

namespace Miki.Framework.Commands.Nodes
{
	public class ModuleNode : Node
	{
		public ModuleNode(string name) : base(name)
		{
		}

		public override async Task ExecuteAsync(CommandContext context)
		{
			QueryObject query = context.Get<QueryObject>(ContextConstants.Query);

			foreach(Node node in Nodes)
			{
				if (await node.IsCompatibleAsync(context))
				{
					await node.ExecuteAsync(context);
					break;
				}
			}
		}

		public override async Task<bool> IsCompatibleAsync(CommandContext context)
		{
			foreach(Node node in Nodes)
			{
				if(await node.IsCompatibleAsync(context))
				{
					return true;
				}
			}
			return false;
		}

		public override Task<bool> IsEnabledAsync(CommandContext channel)
		{
			throw new NotImplementedException();
		}
	}
}
