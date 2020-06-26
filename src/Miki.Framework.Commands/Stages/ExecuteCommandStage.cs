using System.Linq;

namespace Miki.Framework.Commands.Stages
{
    using System;
    using System.Threading.Tasks;
    using Miki.Discord.Common;
    using Miki.Framework.Commands.Pipelines;
    using Miki.Logging;

    public class ExecuteCommandStage : IPipelineStage
    {
        private readonly CommandTree map;

        public ExecuteCommandStage(CommandTree map)
        {
            this.map = map ?? throw new ArgumentNullException(nameof(map));
        }

        public async ValueTask CheckAsync(IDiscordMessage data, IMutableContext e, Func<ValueTask> next)
        {
            Log.Debug($"Starting command aggregation with query '{e.GetQuery()}'");

            var pack = e.GetArgumentPack().Pack;
            var results = map.GetCommands(pack)
                .Where(t => t.Node is IExecutable)
                .ToArray();
            
            if (results.Length == 0)
            {
                Log.Warning($"No command was found with query '{string.Join(" ", e.GetQuery())}'");
                return;
            }

            if (results.Length == 1)
            {
                var result = results[0];
                pack.SetCursor(result.CursorPosition);
                e.SetExecutable((IExecutable) result.Node);
            }
            else
            {
                e.SetExecutable(new MultiExecutable(results));
            }
            
            await next();
        }
    }
}