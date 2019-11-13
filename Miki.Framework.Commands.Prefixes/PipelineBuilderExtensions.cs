namespace Miki.Framework.Commands.Prefixes
{
    using Miki.Discord.Common;
    using Miki.Framework.Commands.Pipelines;
    using System;
    using System.Threading.Tasks;

    public class PipelineStageTrigger : IPipelineStage
    {
        private readonly PrefixService<IDiscordMessage> service;

        public PipelineStageTrigger(PrefixService<IDiscordMessage> service)
        {
            this.service = service;
        }

        public async ValueTask CheckAsync(IDiscordMessage msg, IMutableContext e, Func<ValueTask> next)
        {
            var result = await service.MatchAsync(e, msg);
            if(result == null)
            {
                return;
            }

            e.SetContext(PipelineBuilderExtensions.PrefixMatchKey, result);
            e.GetQuery()[0] = e.GetQuery()[0].Substring(result.Length - 1);
            await next();
        }
    }
}

namespace Miki.Framework.Commands
{
    using Miki.Framework.Commands.Prefixes;
    using Microsoft.Extensions.DependencyInjection;
    using Miki.Discord.Common;

    public static class PipelineBuilderExtensions
    {
        public const string PrefixMatchKey = "prefix-match";

        public static CommandPipelineBuilder UsePrefixes(
            this CommandPipelineBuilder builder)
        {
            return builder.UseStage(
                new PipelineStageTrigger(
                    builder.Services.GetService<PrefixService<IDiscordMessage>>()));
        }

        public static string GetPrefixMatch(this IContext e)
        {
            return e.GetContext<string>(PrefixMatchKey);
        }
    }
}
