using Miki.Discord.Common;
using System;
using System.Threading.Tasks;

namespace Miki.Framework.Commands.Pipelines
{
	public interface IPipelineStage
	{
		Task CheckAsync(IDiscordMessage data, IMutableContext e, Func<Task> next);
	}
}
