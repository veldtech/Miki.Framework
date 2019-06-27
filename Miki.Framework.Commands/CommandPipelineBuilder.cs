using Microsoft.Extensions.DependencyInjection;
using Miki.Discord.Common;
using Miki.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miki.Framework.Commands.Pipelines
{
    public class CommandPipelineBuilder
    {
        private readonly List<IPipelineStage> _stages = new List<IPipelineStage>();
        private readonly MikiApp _app;
        private readonly ServiceCollection _serviceCollection;

        public CommandPipelineBuilder(MikiApp app)
        {
            _app = app;
            _serviceCollection = new ServiceCollection();
        }

        public CommandPipeline Build()
        {
            return new CommandPipeline(
                _app, _serviceCollection, _stages);
        }

        public CommandPipelineBuilder UseStage(IPipelineStage stage)
        {
            _serviceCollection.AddSingleton(stage.GetType(), stage);
            _stages.Add(stage);
            return this;
        }
    }

    public class CommandPipeline
    {
        const string PipelineArgumentKey = "core-pipeline";

        public IReadOnlyList<IPipelineStage> PipelineStages { get; }
        public Func<Exception, IContext, Task> OnError { get; set; }

        private readonly IServiceProvider _services;
        private readonly IServiceProvider _stageServices;

        internal CommandPipeline(MikiApp app, ServiceCollection stageServices, List<IPipelineStage> stages)
        {
            PipelineStages = stages;
            _services = app.Services;
            _stageServices = stageServices.BuildServiceProvider();
        }

        // TODO (Veld): Move IDiscordMessage to abstraction for a library-free solution.
        public async Task CheckAsync(IDiscordMessage data)
        {
            using (ContextObject c = new ContextObject(_services, _stageServices))
            {
                int index = 0;
                async Task nextFunc()
                {
                    if (index == PipelineStages.Count)
                    {
                        if (c.Executable != null)
                        {
                            await c.Executable.RunAsync(c);
                        }
                        return;
                    }

                    var stage = PipelineStages.ElementAtOrDefault(index);
                    index++;

                    if (stage == null)
                    {
                        return;
                    }
                    await stage.CheckAsync(data, c, nextFunc);
                }

                try
                {
                    await nextFunc();
                }
                catch(Exception e)
                {
                    if (OnError != null)
                    {
                        await OnError(e, c);
                    }
                }
            }
        }

        public IEnumerable<T> GetPipelineStagesOfType<T>()
            where T : class, IPipelineStage
        {
            return PipelineStages.Where(x => x is T)
                .Select(x => x as T);
        }
    }
}
