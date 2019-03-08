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
        public async Task<EventContext> CheckTrigger(EventContext context, IDiscordMessage packet)
        {
            var result = Regex.Match(packet.Content, "^<@!?\\d+> ");
            if(!result.Success)
            {
                return null;
            }

            var ctx = CommandContext.FromMessageContext(context as MessageContext);
            ctx.PrefixUsed = result.Value;
            ctx.Prefix = new PrefixTrigger(ctx.PrefixUsed, false);
            return ctx;
        }
    }
}
