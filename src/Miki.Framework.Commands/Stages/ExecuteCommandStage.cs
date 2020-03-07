namespace Miki.Framework.Commands.Stages
{
    using System;
    using System.Threading.Tasks;
    using Miki.Discord.Common;
    using Miki.Framework.Commands.Pipelines;
    using Miki.Logging;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ExecuteCommandStage'
    public class ExecuteCommandStage : IPipelineStage
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ExecuteCommandStage'
    {
        private readonly CommandTree map;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ExecuteCommandStage.ExecuteCommandStage(CommandTree)'
        public ExecuteCommandStage(CommandTree map)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ExecuteCommandStage.ExecuteCommandStage(CommandTree)'
        {
            this.map = map ?? throw new ArgumentNullException(nameof(map));
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ExecuteCommandStage.CheckAsync(IDiscordMessage, IMutableContext, Func<ValueTask>)'
        public async ValueTask CheckAsync(IDiscordMessage data, IMutableContext e, Func<ValueTask> next)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ExecuteCommandStage.CheckAsync(IDiscordMessage, IMutableContext, Func<ValueTask>)'
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