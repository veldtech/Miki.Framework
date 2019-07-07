using Microsoft.EntityFrameworkCore;
using Miki.Discord.Common;
using Miki.Framework.Commands.Permissions;
using Miki.Framework.Commands.Permissions.Attributes;
using Miki.Framework.Commands.Permissions.Models;
using Miki.Framework.Commands.Pipelines;
using Miki.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Miki.Framework.Commands.Permissions
{
    public class PermissionPipelineStage : IPipelineStage
    {
        internal const string UserLevelKey = "user-level";

        public async Task CheckAsync(
            IDiscordMessage data,
            IMutableContext e,
            Func<Task> next)
        {
            if (!(e.GetMessage().Author is IDiscordGuildUser))
            {
                // TODO: handle DM permissions by giving the default.
            }

            var guildUser = e.GetMessage().Author as IDiscordGuildUser;
            if (await guildUser.HasPermissionsAsync(GuildPermission.Administrator))
            {
                await next();
            }
            else if(await GetAllowedForUser(e, data, e.Executable.ToString()))
            {
                await next();
            }
        }

        public async Task SetForUserAsync(
            IContext c,
            long entityId,
            EntityType type,
            string commandName,
            bool allow)
        {
            if (c.GetGuild() == null)
            {
                return;
            }

            var db = c.GetService<DbContext>();

            var entity = await db.Set<Permission>()
                .SingleOrDefaultAsync(x => x.EntityId == entityId && x.CommandName == commandName);

            if (entity == null)
            {
                db.Add(new Permission
                {
                    CommandName = commandName,
                    EntityId = entityId,
                    GuildId = (long)c.GetGuild().Id,
                    Status = allow ? PermissionStatus.Allow : PermissionStatus.Deny,
                    Type = type
                });
            }
            else
            {
                if (entity.Type != type)
                {
                    Log.Warning($"Set permission type for {{{entityId}, {commandName}}} does not match of (expected: {type}, actual: {entity.Type})");
                }

                entity.Status = allow ? PermissionStatus.Allow : PermissionStatus.Deny;
            }

            await db.SaveChangesAsync();
        }

        public async Task<bool> GetAllowedForUser(
            IContext c,
            IDiscordMessage request,
            string commandName)
        {
            var db = c.GetService<DbContext>();
            if(!(request.Author is IDiscordGuildUser guildUser))
            {
                return true;
            }

            var userPermission = await db.Set<Permission>()
                .SingleOrDefaultAsync(x => x.EntityId == (long)request.Author.Id
                    && x.GuildId == (long)guildUser.GuildId
                    && x.CommandName == commandName);
            if (userPermission != null
                && userPermission.Status != PermissionStatus.Default)
            {
                return userPermission.Status == PermissionStatus.Allow;
            }

            var channelPermission = await db.Set<Permission>()
                .SingleOrDefaultAsync(x => x.EntityId == (long)request.ChannelId
                    && x.GuildId == (long)guildUser.GuildId
                    && x.CommandName == commandName);
            if(channelPermission != null 
                && channelPermission.Status != PermissionStatus.Default)
            {
                return channelPermission.Status == PermissionStatus.Allow;
            }

            var rolePermission = await db.Set<Permission>()
                .Where(x => guildUser.RoleIds.Any(z => (long)z == x.EntityId)
                    && x.GuildId == (long)guildUser.GuildId
                    && x.CommandName == commandName
                    && x.Status != PermissionStatus.Default)
                .ToListAsync();
            if(rolePermission.Any(x => x.Status == PermissionStatus.Deny))
            {
                return false;
            }
            else if(rolePermission.Any(x => x.Status == PermissionStatus.Allow))
            {
                return true;
            }

            var guildPermission = await db.Set<Permission>()
                .SingleOrDefaultAsync(x => x.EntityId == (long)guildUser.GuildId
                    && x.GuildId == (long)guildUser.GuildId
                    && x.CommandName == commandName);
            if(guildPermission != null)
            {
                return guildPermission.Status == PermissionStatus.Allow;
            }

            return c.Executable.GetType()
                .GetCustomAttributes(typeof(DefaultPermissionAttribute), false)
                .OfType<DefaultPermissionAttribute>()
                .Select(x => x.Status == PermissionStatus.Allow)
                .FirstOrDefault();
        }
    }
}

namespace Miki.Framework.Commands
{
    public static class Extensions
    {
        public static CommandPipelineBuilder UsePermissions(
            this CommandPipelineBuilder builder)
        {
            builder.UseStage(new PermissionPipelineStage());
            return builder;
        }
    }
}
