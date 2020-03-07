namespace Miki.Framework.Commands.Stages
{
    using System;
    using System.Threading.Tasks;
    using Miki.Discord.Common;
    using Miki.Framework.Commands.Pipelines;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'FetchDataStage'
    public class FetchDataStage : IPipelineStage
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'FetchDataStage'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'FetchDataStage.ChannelArgumentKey'
        public static string ChannelArgumentKey = "framework-channel";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'FetchDataStage.ChannelArgumentKey'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'FetchDataStage.GuildArgumentKey'
        public static string GuildArgumentKey = "framework-guild";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'FetchDataStage.GuildArgumentKey'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'FetchDataStage.CheckAsync(IDiscordMessage, IMutableContext, Func<ValueTask>)'
        public async ValueTask CheckAsync(IDiscordMessage data, IMutableContext e, Func<ValueTask> next)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'FetchDataStage.CheckAsync(IDiscordMessage, IMutableContext, Func<ValueTask>)'
        {
            var channel = await e.GetMessage().GetChannelAsync();
            if(channel == null)
            {
                throw new InvalidOperationException("This channel is not supported");
            }
            e.SetContext(ChannelArgumentKey, channel);
            if(channel is IDiscordGuildChannel gc)
            {
                e.SetContext(GuildArgumentKey, await gc.GetGuildAsync());
            }
            await next();
        }
    }
}

namespace Miki.Framework
{
    using Miki.Discord.Common;
    using Miki.Framework.Commands.Stages;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'FetchDataStageExtensions'
    public static class FetchDataStageExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'FetchDataStageExtensions'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'FetchDataStageExtensions.GetChannel(IContext)'
        public static IDiscordTextChannel GetChannel(this IContext context)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'FetchDataStageExtensions.GetChannel(IContext)'
        {
            return context.GetContext<IDiscordTextChannel>(FetchDataStage.ChannelArgumentKey);
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'FetchDataStageExtensions.GetGuild(IContext)'
        public static IDiscordGuild GetGuild(this IContext context)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'FetchDataStageExtensions.GetGuild(IContext)'
        {
            return context.GetContext<IDiscordGuild>(FetchDataStage.GuildArgumentKey);
        }
    }
}
