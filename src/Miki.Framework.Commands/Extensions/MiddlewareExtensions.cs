using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Internal;
using Miki.Discord.Common;
using Miki.Framework.Commands.Pipelines;
using Miki.Framework.Hosting;

namespace Miki.Framework.Commands
{
    public static class MiddlewareExtensions
    {
        private const string CoreStageRegistered = "CoreStageRegistered";

        private static IBotApplicationBuilder UseStageInternal(IBotApplicationBuilder app, IPipelineStage stage)
        {
            return app.Use(next =>
            {
                return context => stage.CheckAsync(
                    (IDiscordMessage) context.Message.InnerMessage,
                    (IMutableContext) context,
                    () => next(context));
            });
        }
        
        public static IBotApplicationBuilder UseStage(this IBotApplicationBuilder app, IPipelineStage stage)
        {
            if (!app.Properties.TryGetValue(CoreStageRegistered, out var value) || !Equals(value, true))
            {
                if (!(stage is CorePipelineStage))
                {
                    UseStageInternal(app, new CorePipelineStage());
                }
                
                app.Properties[CoreStageRegistered] = true;
            }
                
            return UseStageInternal(app, stage);
        }

        public static IBotApplicationBuilder UseStage<T>(this IBotApplicationBuilder app)
            where T : IPipelineStage
        {
            return UseStage(app, app.ApplicationServices.GetOrCreateService<T>());
        }

        public static IBotApplicationBuilder UseCommandPipeline(this IBotApplicationBuilder app, Action<CommandPipelineBuilder> configure)
        {
            var builder = new CommandPipelineBuilder(app.ApplicationServices);
            configure(builder);

            foreach (var stage in builder.Stages)
            {
                UseStage(app, stage);
            }
            
            return app;
        }
    }
}