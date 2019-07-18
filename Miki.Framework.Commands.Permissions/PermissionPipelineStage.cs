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
            var db = e.GetService<DbContext>();
            if (!(e.GetMessage().Author is IDiscordGuildUser))
            {
                if((e.Executable as Node)
                    .Attributes.OfType<DefaultPermissionAttribute>()
                    .Any(x => x.Status == PermissionStatus.Deny))
                {
                    return;
                }
            }

            if(await GetAllowedForUser(
                db, 
                e.GetMessage().Author, 
                e.GetChannel(), 
                e.Executable))
            {
                await next();
            }
        }

        public async Task SetForUserAsync(
            IContext c,
            long entityId,
            EntityType type,
            string commandName,
            PermissionStatus allow)
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
                    Status = allow,
                    Type = type
                });
            }
            else
            {
                if (entity.Type != type)
                {
                    Log.Warning($"Set permission type for {{{entityId}, {commandName}}} does not match of (expected: {type}, actual: {entity.Type})");
                }

                entity.Status = allow;
            }

            await db.SaveChangesAsync();
        }

        public async Task<bool> GetAllowedForUser(
            DbContext db,
            IDiscordUser user,
            IDiscordChannel channel,
            IExecutable command)
        {
            string commandName = command.ToString();
            if ((user is IDiscordGuildUser guildUser))
            {
                if (await guildUser.HasPermissionsAsync(GuildPermission.Administrator))
                {
                    return true;
                }

                var userPermission = await db.Set<Permission>()
                    .SingleOrDefaultAsync(x => x.EntityId == user.Id.ToDbLong()
                        && x.GuildId == guildUser.GuildId.ToDbLong()
                        && x.CommandName == commandName);
                if (userPermission != null
                    && userPermission.Status != PermissionStatus.Default)
                {
                    return userPermission.Status == PermissionStatus.Allow;
                }

                var channelPermission = await db.Set<Permission>()
                    .SingleOrDefaultAsync(x => x.EntityId == channel.Id.ToDbLong()
                        && x.GuildId == guildUser.GuildId.ToDbLong()
                        && x.CommandName == commandName);
                if (channelPermission != null
                    && channelPermission.Status != PermissionStatus.Default)
                {
                    return channelPermission.Status == PermissionStatus.Allow;
                }

                var rolePermission = await db.Set<Permission>()
                    .Where(x => guildUser.RoleIds.Any(z => z.ToDbLong() == x.EntityId)
                        && x.GuildId == guildUser.GuildId.ToDbLong()
                        && x.CommandName == commandName
                        && x.Status != PermissionStatus.Default)
                    .ToListAsync();
                if (rolePermission.Any(x => x.Status == PermissionStatus.Deny))
                {
                    return false;
                }
                if (rolePermission.Any(x => x.Status == PermissionStatus.Allow))
                {
                    return true;
                }

                var guildPermission = await db.Set<Permission>()
                    .SingleOrDefaultAsync(x => x.EntityId == guildUser.GuildId.ToDbLong()
                        && x.GuildId == guildUser.GuildId.ToDbLong()
                        && x.CommandName == commandName);
                if (guildPermission != null)
                {
                    return guildPermission.Status == PermissionStatus.Allow;
                }
            }

            var defaultDenied = (command as Node).Attributes
                .OfType<DefaultPermissionAttribute>()
                .Any(x => x.Status == PermissionStatus.Deny);
            return !defaultDenied;
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
