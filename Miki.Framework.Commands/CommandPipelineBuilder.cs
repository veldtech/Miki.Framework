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

        private readonly List<IPipelineStage> _stages = new List<IPipelineStage>();
		private readonly ServiceCollection _serviceCollection;

		public CommandPipelineBuilder(IServiceProvider services)
		{
            Services = services;
			_serviceCollection = new ServiceCollection();
		}

		public CommandPipeline Build()
		{
			return new CommandPipeline(
                Services, _serviceCollection, _stages);
		}

		public CommandPipelineBuilder UseStage(IPipelineStage stage)
		{
			_serviceCollection.AddSingleton(stage.GetType(), stage);
			_stages.Add(stage);
			return this;
		}
	}
}
