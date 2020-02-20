namespace Miki.Framework
{
    using Microsoft.Extensions.DependencyInjection;
    using Miki.Framework.Commands;
    using Miki.Framework.Commands.Stages;

    public static class ExecuteCommandExtensions
	{
		public static CommandPipelineBuilder UseCommandHandler(
            this CommandPipelineBuilder builder)
		{
			builder.UseStage(new ExecuteCommandStage(
                builder.Services.GetService<CommandTree>()));
			return builder;
		}
	}
}
