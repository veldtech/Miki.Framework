using System;

namespace Miki.Framework.Commands
{
    using Miki.Discord.Common;
    using Miki.Framework.Commands.Pipelines;
    using System;
    using System.Threading.Tasks;
	
    /// <summary>
    /// Sets up basic values such as query and the intial message entity.
    /// </summary>
    public class CorePipelineStage : IPipelineStage
	{
		public static string MessageContextKey = "framework-message";
		public static string QueryContextKey = "framework-query";

		public ValueTask CheckAsync(IDiscordMessage msg, IMutableContext e, Func<ValueTask> next)
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

    public static class CorePipelineStageExtensions
    {
		public static IDiscordMessage GetMessage(this IContext context)
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