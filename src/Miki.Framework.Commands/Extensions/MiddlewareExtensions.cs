using System;
using System.Collections.Generic;
using Miki.Framework.Commands.Pipelines;
using Miki.Framework.Hosting;

namespace Miki.Framework.Commands
{
    public static class MiddlewareExtensions
    {
        public static IBotApplicationBuilder UseStage(this IBotApplicationBuilder app, IPipelineStage stage)
        {
            return app.Use(next =>
            {
                return context => stage.CheckAsync(context.Message, (IMutableContext) context, () => next(context));
            });
        }

        public static IBotApplicationBuilder UseStage<T>(this IBotApplicationBuilder app)
            where T : IPipelineStage
        {
            return UseStage(app, app.ApplicationServices.GetOrCreateService<T>());
        }

        public static IBotApplicationBuilder UsePipeline(this IBotApplicationBuilder app, Action<CommandPipelineBuilder> configure)
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