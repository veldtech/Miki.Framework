namespace Miki.Framework.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Miki.Discord.Common;
    using Miki.Framework.Commands.Pipelines;

    public class CommandPipeline : IAsyncEventingExecutor<IDiscordMessage>
    {
        public IReadOnlyList<IPipelineStage> PipelineStages { get; }

        public Func<IExecutionResult<IDiscordMessage>, ValueTask> OnExecuted { get; set; }


        private readonly IServiceProvider _services;
        private readonly IServiceProvider _stageServices;

        internal CommandPipeline(
            IServiceProvider app,
            IServiceCollection stageServices,
            IReadOnlyList<IPipelineStage> stages)
        {
            PipelineStages = stages;
            _services = app;
            _stageServices = stageServices.BuildServiceProvider();
        }

        // TODO (Veld): Move IDiscordMessage to abstraction for a library-free solution.
        public async Task ExecuteAsync(IDiscordMessage data)
        {
            using ContextObject contextObj = new ContextObject(_services, _stageServices);
            int index = 0;

            async Task NextFunc()
            {
                if (index == PipelineStages.Count)
                {
                    if (contextObj.Executable != null)
                    {
                        await contextObj.Executable.ExecuteAsync(contextObj);
                    }

                    return;
                }

                var stage = PipelineStages.ElementAtOrDefault(index);
                index++;

                if (stage == null)
                {
                    return;
                }

                await stage.CheckAsync(data, contextObj, NextFunc);
            }

            try
            {
                await NextFunc();
            }
            catch (Exception e)
            {
                if (this.OnExecuted != null)
                {
                    await this.OnExecuted(
                        new ExecutionResult<IDiscordMessage>(contextObj, data, e));
                }
            }
        }
    }
}
