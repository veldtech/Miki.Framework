namespace Miki.Framework.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Miki.Discord.Common;
    using Miki.Framework.Commands.Pipelines;
    using Miki.Logging;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'CommandPipeline'
    public class CommandPipeline : IAsyncEventingExecutor<IDiscordMessage>
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'CommandPipeline'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'CommandPipeline.PipelineStages'
        public IReadOnlyList<IPipelineStage> PipelineStages { get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'CommandPipeline.PipelineStages'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'CommandPipeline.OnExecuted'
        public Func<IExecutionResult<IDiscordMessage>, ValueTask> OnExecuted { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'CommandPipeline.OnExecuted'

        private readonly IServiceProvider services;

        internal CommandPipeline(
            IServiceProvider app,
            IReadOnlyList<IPipelineStage> stages)
        {
            PipelineStages = stages;
            services = app;
        }

        // TODO (velddev): Move IDiscordMessage to abstraction for a library-free solution.
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'CommandPipeline.ExecuteAsync(IDiscordMessage)'
        public async ValueTask ExecuteAsync(IDiscordMessage data)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'CommandPipeline.ExecuteAsync(IDiscordMessage)'
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
                    await OnExecuted(
                        new ExecutionResult<IDiscordMessage>(contextObj, data, exception));
                }
            }
        }
    }
}
