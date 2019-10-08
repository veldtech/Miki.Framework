using System.Linq.Expressions;

namespace Miki.Framework.Commands.Permissions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Miki.Discord.Common;
    using Miki.Framework.Commands.Permissions.Models;
    using Miki.Patterns.Repositories;

    public class PermissionService
    {
        private readonly IUnitOfWork unit;
        private readonly IAsyncRepository<Permission> repository;

        public PermissionService(IUnitOfWork unit)
        {
            this.unit = unit ?? throw new ArgumentNullException(nameof(unit));
            this.repository = unit.GetRepository<Permission>();
        }

        public ValueTask DeleteAsync(Permission permission)
        {
            return repository.DeleteAsync(permission);
        }

        public async ValueTask<bool> ExistsAsync(Permission permission)
        {
            var enumerable = await repository.ListAsync();
            if (enumerable is IQueryable<Permission> queryable)
            {
                return await queryable.AnyAsync(
                    x => x.EntityId == permission.EntityId
                         && x.CommandName == permission.CommandName
                         && x.GuildId == permission.GuildId)
                    .ConfigureAwait(false);
            }
            return enumerable.Any(
                x => x.EntityId == permission.EntityId
                     && x.CommandName == permission.CommandName
                     && x.GuildId == permission.GuildId);
        }

        public ValueTask<Permission> GetPermissionAsync(
            long entityId, string commandName, long guildId)
        {
            return repository.GetAsync(entityId, commandName, guildId);
        }
        public async ValueTask<Permission> GetPriorityPermissionAsync(
            long guildId, string commandName, long[] entityIds)
        {
            var enumerable = await repository.ListAsync();
            enumerable = enumerable.Where(x => x.CommandName == commandName && x.GuildId == guildId)
                .Where(x => entityIds.Contains(x.EntityId))
                .OrderBy(x => x.Type);
            if(enumerable is IQueryable<Permission> queryable)
            {
                return await queryable.FirstOrDefaultAsync(x => x.Status != PermissionStatus.Default)
                    .ConfigureAwait(false);
            }
            return enumerable.FirstOrDefault(x => x.Status != PermissionStatus.Default);
        }
        public async ValueTask<Permission> GetPriorityPermissionAsync(
            IContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if(context.GetMessage().Author is IDiscordGuildUser gu)
            {
                if(await gu.HasPermissionsAsync(GuildPermission.Administrator)
                    .ConfigureAwait(false))
                {
                    return new Permission
                    {
                        EntityId = 0,
                        CommandName = context.Executable.ToString(),
                        GuildId = (long)context.GetGuild().Id,
                        Status = PermissionStatus.Allow,
                        Type = EntityType.User
                    };
                }
            }

            List<long> idList = new List<long>();
            if(context.GetMessage() != null)
            {
                idList.Add((long)context.GetMessage().Author.Id);
                idList.Add((long)context.GetMessage().ChannelId);
            }
            if(context.GetGuild() != null)
            {
                idList.Add((long)context.GetGuild().Id);
            }
            if(context.GetMessage().Author is IDiscordGuildUser user)
            {
                idList.AddRange(user.RoleIds.Select(x => (long)x));
            }
            return await GetPriorityPermissionAsync(
                (long)context.GetGuild().Id, context.Executable.ToString(), idList.ToArray());
        }

        public async ValueTask<List<Permission>> ListPermissionsAsync(long guildId)
        {
            var enumerable = await repository.ListAsync();
            enumerable = enumerable.Where(x => x.GuildId == guildId);
            if(enumerable is IQueryable<Permission> queryable)
            {
                return await queryable.ToListAsync()
                    .ConfigureAwait(false);
            }
            return enumerable.ToList();
        }
        public async ValueTask<List<Permission>> ListPermissionsAsync(
            long guildId, string commandName, params long[] entityFilter)
        {
            var enumerable = await repository.ListAsync();
            if(enumerable is IQueryable<Permission> queryable)
            {
                return await queryable.Where(x => x.GuildId == guildId)
                    .Where(x => entityFilter.Contains(x.EntityId))
                    .Where(x => x.CommandName == commandName)
                    .ToListAsync().ConfigureAwait(false);
            }
            throw new NotSupportedException();
        }
        public async ValueTask<List<Permission>> ListPermissionsAsync(
    long guildId, params long[] entityFilter)
        {
            var enumerable = await repository.ListAsync();
            if(enumerable is IQueryable<Permission> queryable)
            {
                return await queryable.Where(x => x.GuildId == guildId)
                    .Where(x => entityFilter.Contains(x.EntityId))
                    .ToListAsync().ConfigureAwait(false);
            }
            throw new NotSupportedException();
        }

        public async ValueTask SetPermissionAsync(Permission permission)
        {
            if (await ExistsAsync(permission))
            {
                await repository.EditAsync(permission);
            }
            else
            {
                await repository.AddAsync(permission);
            }

            await unit.CommitAsync();
        }
    }
}
