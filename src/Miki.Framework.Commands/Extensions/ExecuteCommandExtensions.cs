using Microsoft.Extensions.DependencyInjection;
using Miki.Framework.Commands;
using Miki.Framework.Commands.Stages;

namespace Miki.Framework
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ExecuteCommandExtensions'
    public static class ExecuteCommandExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ExecuteCommandExtensions'
	{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ExecuteCommandExtensions.UseCommandHandler(CommandPipelineBuilder)'
		public static CommandPipelineBuilder UseCommandHandler(
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ExecuteCommandExtensions.UseCommandHandler(CommandPipelineBuilder)'
            this CommandPipelineBuilder builder)
		{
			builder.UseStage(new ExecuteCommandStage(
                builder.Services.GetService<CommandTree>()));
			return builder;
		}
	}
}
