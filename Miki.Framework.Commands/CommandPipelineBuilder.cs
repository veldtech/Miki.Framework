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
		private readonly List<IPipelineStage> _stages = new List<IPipelineStage>();
		private readonly IServiceProvider _services;
		private readonly ServiceCollection _serviceCollection;

		public CommandPipelineBuilder(IServiceProvider services)
		{
            _services = services;
			_serviceCollection = new ServiceCollection();
		}

		public CommandPipeline Build()
		{
			return new CommandPipeline(
                _services, _serviceCollection, _stages);
		}

		public CommandPipelineBuilder UseStage(IPipelineStage stage)
		{
			_serviceCollection.AddSingleton(stage.GetType(), stage);
			_stages.Add(stage);
			return this;
		}
	}

	public class CommandPipeline : IAsyncExecutor<IDiscordMessage>
	{
		public IReadOnlyList<IPipelineStage> PipelineStages { get; }

        public Func<IContext, Task> CommandProcessed { get; set; }
        public Func<Exception, IContext, Task> CommandError { get; set; }


		private readonly IServiceProvider _services;
		private readonly IServiceProvider _stageServices;

		internal CommandPipeline(IServiceProvider app, IServiceCollection stageServices, IReadOnlyList<IPipelineStage> stages)
		{
			PipelineStages = stages;
			_services = app;
			_stageServices = stageServices.BuildServiceProvider();
		}

		// TODO (Veld): Move IDiscordMessage to abstraction for a library-free solution.
		public async Task ExecuteAsync(IDiscordMessage data)
        {
            using ContextObject c = new ContextObject(_services, _stageServices);
            int index = 0;
            async Task NextFunc()
            {
                if(index == PipelineStages.Count)
                {
                    if(c.Executable != null)
                    {
                        await c.Executable.ExecuteAsync(c);
                    }
                    return;
                }

                var stage = PipelineStages.ElementAtOrDefault(index);
                index++;

                if(stage == null)
                {
                    return;
                }
                await stage.CheckAsync(data, c, NextFunc);
            }

            try
            {
                await NextFunc();
            }
            catch(Exception e)
            {
                if(this.CommandError != null)
                {
                    await this.CommandError(e, c);
                }
            }
        }
    }
}
