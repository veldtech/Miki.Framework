using Miki.Discord.Common;
using Miki.Framework.Arguments;
using Miki.Framework.Commands;
using Miki.Framework.Commands.Pipelines;
using Miki.Framework.Pipelines;
using System.Threading.Tasks;

namespace Miki.Framework.Commands.Pipelines
{
    public class ArgumentPackBuilder : IPipelineStage
    {
        public const string ArgumentKey = "framework-arguments";

        private readonly ArgumentParseProvider _provider;

        public ArgumentPackBuilder()
        {
            _provider = new ArgumentParseProvider();
        }
        public ArgumentPackBuilder(ArgumentParseProvider provider)
        {
            _provider = provider;
        }
        
        public ValueTask<bool> CheckAsync(IDiscordMessage message, IMutableContext e)
        {
            e.SetContext<ITypedArgumentPack>(
                ArgumentKey,
                new TypedArgumentPack(
                    new ArgumentPack(e.GetQuery()),
                    _provider));
            return new ValueTask<bool>(true);
        }
    }
}

namespace Miki.Framework.Commands
{
    public static class ArgumentPackCommandPipelineExtensions
    {
        public static CommandPipelineBuilder WithArgumentPack(this CommandPipelineBuilder builder)
        {
            return builder.WithStage(new ArgumentPackBuilder());
        }
        public static CommandPipelineBuilder WithArgumentPack(this CommandPipelineBuilder builder, ArgumentParseProvider provider)
        {
            return builder.WithStage(new ArgumentPackBuilder(provider));
        }
    }
}

namespace Miki.Framework
{
    public static class ArgumentPackContextExtensions
    {
        public static ITypedArgumentPack GetArgumentPack(this IContext context)
        {
            return context.GetContext<ITypedArgumentPack>(ArgumentPackBuilder.ArgumentKey);
        }
    }
}