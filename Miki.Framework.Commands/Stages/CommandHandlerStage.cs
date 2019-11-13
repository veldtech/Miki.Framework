﻿namespace Miki.Framework.Commands.Stages
{
    using Miki.Discord.Common;
    using Miki.Framework.Commands.Pipelines;
    using Miki.Logging;
    using System;
    using System.Threading.Tasks;

    public class CommandHandlerStage : IPipelineStage
	{
		private readonly CommandTree map;

        public CommandHandlerStage(CommandTree map)
		{
            this.map = map ?? throw new ArgumentNullException(nameof(map));
		}

		public async ValueTask CheckAsync(IDiscordMessage data, IMutableContext e, Func<ValueTask> next)
		{
			Log.Debug($"Starting command aggregation with query '{e.GetQuery()}'");

			var command = map.GetCommand(e.GetArgumentPack().Pack);
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
    }
}

namespace Miki.Framework.Commands
{
    using Microsoft.Extensions.DependencyInjection;
    using Miki.Framework.Commands.Stages;

    public static class CommandHandlerExtensions
	{
		public static CommandPipelineBuilder UseCommandHandler(
            this CommandPipelineBuilder builder)
		{
			builder.UseStage(new CommandHandlerStage(
                builder.Services.GetService<CommandTree>()));
			return builder;
		}
	}
}
