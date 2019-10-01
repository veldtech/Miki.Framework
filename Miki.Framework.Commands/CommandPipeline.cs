namespace Miki.Framework.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Miki.Discord.Common;
    using Miki.Framework.Commands.Pipelines;
    using Miki.Logging;

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

        public ValueTask BenchmarkTask(Type t, Func<ValueTask> innerTask, Stopwatch sw = null)
        {
            if(sw == null)
            {
                sw = Stopwatch.StartNew();
            }
            Log.Debug($"Task '{t.Name}' took {sw.Elapsed.TotalMilliseconds}ms.");
            sw.Restart();
            return innerTask();    
        }

        // TODO (Veld): Move IDiscordMessage to abstraction for a library-free solution.
        public async ValueTask ExecuteAsync(IDiscordMessage data)
        {
            var sw = Stopwatch.StartNew();
            using ContextObject contextObj = new ContextObject(_services, _stageServices);
            int index = 0;

            Func<ValueTask> nextFunc = null;
            ValueTask NextFunc()
            {
                if (index == PipelineStages.Count)
                {
                    if (contextObj.Executable != null)
                    {
                        return contextObj.Executable.ExecuteAsync(contextObj);
                    }
                    return default;
                }
                var stage = PipelineStages[index];
                index++;
                if (stage == null)
                {
                    return default;
                }
                return stage.CheckAsync(data, contextObj, nextFunc);
            }
            nextFunc = () => NextFunc();
#if DEBUG
            nextFunc = () => BenchmarkTask(
                PipelineStages.ElementAtOrDefault(index - 1)?.GetType() ?? contextObj.Executable.GetType(),
                () => NextFunc(), 
                sw);
#endif

            try
            {
                var totalTime = Stopwatch.StartNew();
                sw.Start();
                await NextFunc();
                Log.Message($"request {data.ChannelId} - {data.Author.Username} took {totalTime.Elapsed.TotalMilliseconds}ms.");
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
