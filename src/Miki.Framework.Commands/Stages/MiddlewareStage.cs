namespace Miki.Framework.Commands.Stages
{
    using Miki.Discord.Common;
    using Miki.Framework.Commands.Pipelines;
    using System;
    using System.Threading.Tasks;

    public class MiddlewareStage : IPipelineStage
	{
		private readonly Func<IDiscordMessage, IMutableContext, Func<ValueTask>, ValueTask> fn;

		public MiddlewareStage(Func<IDiscordMessage, IMutableContext, Func<ValueTask>, ValueTask> fn)
		{
			this.fn = fn;
		}

		public ValueTask CheckAsync(IDiscordMessage data, IMutableContext e, Func<ValueTask> next)
		{
			if(fn != null)
			{
				return fn(data, e, next);
			}
            return default;
		}
	}
}

namespace Miki.Framework
{
    using System;
    using System.Threading.Tasks;
    using Miki.Discord.Common;
    using Miki.Framework.Commands;
    using Miki.Framework.Commands.Stages;

    public static class ContextExtensions
    {
		public static CommandPipelineBuilder UseStage(this CommandPipelineBuilder b,
			Func<IDiscordMessage, IMutableContext, Func<ValueTask>, ValueTask> fn)
		{
			return b.UseStage(new MiddlewareStage(fn));
		}
	}
}