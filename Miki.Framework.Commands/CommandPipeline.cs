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

        internal CommandPipeline(
            IServiceProvider app,
            IReadOnlyList<IPipelineStage> stages)
        {
            PipelineStages = stages;
            services = app;
        }

        // TODO (velddev): Move IDiscordMessage to abstraction for a library-free solution.
        public async ValueTask ExecuteAsync(IDiscordMessage data)
        {
            var sw = Stopwatch.StartNew();
            using ContextObject contextObj = new ContextObject(services);
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

            Exception exception = null;
            try
            {
                var totalTime = Stopwatch.StartNew();
                sw.Start();
                await NextFunc();
                Log.Message($"request {contextObj.Executable} took {totalTime.Elapsed.TotalMilliseconds}ms.");
            }
            catch (Exception e)
            {
                exception = e;
            }
            finally
            {
                if(this.OnExecuted != null)
                {
                    await this.OnExecuted(
                        new ExecutionResult<IDiscordMessage>(contextObj, data, exception));
                }
            }
        }
    }
}
