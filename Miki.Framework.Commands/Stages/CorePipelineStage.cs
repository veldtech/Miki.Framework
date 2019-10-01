using Miki.Discord.Common;
using Miki.Framework.Commands;
using Miki.Framework.Commands.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miki.Framework.Commands
{
	public class CorePipelineStage : IPipelineStage
	{
		public const string MessageArgumentKey = "framework-message";
		public const string QueryArgumentKey = "framework-query";

		public ValueTask CheckAsync(IDiscordMessage msg, IMutableContext e, Func<ValueTask> next)
		{
			e.SetContext(MessageArgumentKey, msg);
			e.SetContext(QueryArgumentKey, msg.Content.Split(' ')
				.ToList());
			return next();
		}
	}
}

namespace Miki.Framework
{
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