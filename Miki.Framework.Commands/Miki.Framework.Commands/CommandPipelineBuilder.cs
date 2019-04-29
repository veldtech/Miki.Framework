using Miki.Discord.Common;
using Miki.Framework.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Commands.Pipelines
{
    public class CommandPipelineBuilder
    {
        private List<IPipelineStage> _stages = new List<IPipelineStage>();

        private CommandMap _commandMap;

        private MikiAppBuilder _app;

        public CommandPipelineBuilder(MikiAppBuilder app)
        {
            _app = app;
        }

        public CommandPipeline Build(MikiApp app)
        {
            return new CommandPipeline(
                app, 
                _commandMap, 
                _stages);
        }

        public CommandPipelineBuilder WithStage(IPipelineStage stage)
        {
            _app.AddSingletonService(stage.GetType(), stage);
            _stages.Add(stage);
            return this;
        }

        public CommandPipelineBuilder WithCommandMap(CommandMap map)
        {
            _commandMap = map;
            return this;
        }
    }

    public class CommandPipeline
    {
        public IReadOnlyList<IPipelineStage> PipelineStages { get; }

        private CommandMap _map;
        private MikiApp _app;

        internal CommandPipeline(MikiApp app, CommandMap map, List<IPipelineStage> stages)
        {
            PipelineStages = stages;
            _app = app;
            _map = map;
        }

        // TODO (Veld): Move IDiscordMessage to abstraction for a library-free solution.
        public async Task CheckAsync(IDiscordMessage data)
        {
            using (ContextObject c = new ContextObject(_app.Services))
            {
                foreach (var stage in PipelineStages)
                {
                    if (!await stage.CheckAsync(data, c))
                    {
                        return;
                    }
                }
                await _map.Root.RunAsync(c);
            }
        }
    }
}
