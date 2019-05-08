using Miki.Discord.Common;
using Miki.Framework.Arguments;
using Miki.Framework.Commands.Pipelines;
using Miki.Framework.Commands.Stages;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Commands.Stages
{
    public class CommandHandlerStage : IPipelineStage
    {
        private readonly CommandMap _map;

        public CommandHandlerStage(CommandMap map)
        {
            _map = map;
        }

        public async Task CheckAsync(IDiscordMessage data, IMutableContext e, Func<Task> next)
        {
            var command = GetCommand(e.GetArgumentPack().Pack);
            if (command == null)
            {
                return;
            }

            if (command is IExecutable exec)
            {
                e.SetExecutable(exec);
            }
            else
            {
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
        public static CommandPipelineBuilder UseCommandHandler(this CommandPipelineBuilder builder, Assembly assembly)
        {
            return UseCommandHandler(builder, CommandMap.FromAssembly(assembly));
        }
        public static CommandPipelineBuilder UseCommandHandler(this CommandPipelineBuilder builder, CommandMap map)
        {
            builder.UseStage(new CommandHandlerStage(map));
            return builder;
        }
    }
}
