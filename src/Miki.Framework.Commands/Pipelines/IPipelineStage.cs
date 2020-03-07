namespace Miki.Framework.Commands.Pipelines
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Discord.Common;

    /// <summary>
    /// Chainable pipeline stage which mutates and moves forward to the next stage.
    /// </summary>
    public interface IPipelineStage
	{
        /// <summary>
        /// Executes and handles edge cases.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="e"></param>
        /// <param name="next"></param>
        /// <returns></returns>
		ValueTask CheckAsync(IDiscordMessage data, IMutableContext e, [NotNull] Func<ValueTask> next);
	}
}
