using Miki.Discord.Common;
using Miki.Framework.Commands.Pipelines;
using Miki.Framework.Commands.Stages;
using System;
using System.Threading.Tasks;

namespace Miki.Framework.Commands.Stages
{
	public class MiddlewareStage : IPipelineStage
	{
		private readonly Func<IDiscordMessage, IMutableContext, Func<Task>, Task> _fn;

		public MiddlewareStage(Func<IDiscordMessage, IMutableContext, Func<Task>, Task> fn)
		{
			_fn = fn;
		}

		public async Task CheckAsync(IDiscordMessage data, IMutableContext e, Func<Task> next)
		{
			if(_fn != null)
			{
				await _fn(data, e, next);
			}
		}
	}
}

namespace Miki.Framework.Commands
{
	public static class Extensions
	{
		public static CommandPipelineBuilder UseStage(this CommandPipelineBuilder b,
			Func<IDiscordMessage, IMutableContext, Func<Task>, Task> fn)
		{
			return b.UseStage(new MiddlewareStage(fn));
		}
	}
}