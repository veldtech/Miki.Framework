namespace Miki.Framework.Commands.Stages
{
    using Miki.Discord.Common;
    using Miki.Framework.Commands.Pipelines;
    using System;
    using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'MiddlewareStage'
    public class MiddlewareStage : IPipelineStage
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'MiddlewareStage'
	{
		private readonly Func<IDiscordMessage, IMutableContext, Func<ValueTask>, ValueTask> fn;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'MiddlewareStage.MiddlewareStage(Func<IDiscordMessage, IMutableContext, Func<ValueTask>, ValueTask>)'
		public MiddlewareStage(Func<IDiscordMessage, IMutableContext, Func<ValueTask>, ValueTask> fn)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'MiddlewareStage.MiddlewareStage(Func<IDiscordMessage, IMutableContext, Func<ValueTask>, ValueTask>)'
		{
			this.fn = fn;
		}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'MiddlewareStage.CheckAsync(IDiscordMessage, IMutableContext, Func<ValueTask>)'
		public ValueTask CheckAsync(IDiscordMessage data, IMutableContext e, Func<ValueTask> next)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'MiddlewareStage.CheckAsync(IDiscordMessage, IMutableContext, Func<ValueTask>)'
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ContextExtensions'
    public static class ContextExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ContextExtensions'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ContextExtensions.UseStage(CommandPipelineBuilder, Func<IDiscordMessage, IMutableContext, Func<ValueTask>, ValueTask>)'
		public static CommandPipelineBuilder UseStage(this CommandPipelineBuilder b,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ContextExtensions.UseStage(CommandPipelineBuilder, Func<IDiscordMessage, IMutableContext, Func<ValueTask>, ValueTask>)'
			Func<IDiscordMessage, IMutableContext, Func<ValueTask>, ValueTask> fn)
		{
			return b.UseStage(new MiddlewareStage(fn));
		}
	}
}