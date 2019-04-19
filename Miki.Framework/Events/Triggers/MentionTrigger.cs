using Miki.Discord.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Miki.Framework.Events.Triggers
{
    public class MentionTrigger : ITrigger<IDiscordMessage>
    {
        public ValueTask<EventContext> CheckTriggerAsync(EventContext context, IDiscordMessage packet)
        {
            var result = Regex.Match(packet.Content, "^<@!?(\\d+)> ");
            if(!result.Success)
            {
                return new ValueTask<EventContext>(
                    Task.FromResult<EventContext>(null));
            }

            if(context.Self.Id.ToString() != result.Groups[1].Value)
            {
                return new ValueTask<EventContext>(
                    Task.FromResult<EventContext>(null));
            }

            var ctx = CommandContext.FromMessageContext(context as MessageContext);
            ctx.PrefixUsed = result.Value;
            ctx.Prefix = new PrefixTrigger(ctx.PrefixUsed, false);
            return new ValueTask<EventContext>(ctx);
        }
    }
}
