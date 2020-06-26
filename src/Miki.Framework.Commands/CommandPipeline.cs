﻿namespace Miki.Framework.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Miki.Discord.Common;
    using Miki.Framework.Models;
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
            using ContextObject contextObj = new ContextObject(services, new DiscordMessage(data));
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
                await NextFunc();
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
                        new ExecutionResult<IDiscordMessage>(
                            contextObj, data, sw.ElapsedMilliseconds, exception));
                }
            }                
            Log.Message($"request {contextObj.Executable} took {sw.ElapsedMilliseconds}ms.");
        }
    }
}
