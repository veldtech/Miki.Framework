namespace Miki.Framework.Commands
{
    using Miki.Discord.Common;
    using Miki.Framework.Commands.Pipelines;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
	
    public class CorePipelineStage : IPipelineStage
	{
		public const string MessageArgumentKey = "framework-message";
		public const string QueryArgumentKey = "framework-query";

		public ValueTask CheckAsync(IDiscordMessage msg, IMutableContext e, Func<ValueTask> next)
		{
			e.SetContext(MessageArgumentKey, msg);
			e.SetContext(QueryArgumentKey, msg.Content.Split(' ').ToList());
			return next();
		}
	}
}

namespace Miki.Framework
{
    using System.Collections.Generic;
    using Commands;
    using Discord.Common;

    public static class CorePipelineStageExtensions
    {
		public static IDiscordMessage GetMessage(this IContext context)
		{
			return context.GetContext<IDiscordMessage>(CorePipelineStage.MessageArgumentKey);
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