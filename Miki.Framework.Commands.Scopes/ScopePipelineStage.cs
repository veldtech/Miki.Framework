
namespace Miki.Framework.Commands.Scopes
{
    using Microsoft.EntityFrameworkCore;
    using Miki.Discord.Common;
    using Miki.Framework.Commands.Pipelines;
    using Miki.Framework.Commands.Scopes.Attributes;
    using Miki.Framework.Commands.Scopes.Models;
    using Miki.Logging;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class ScopePipelineStage : IPipelineStage
	{
        public async Task AddScopeAsync(DbContext context, IDiscordUser user, string scope)
        {
            var scopeObject = await context.Set<Scope>()
                .FindAsync(scope, user.Id.ToDbLong());
            if(scopeObject == null)
            {
                context.Set<Scope>()
                    .Add(new Scope
                    {
                        ScopeId = scope.ToLowerInvariant(),
                        UserId = (long)user.Id
                    });
                await context.SaveChangesAsync();
            }
        }

        public async ValueTask CheckAsync(IDiscordMessage data, IMutableContext e, Func<ValueTask> next)
		{
            if (e.Executable is Node node)
            {
                var db = e.GetService<DbContext>();

                var scopesRequired = node.Attributes
                    .OfType<RequiresScopeAttribute>()
                    .Select(x => x.ScopeId)
                    .ToList();

                if (scopesRequired.Any())
                {
                    var scopesGranted = await db.Set<Scope>()
                        .Where(x => x.UserId == e.GetMessage().Author.Id.ToDbLong()
                                    && scopesRequired.Contains(x.ScopeId))
                        .Select(x => x.ScopeId)
                        .ToListAsync()
                        .ConfigureAwait(false);

                    if (!scopesRequired.All(x => scopesGranted.Contains(x)))
                    {
                        Log.Debug($"User '{e.GetMessage().Author}' tried to access {node}, but was not allowed to.");
                        return;
                    }
                }
            }

            await next().ConfigureAwait(false);
        }
    }
}

namespace Miki.Framework.Commands
{
    using Miki.Framework.Commands.Scopes;   

    public static class ScopeExtensions
	{
		public static CommandPipelineBuilder UseScopes(this CommandPipelineBuilder builder)
		{
			builder.UseStage(new ScopePipelineStage());
			return builder;
		}
	}
}