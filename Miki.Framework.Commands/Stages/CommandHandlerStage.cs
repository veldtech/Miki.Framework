using Miki.Discord.Common;
using Miki.Framework.Arguments;
using Miki.Framework.Commands.Pipelines;
using Miki.Framework.Commands.Stages;
using Miki.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Miki.Framework.Commands.Stages
{
	public class CommandHandlerStage : IPipelineStage
	{
		private readonly CommandTree _map;

		public IEnumerable<Node> Modules
			=> _map.Root.Children;

		public CommandHandlerStage(CommandTree map)
		{
			_map = map;
		}

		public async Task CheckAsync(IDiscordMessage data, IMutableContext e, Func<Task> next)
		{
			Log.Debug($"Starting command aggregation with query '{e.GetQuery()}'");

			var command = GetCommand(e.GetArgumentPack().Pack);
			if(command == null)
			{
				Log.Warning($"No command was found with query '{string.Join(" ", e.GetQuery())}'");
				return;
			}

			if(command is IExecutable exec)
			{
				e.SetExecutable(exec);
				await next();
			}
		}

		public Node GetCommand(string name)
			=> GetCommand(new ArgumentPack(name.Split(' ')));
		public Node GetCommand(IArgumentPack pack)
			=> _map.GetCommand(pack);
	}
}

namespace Miki.Framework.Commands
{
	public static class CommandHandlerExtensions
	{
		public static CommandPipelineBuilder UseCommandHandler(this CommandPipelineBuilder builder, CommandTree map)
		{
			builder.UseStage(new CommandHandlerStage(map));
			return builder;
		}
	}
}
