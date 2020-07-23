using Miki.Discord.Common;
using Miki.Framework.Arguments;
using System;
using System.Linq;
using System.Threading.Tasks;
using Miki.Framework.Commands.Pipelines;

namespace Miki.Framework.Commands.Pipelines
{
    /// <summary>
    /// Allows you to use FIFO-like argument readers. Used in other packages as a dependency.
    /// </summary>
	public class ArgumentPackBuilder : IPipelineStage
	{
		internal static string ArgumentKey = "framework-arguments";

		private readonly ArgumentParseProvider provider;

        /// <summary>
        /// Allows you to use FIFO-like argument readers. Used in other packages as a dependency.
        /// </summary>
		public ArgumentPackBuilder()
            : this(new ArgumentParseProvider())
		{ }

        /// <summary>
        /// Allows you to use FIFO-like argument readers. Used in other packages as a dependency. With 
        /// non-default providers if you prefer overriding default implementation.
        /// </summary>
		public ArgumentPackBuilder(ArgumentParseProvider provider)
		{
			this.provider = provider;
		}

		/// <inheritdoc/>
		public async ValueTask CheckAsync(
            IDiscordMessage message, IMutableContext e, Func<ValueTask> next)
		{
			e.SetContext<ITypedArgumentPack>(
				ArgumentKey,
				new TypedArgumentPack(
					new ArgumentPack(
                        e.GetQuery().Split(' ').Where(x => !string.IsNullOrWhiteSpace(x))), provider));
			await next();
		}
	}
}

namespace Miki.Framework.Commands
{
	/// <summary>
	/// Helper class for builder extensions
	/// </summary>
    public static class ArgumentPackCommandPipelineExtensions
	{
		/// <summary>
		/// Allows you to use FIFO-like argument readers. Used in other packages as a dependency.
		/// </summary>
		public static CommandPipelineBuilder UseArgumentPack(this CommandPipelineBuilder builder)
		{
			return builder.UseStage(new ArgumentPackBuilder());
		}
        /// <summary>
        /// Allows you to use FIFO-like argument readers. Used in other packages as a dependency. With 
        /// non-default providers if you prefer overriding default implementation.
        /// </summary>
		public static CommandPipelineBuilder UseArgumentPack(
           this CommandPipelineBuilder builder, ArgumentParseProvider provider)
		{
			return builder.UseStage(new ArgumentPackBuilder(provider));
		}
	}
}

namespace Miki.Framework
{
    /// <summary>
    /// Helper class for context extensions
    /// </summary>
	public static class ArgumentPackContextExtensions
	{
		/// <summary>
		/// Gets this context's arguments.
		/// </summary>
		public static ITypedArgumentPack GetArgumentPack(this IContext context)
		{
			return context.GetContext<ITypedArgumentPack>(ArgumentPackBuilder.ArgumentKey);
		}
	}
}