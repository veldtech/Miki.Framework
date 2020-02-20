namespace Miki.Framework.Commands.Prefixes
{
    using Miki.Discord.Common;
    using Miki.Framework.Commands.Pipelines;
    using System;
    using System.Threading.Tasks;

    public class PipelineStageTrigger : IPipelineStage
    {
        private readonly IPrefixService service;

        public PipelineStageTrigger(IPrefixService service)
        {
            this.service = service;
        }

        public async ValueTask CheckAsync(IDiscordMessage msg, IMutableContext e, Func<ValueTask> next)
        {
            var result = await service.MatchAsync(e);
            if(result == null)
            {
                return;
            }

            e.SetContext(PipelineBuilderExtensions.PrefixMatchKey, result);
            e.SetQuery(e.GetQuery().Substring(result.Length - 1));
            await next();
        }
    }
}

namespace Miki.Framework.Commands
{
    using Miki.Framework.Commands.Prefixes;
    using Microsoft.Extensions.DependencyInjection;

    public static class PipelineBuilderExtensions
    {
        public const string PrefixMatchKey = "prefix-match";

        public static CommandPipelineBuilder UsePrefixes(
            this CommandPipelineBuilder builder)
        {
            return builder.UseStage(
                new PipelineStageTrigger(
                    builder.Services.GetService<IPrefixService>()));
        }

        public static string GetPrefixMatch(this IContext e)
        {
            return e.GetContext<string>(PrefixMatchKey);
        }
    }
}
