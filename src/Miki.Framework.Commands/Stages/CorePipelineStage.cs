namespace Miki.Framework.Commands
{
    using Miki.Discord.Common;
    using Miki.Framework.Commands.Pipelines;
    using System;
    using System.Threading.Tasks;
	
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'CorePipelineStage'
    public class CorePipelineStage : IPipelineStage
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'CorePipelineStage'
	{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'CorePipelineStage.MessageContextKey'
		public static string MessageContextKey = "framework-message";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'CorePipelineStage.MessageContextKey'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'CorePipelineStage.QueryContextKey'
		public static string QueryContextKey = "framework-query";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'CorePipelineStage.QueryContextKey'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'CorePipelineStage.CheckAsync(IDiscordMessage, IMutableContext, Func<ValueTask>)'
		public ValueTask CheckAsync(IDiscordMessage msg, IMutableContext e, Func<ValueTask> next)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'CorePipelineStage.CheckAsync(IDiscordMessage, IMutableContext, Func<ValueTask>)'
		{
			e.SetContext(MessageContextKey, msg);
			e.SetContext(QueryContextKey, msg.Content);
			return next();
		}
	}
}

namespace Miki.Framework
{
    using Commands;
    using Discord.Common;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'CorePipelineStageExtensions'
    public static class CorePipelineStageExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'CorePipelineStageExtensions'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'CorePipelineStageExtensions.GetMessage(IContext)'
		public static IDiscordMessage GetMessage(this IContext context)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'CorePipelineStageExtensions.GetMessage(IContext)'
		{
			return context.GetContext<IDiscordMessage>(CorePipelineStage.MessageContextKey);
        }

        /// <summary>
        /// Mutable version of the query.
        /// </summary>
        public static string GetQuery(this IContext context)
		{
			return context.GetContext<string>(CorePipelineStage.QueryContextKey);
		}

        /// <summary>
        /// Sets the query.
        /// </summary>
        public static void SetQuery(this IMutableContext context, string query)
        {
            context.SetContext(CorePipelineStage.QueryContextKey, query);
        }
    }
}