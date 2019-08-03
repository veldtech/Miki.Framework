﻿using Miki.Discord.Common;
using Miki.Framework.Commands.Nodes;
using Miki.Framework.Commands.Pipelines;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Miki.Framework.Commands.Scopes.Attributes;
using Microsoft.EntityFrameworkCore;
using Miki.Framework.Commands.Scopes.Models;

namespace Miki.Framework.Commands.Scopes
{
    public class ScopePipelineStage : IPipelineStage
    {
        public async Task CheckAsync(IDiscordMessage data, IMutableContext e, Func<Task> next)
        {
            if(e.Executable == null)
            {
                return;
            }

            if(e is NodeExecutable node)
            {
                var db = e.GetService<DbContext>();

                var scopesRequired = node.Attributes
                    .OfType<RequiresScopeAttribute>()
                    .Select(x => x.ScopeId);

                var scopesGranted = await db.Set<Scope>()
                    .Where(x => x.UserId == (long)e.GetMessage().Author.Id
                        && scopesRequired.Contains(x.ScopeId))
                    .Select(x => x.ScopeId)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if(scopesRequired.All(x => scopesGranted.Contains(x)))
                {
                    await next().ConfigureAwait(false);
                }
            }
        }
    }
}

namespace Miki.Framework.Commands
{
    public static class ScopeExtensions
    {

    }
}