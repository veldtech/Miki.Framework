namespace Miki.Framework.Commands
{
    using Microsoft.Extensions.DependencyInjection;
    using Miki.Discord.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Miki.Framework.Commands.Pipelines;

    public class CommandPipelineBuilder
	{
        public IServiceProvider Services { get; }

        private readonly List<IPipelineStage> stages = new List<IPipelineStage>();

		public CommandPipelineBuilder(IServiceProvider services)
		{
            Services = services;
		}

		public CommandPipeline Build()
		{
			return new CommandPipeline(Services, stages);
		}

		public CommandPipelineBuilder UseStage(IPipelineStage stage)
		{
			stages.Add(stage);
			return this;
		}
	}
}
