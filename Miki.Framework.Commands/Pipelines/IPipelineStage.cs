namespace Miki.Framework.Commands.Pipelines
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Discord.Common;

    public interface IPipelineStage
	{
		ValueTask CheckAsync(IDiscordMessage data, IMutableContext e, [NotNull] Func<ValueTask> next);
	}
}
