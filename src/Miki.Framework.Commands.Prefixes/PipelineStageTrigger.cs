namespace Miki.Framework.Commands.Prefixes
{
    using System;
    using System.Threading.Tasks;
    using Miki.Discord.Common;
    using Miki.Framework.Commands.Pipelines;
    using Miki.Logging;

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
                Log.Debug("No prefix found matched query.");
                return;
            }

            e.SetContext(PipelineBuilderExtensions.PrefixMatchKey, result);
            e.SetQuery(e.GetQuery().Substring(result.Length).TrimStart());
            await next();
        }
    }
}