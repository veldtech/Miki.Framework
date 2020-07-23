using Miki.Discord.Common;
using Miki.Framework.Commands.Pipelines;
using Miki.Framework.Commands.Scopes.Attributes;
using Miki.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Miki.Framework.Commands.Scopes;

namespace Miki.Framework.Commands.Scopes
{
    public class ScopePipelineStage : IPipelineStage
	{
        private readonly IScopeService service;

        public ScopePipelineStage(IScopeService service)
        {
            this.service = service;
        }

        public async ValueTask CheckAsync(IDiscordMessage data, IMutableContext e, Func<ValueTask> next)
        {
            if(e.Executable == null)
            {
                Log.Warning("No command was selected, discontinue the flow.");
                return;
            }

            if(!(e.Executable is Node node))
            {
                Log.Warning("Executable was not made from a default Node.");
                return;
            }

            var scopesRequired = node.Attributes
                .OfType<RequiresScopeAttribute>()
                .Select(x => x.ScopeId)
                .ToList();

            var scopesGranted = await service.HasScopeAsync(
                    (long)e.GetMessage().Author.Id, scopesRequired)
                .ConfigureAwait(false);

            if(!scopesGranted)
            {
                Log.Warning("User tried to access scoped command, failed scope check.");
                return;
            }

            await next().ConfigureAwait(false);
        }
    }
}

namespace Miki.Framework.Commands
{
    public static class ScopeExtensions
	{
        /// <summary>
        /// Enable the feature to create feature-flag like scopes to allow specific users to specific
        /// commands.
        /// </summary>
        public static CommandPipelineBuilder UseScopes(this CommandPipelineBuilder builder)
		{
            return builder.UseStage(
                new ScopePipelineStage(builder.Services.GetService<IScopeService>()));
		}
	}
}