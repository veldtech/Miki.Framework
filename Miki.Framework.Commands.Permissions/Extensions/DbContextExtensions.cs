using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Miki.Framework.Commands.Permissions.Models;

namespace Miki.Framework.Commands.Permissions.Extensions
{
    public static class DbContextExtensions
    {
        public static async Task<PermissionLevel> GetUserPermissionLevelAsync(this DbContext db, long userId, long guildId, PermissionLevel defaultPermissionLevel = PermissionLevel.DEFAULT)
        {
            var p = await db.Set<Permission>().FirstOrDefaultAsync(x => x.UserId == userId && (x.GuildId == guildId || x.GuildId == 0));

            if (p == null)
            {
                return defaultPermissionLevel;
            }

            return (PermissionLevel) p.PermissionLevel;
        }
    }
}
