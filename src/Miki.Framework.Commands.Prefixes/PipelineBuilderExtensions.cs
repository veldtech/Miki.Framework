namespace Miki.Framework.Commands
{
    using Miki.Framework.Commands.Prefixes;
    using Microsoft.Extensions.DependencyInjection;

    public static class PipelineBuilderExtensions
    {
        public const string PrefixMatchKey = "prefix-match";

        public static CommandPipelineBuilder UsePrefixes(
            this CommandPipelineBuilder builder)
        {
            return builder.UseStage(
                new PipelineStageTrigger(
                    builder.Services.GetService<IPrefixService>()));
        }

        public static string GetPrefixMatch(this IContext e)
        {
            return e.GetContext<string>(PrefixMatchKey);
        }
    }
}
