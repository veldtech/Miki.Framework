using Miki.Discord.Common;
using Miki.Framework.Commands;
using Miki.Framework.Commands.Pipelines;
using Miki.Framework.Commands.Stages;
using System;
using System.Threading.Tasks;

namespace Miki.Framework.Commands.Stages
{
	public class MiddlewareStage : IPipelineStage
	{
		private readonly Func<IDiscordMessage, IMutableContext, Func<ValueTask>, ValueTask> _fn;

		public MiddlewareStage(Func<IDiscordMessage, IMutableContext, Func<ValueTask>, ValueTask> fn)
		{
			_fn = fn;
		}

		public ValueTask CheckAsync(IDiscordMessage data, IMutableContext e, Func<ValueTask> next)
		{
			if(_fn != null)
			{
				return _fn(data, e, next);
			}
            return default;
		}
	}
}

namespace Miki.Framework
{
	public static class ContextExtensions
    {
		public static CommandPipelineBuilder UseStage(this CommandPipelineBuilder b,
			Func<IDiscordMessage, IMutableContext, Func<ValueTask>, ValueTask> fn)
		{
			return b.UseStage(new MiddlewareStage(fn));
		}
	}
}