using System.Threading.Tasks;
using Miki.Framework.Commands.Permissions.Models;

namespace Miki.Framework.Commands.Permissions
{
    using Microsoft.EntityFrameworkCore;

    public class PermissionService
    {
        private DbContext context { get; }

        public PermissionService(DbContext context)
        {
            this.context = context;
        }

        public Task<Permission> GetPermissionAsync(
            long entityId,
            string commandName,
            long guildId)
        {
            return context.Set<Permission>()
                .SingleOrDefaultAsync(
                    x => x.EntityId == entityId
                         && x.GuildId == guildId
                         && x.CommandName == commandName);
        }
    }
}
