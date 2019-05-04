using Miki.Discord.Common;
using Miki.Framework.Commands;
using Miki.Framework.Commands.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Commands
{
    public class CorePipelineStage : IPipelineStage
    {
        public const string MessageArgumentKey = "framework-message";
        public const string ChannelArgumentKey = "framework-channel";
        public const string GuildArgumentKey = "framework-guild";
        public const string QueryArgumentKey = "framework-query";

        public async Task CheckAsync(IDiscordMessage msg, IMutableContext e, Func<Task> next)
        {
            e.SetContext(MessageArgumentKey, msg);
            e.SetContext(QueryArgumentKey, msg.Content.Split(' ')
                .ToList());
            var channel = await msg.GetChannelAsync() as IDiscordTextChannel;
            e.SetContext(ChannelArgumentKey, channel);
            if (channel is IDiscordGuildChannel gc)
            {
                e.SetContext(GuildArgumentKey, await gc.GetGuildAsync());
            }
            await next();
        }
    }
}

namespace Miki.Framework
{
    public static class ContextExtensions
    {
        public static IDiscordMessage GetMessage(this IContext context)
        {
            return context.GetContext<IDiscordMessage>(CorePipelineStage.MessageArgumentKey);
        }

        public static IDiscordTextChannel GetChannel(this IContext context)
        {
            return context.GetContext<IDiscordTextChannel>(CorePipelineStage.ChannelArgumentKey);
        }

        public static IDiscordGuild GetGuild(this IContext context)
        {
            return context.GetContext<IDiscordGuild>(CorePipelineStage.GuildArgumentKey);
        }
        
        /// <summary>
        /// Mutable version of the query.
        /// </summary>
        public static List<string> GetQuery(this IContext context)
        {
            return context.GetContext<List<string>>(CorePipelineStage.QueryArgumentKey);
        }
    }
}