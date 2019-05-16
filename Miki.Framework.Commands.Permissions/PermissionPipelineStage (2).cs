using Microsoft.EntityFrameworkCore;
using Miki.Discord.Common;
using Miki.Framework.Commands.Permissions;
using Miki.Framework.Commands.Permissions.Attributes;
using Miki.Framework.Commands.Permissions.Models;
using Miki.Framework.Commands.Pipelines;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Miki.Framework.Commands.Permissions
{
    public enum PermissionLevel
    {
        DEFAULT = 0,
        MODERATOR = 1,
        ADMIN = 2,
        OWNER = 3,
        STAFF = 4,
        DEVELOPER = 5,
    }


    public struct PermissionPipelineStageOptions
    {
        public PermissionLevel DefaultPermissionLevel;
    }

    public class PermissionPipelineStage : IPipelineStage
    {
        private readonly PermissionLevel _defaultPermissionLevel = 0;

        internal const string UserLevelKey = "user-level";

        public async Task CheckAsync(
            IDiscordMessage data, IMutableContext e, Func<Task> next)
        {
            if (e.GetGuild() == null)
            {
                e.SetContext(UserLevelKey, _defaultPermissionLevel);
            }
            else
            {
                if (e.GetGuild().OwnerId == e.GetMessage().Author.Id)
                {
                    e.SetContext(UserLevelKey, PermissionLevel.OWNER);
                }
                else
                {
                    var db = e.GetService<DbContext>();

                    long authorId = (long)e.GetMessage().Author.Id;
                    long guildId = (long)e.GetGuild().Id;

                    e.SetContext(
                        UserLevelKey,
                        await GetForUserAsync(db, authorId, guildId));
                }
            }
            await next();
        }

        public PermissionLevel GetForNode(Node node)
        {
            var attrib = node.Requirements
                .Where(x => x is RequiresPermissionAttribute)
                .Select(x => x as RequiresPermissionAttribute)
                .FirstOrDefault();
            if(attrib == null)
            {
                return PermissionLevel.DEFAULT;
            }
            return attrib.Level;
        }

        public async Task<PermissionLevel> GetForUserAsync(
            DbContext db, long userId, long guildId)
        {
            var p = await db.Set<Permission>()
                .FirstOrDefaultAsync(x
                    => x.UserId == userId
                    && (x.GuildId == guildId || x.GuildId == 0));
            return (PermissionLevel)(p?.PermissionLevel 
                ?? (int)_defaultPermissionLevel);
        }

        public async Task SetForUser(
            DbContext db, long userId, long guildId, PermissionLevel level)
        {
            var p = await db.Set<Permission>()
                .SingleOrDefaultAsync(x => x.UserId == userId
                    && (x.GuildId == guildId || x.GuildId == 0));
            if(p == null)
            {
                await db.Set<Permission>()
                    .AddAsync(new Permission
                    {
                        UserId = userId,
                        GuildId = guildId,
                        PermissionLevel = (int)level
                    });
            }
            else
            {
                p.PermissionLevel = (int)level;
            }
            await db.SaveChangesAsync();
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

        public static PermissionLevel GetUserPermissions(this IContext c)
            => c.GetContext<PermissionLevel>(
                PermissionPipelineStage.UserLevelKey);
    }
}
