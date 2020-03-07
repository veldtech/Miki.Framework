namespace Miki.Framework.Commands
{
    using System;
    using System.Collections.Generic;
    using Miki.Framework.Commands.Pipelines;

	/// <summary>
	/// Builds Command Pipelines. Helper pattern to keep your code clean.
	/// </summary>
    public class CommandPipelineBuilder
	{
		/// <summary>
		/// Services that can be used throughout the command pipeline.
		/// </summary>
        public IServiceProvider Services { get; }

        private readonly List<IPipelineStage> stages = new List<IPipelineStage>();

		/// <summary>
		/// Creates a new CommandPipelineBuilder. 
		/// </summary>
		public CommandPipelineBuilder(IServiceProvider services)
		{
            Services = services;
		}

		/// <summary>
		/// Builds the pipeline stage and returns an immutable <see cref="CommandPipeline"/>.
		/// </summary>
		/// <returns></returns>
        public CommandPipeline Build()
		{
			return new CommandPipeline(Services, stages);
		}

		/// <summary>
		/// Initializes a pipeline stage as a runnable stage in the pipeline.
		/// </summary>
		/// <param name="stage">The pipeline stage you'd like to add to the stagelist.</param>
        public CommandPipelineBuilder UseStage(IPipelineStage stage)
		{
			stages.Add(stage);
			return this;
		}
	}
}
