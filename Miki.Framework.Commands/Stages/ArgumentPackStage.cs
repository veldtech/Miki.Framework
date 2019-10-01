using Miki.Discord.Common;
using Miki.Framework.Arguments;
using Miki.Framework.Commands.Pipelines;
using System;
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

		public async ValueTask CheckAsync(IDiscordMessage message, IMutableContext e, Func<ValueTask> next)
		{
			e.SetContext<ITypedArgumentPack>(
				ArgumentKey,
				new TypedArgumentPack(
					new ArgumentPack(e.GetQuery()),
					_provider));
			await next();
		}
	}
}

namespace Miki.Framework.Commands
{
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
	public static class ArgumentPackContextExtensions
	{
		public static ITypedArgumentPack GetArgumentPack(this IContext context)
		{
			return context.GetContext<ITypedArgumentPack>(ArgumentPackBuilder.ArgumentKey);
		}
	}
}