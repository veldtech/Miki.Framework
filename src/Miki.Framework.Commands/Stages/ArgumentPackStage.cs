namespace Miki.Framework.Commands.Pipelines
{
    using Miki.Discord.Common;
    using Miki.Framework.Arguments;
    using System;
    using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ArgumentPackBuilder'
    public class ArgumentPackBuilder : IPipelineStage
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ArgumentPackBuilder'
	{
		internal static string ArgumentKey = "framework-arguments";

		private readonly ArgumentParseProvider provider;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ArgumentPackBuilder.ArgumentPackBuilder()'
		public ArgumentPackBuilder()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ArgumentPackBuilder.ArgumentPackBuilder()'
            : this(new ArgumentParseProvider())
		{ }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ArgumentPackBuilder.ArgumentPackBuilder(ArgumentParseProvider)'
		public ArgumentPackBuilder(ArgumentParseProvider provider)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ArgumentPackBuilder.ArgumentPackBuilder(ArgumentParseProvider)'
		{
			this.provider = provider;
		}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ArgumentPackBuilder.CheckAsync(IDiscordMessage, IMutableContext, Func<ValueTask>)'
		public async ValueTask CheckAsync(
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ArgumentPackBuilder.CheckAsync(IDiscordMessage, IMutableContext, Func<ValueTask>)'
            IDiscordMessage message, IMutableContext e, Func<ValueTask> next)
		{
			e.SetContext<ITypedArgumentPack>(
				ArgumentKey,
				new TypedArgumentPack(
					new ArgumentPack(e.GetQuery().Split(' ')),
					provider));
			await next();
		}
	}
}

namespace Miki.Framework.Commands
{
    using Miki.Framework.Arguments;
    using Miki.Framework.Commands.Pipelines;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ArgumentPackCommandPipelineExtensions'
    public static class ArgumentPackCommandPipelineExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ArgumentPackCommandPipelineExtensions'
	{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ArgumentPackCommandPipelineExtensions.UseArgumentPack(CommandPipelineBuilder)'
		public static CommandPipelineBuilder UseArgumentPack(this CommandPipelineBuilder builder)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ArgumentPackCommandPipelineExtensions.UseArgumentPack(CommandPipelineBuilder)'
		{
			return builder.UseStage(new ArgumentPackBuilder());
		}
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ArgumentPackCommandPipelineExtensions.UseArgumentPack(CommandPipelineBuilder, ArgumentParseProvider)'
		public static CommandPipelineBuilder UseArgumentPack(
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ArgumentPackCommandPipelineExtensions.UseArgumentPack(CommandPipelineBuilder, ArgumentParseProvider)'
            this CommandPipelineBuilder builder, ArgumentParseProvider provider)
		{
			return builder.UseStage(new ArgumentPackBuilder(provider));
		}
	}
}

namespace Miki.Framework
{
    using Miki.Framework.Arguments;

    using Miki.Framework.Commands.Pipelines;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ArgumentPackContextExtensions'
    public static class ArgumentPackContextExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ArgumentPackContextExtensions'
	{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ArgumentPackContextExtensions.GetArgumentPack(IContext)'
		public static ITypedArgumentPack GetArgumentPack(this IContext context)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ArgumentPackContextExtensions.GetArgumentPack(IContext)'
		{
			return context.GetContext<ITypedArgumentPack>(ArgumentPackBuilder.ArgumentKey);
		}
	}
}