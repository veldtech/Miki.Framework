using Microsoft.EntityFrameworkCore;
using Miki.Discord.Common;
using Miki.Framework.Commands.Permissions;
using Miki.Framework.Commands.Permissions.Models;
using Miki.Framework.Commands.Pipelines;
using System;
using System.Threading.Tasks;
using Miki.Framework.Commands.Permissions.Extensions;

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
        internal const string UserLevelKey = "user-level";

        public async Task CheckAsync(
            IDiscordMessage data, IMutableContext e, Func<Task> next)
        {
            if (e.GetGuild() == null)
            {
                e.SetContext(UserLevelKey, PermissionLevel.DEFAULT);
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

                    e.SetContext(UserLevelKey, await db.GetUserPermissionLevelAsync(authorId, guildId));
                }
            }
            await next();
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
