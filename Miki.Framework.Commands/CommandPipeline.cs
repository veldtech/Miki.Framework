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

        private readonly IServiceProvider services;
        [Obsolete("Use direct services instead.")]
        private readonly IServiceProvider stageServices;

        internal CommandPipeline(
            IServiceProvider app,
            IServiceCollection stageServices,
            IReadOnlyList<IPipelineStage> stages)
        {
            PipelineStages = stages;
            services = app;
            this.stageServices = stageServices.BuildServiceProvider();
        }

        // TODO (velddev): Move IDiscordMessage to abstraction for a library-free solution.
        public async ValueTask ExecuteAsync(IDiscordMessage data)
        {
            var sw = Stopwatch.StartNew();
            using ContextObject contextObj = new ContextObject(services, stageServices);
            int index = 0;

            Func<ValueTask> nextFunc = null;
            ValueTask NextFunc()
            {
                if (contextObj == null)
                {
                    throw new InvalidOperationException("You're not allowed to nullify data");
                }

                if (index == PipelineStages.Count)
                {
                    return contextObj.Executable?.ExecuteAsync(contextObj) ?? default;
                }
                var stage = PipelineStages[index];
                index++;
                return stage?.CheckAsync(data, contextObj, nextFunc) ?? default;
            }
            nextFunc = NextFunc;

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
