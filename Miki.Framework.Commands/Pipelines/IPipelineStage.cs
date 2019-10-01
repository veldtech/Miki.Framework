using Miki.Discord.Common;
using System;
using System.Threading.Tasks;

namespace Miki.Framework.Commands.Pipelines
{
	public interface IPipelineStage
	{
		ValueTask CheckAsync(IDiscordMessage data, IMutableContext e, Func<ValueTask> next);
	}
}
