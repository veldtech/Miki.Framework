﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miki.Discord.Common;
using Miki.Framework.Commands.Pipelines;
using Miki.Framework.Events;
using Miki.Framework.Events.Triggers;

namespace Miki.Framework.Commands
{
    public static class PipelineBuilderExtensions
    {
        public const string PrefixMatchKey = "prefix-match";

        public static CommandPipelineBuilder UsePrefix(
            this CommandPipelineBuilder builder, string prefix, bool isDefault, bool editable = false)
        {
            return builder.UseStage(
                new PipelineStageTrigger(
                    new PrefixTrigger(prefix, isDefault, editable)));
        }
        public static CommandPipelineBuilder UsePrefixes(
            this CommandPipelineBuilder builder,
            params ITrigger<IDiscordMessage>[] triggers)
        {
            return builder.UseStage(
                new PipelineStageTrigger(triggers));
        }

        public static string GetPrefixMatch(this IContext e)
        {
            return e.GetContext<string>(PrefixMatchKey);
        }
    }

    public class PipelineStageTrigger : IPipelineStage
    {
        private readonly IEnumerable<ITrigger<IDiscordMessage>> _triggers;

        public PipelineStageTrigger(params ITrigger<IDiscordMessage>[] triggers)
        {
            _triggers = triggers;
        }

        public PrefixTrigger GetDefaultTrigger()
        {
            if(_triggers == null)
            {
                return null;
            }
            return _triggers.Where(x => x is PrefixTrigger)
                .Select(x => x as PrefixTrigger)
                .FirstOrDefault(x => x.IsDefault);
        }

        public async Task CheckAsync(IDiscordMessage msg, IMutableContext e, Func<Task> next)
        {
            foreach(var i in _triggers)
            {
                var m = await i.CheckTriggerAsync(e, msg);
                if (m != null)
                {
                    e.SetContext(PipelineBuilderExtensions.PrefixMatchKey, m);
                    await next();
                    break;
                }
            }
        }
    }
}