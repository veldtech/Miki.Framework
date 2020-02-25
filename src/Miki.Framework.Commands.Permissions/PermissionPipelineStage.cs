﻿namespace Miki.Framework.Commands.Permissions
{
    using Miki.Discord.Common;
    using Miki.Framework.Commands.Nodes;
    using Miki.Framework.Commands.Permissions.Attributes;
    using Miki.Framework.Commands.Permissions.Models;
    using Miki.Framework.Commands.Pipelines;
    using Miki.Logging;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;

    public class PermissionPipelineStage : IPipelineStage
	{
        private readonly PermissionService service;

        public PermissionPipelineStage(PermissionService service)
        {
            this.service = service;
        }

        public async ValueTask CheckAsync(
            IDiscordMessage data,
            IMutableContext e,
            [NotNull] Func<ValueTask> next)
        {
            if (e?.Executable == null)
            {
                Log.Debug("No executable found to perform permission check on.");
                return;
            }

            var message = e.GetMessage();
            if(message.Author is IDiscordGuildUser)
            {
                var permission = await service.GetPriorityPermissionAsync(e);
                if(permission == null)
                {
                    var attribs = (e.Executable as NodeExecutable)?.Attributes;

                    permission = new Permission()
                    {
                        GuildId = (long)e.GetGuild().Id,
                        CommandName = e.Executable.ToString(),
                        EntityId = 0,
                        Status = attribs?.OfType<DefaultPermissionAttribute>()
                            .FirstOrDefault()?.Status ?? PermissionStatus.Allow,
                        Type = 0
                    };
                }
                Log.Debug(permission.ToString());

                if(permission.Status == PermissionStatus.Allow)
                {
                    await next();
                }
            }
            else
            {
                var attribs = (e.Executable as NodeExecutable)?.Attributes;
                if(attribs.OfType<DefaultPermissionAttribute>()
                    .Any(x => x.Status == PermissionStatus.Deny))
                {
                    throw new InvalidOperationException(
                        "Denied request due to default setting set to Deny");
                }

                await next();
            }
        }
    }
}

namespace Miki.Framework.Commands
{
    using Microsoft.Extensions.DependencyInjection;
    using Miki.Framework.Commands.Permissions;

    public static class PermissionExtensions
	{
		public static CommandPipelineBuilder UsePermissions(
			this CommandPipelineBuilder builder)
		{
            builder?.UseStage(
                new PermissionPipelineStage(
                    builder.Services.GetRequiredService<PermissionService>()));
			return builder;
		}
	}
}
