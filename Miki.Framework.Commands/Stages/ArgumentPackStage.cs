namespace Miki.Framework.Commands.Pipelines
{
    using Miki.Discord.Common;
    using Miki.Framework.Arguments;
    using System;
    using System.Threading.Tasks;

    public class ArgumentPackBuilder : IPipelineStage
	{
		public const string ArgumentKey = "framework-arguments";

		private readonly ArgumentParseProvider provider;

		public ArgumentPackBuilder()
            : this(new ArgumentParseProvider())
		{ }
		public ArgumentPackBuilder(ArgumentParseProvider provider)
		{
			this.provider = provider;
		}

		public async ValueTask CheckAsync(IDiscordMessage message, IMutableContext e, Func<ValueTask> next)
		{
			e.SetContext<ITypedArgumentPack>(
				ArgumentKey,
				new TypedArgumentPack(
					new ArgumentPack(e.GetQuery()),
					provider));
			await next();
		}
	}
}

namespace Miki.Framework.Commands
{
    using Miki.Framework.Arguments;
    using Miki.Framework.Commands.Pipelines;

    public static class ArgumentPackCommandPipelineExtensions
	{
		public static CommandPipelineBuilder UseArgumentPack(this CommandPipelineBuilder builder)
		{
			return builder.UseStage(new ArgumentPackBuilder());
		}
		public static CommandPipelineBuilder UseArgumentPack(this CommandPipelineBuilder builder, ArgumentParseProvider provider)
		{
			return builder.UseStage(new ArgumentPackBuilder(provider));
		}
	}
}

namespace Miki.Framework
{
    using Miki.Framework.Arguments;
    using Miki.Framework.Commands.Pipelines;

    public static class ArgumentPackContextExtensions
	{
		public static ITypedArgumentPack GetArgumentPack(this IContext context)
		{
			return context.GetContext<ITypedArgumentPack>(ArgumentPackBuilder.ArgumentKey);
		}
	}
}