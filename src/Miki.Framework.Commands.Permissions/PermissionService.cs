using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Miki.Discord.Common;
using Miki.Framework.Commands.Permissions.Models;
using Miki.Patterns.Repositories;

namespace Miki.Framework.Commands.Permissions
{
    /// <summary>
    /// Service that handles permissions, used as a basic ACL permission system.
    /// </summary>
    public class PermissionService : IPermissionService
    {
        private readonly IUnitOfWork unit;
        private readonly IAsyncRepository<Permission> repository;

        /// <summary>
        /// Initializes the permission service, use a scoped IUnitOfWork.
        /// </summary>
        public PermissionService(IUnitOfWork unit)
        {
            this.unit = unit ?? throw new ArgumentNullException(nameof(unit));
            this.repository = unit.GetRepository<Permission>();
        }

        /// <inheritdoc/>
        public async ValueTask DeleteAsync(Permission permission)
        {
            await repository.DeleteAsync(permission);
        }

        /// <inheritdoc/>
        public async ValueTask<bool> ExistsAsync(Permission permission)
        {
            var enumerable = await repository.ListAsync();
            if(enumerable is IQueryable<Permission> queryable)
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

        /// <inheritdoc/>
        public ValueTask<Permission> GetPermissionAsync(
            long entityId,
            string commandName,
            long guildId)
        {
            return repository.GetAsync(entityId, commandName, guildId);
        }

        /// <inheritdoc/>
        public async ValueTask<Permission> GetPriorityPermissionAsync(
            long guildId,
            string commandName,
            long[] entityIds)
        {
            var enumerable = await repository.ListAsync();
            if(enumerable is IQueryable<Permission>)
            {
                return await enumerable.AsQueryable()
                    .Where(x => x.CommandName == commandName && x.GuildId == guildId)
                    .Where(x => entityIds.Contains(x.EntityId))
                    .OrderBy(x => x.Type)
                    .FirstOrDefaultAsync(x => x.Status != PermissionStatus.Default)
                    .ConfigureAwait(false);
            }
            return enumerable.Where(x => x.CommandName == commandName && x.GuildId == guildId)
                .Where(x => entityIds.Contains(x.EntityId))
                .OrderBy(x => x.Type)
                .FirstOrDefault(x => x.Status != PermissionStatus.Default);
        }

        /// <inheritdoc/>
        public async ValueTask<Permission> GetPriorityPermissionAsync(
            IContext context)
        {
            if(context == null)
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
                        GuildId = (long) context.GetGuild().Id,
                        Status = PermissionStatus.Allow,
                        Type = EntityType.User
                    };
                }
            }

            List<long> idList = new List<long>();
            if(context.GetMessage() != null)
            {
                idList.Add((long) context.GetMessage().Author.Id);
                idList.Add((long) context.GetMessage().ChannelId);
            }

            if(context.GetGuild() != null)
            {
                idList.Add((long) context.GetGuild().Id);
            }

            if(context.GetMessage().Author is IDiscordGuildUser user
               && user.RoleIds != null
               && user.RoleIds.Any())
            {
                idList.AddRange(user.RoleIds.Select(x => (long) x));
            }

            return await GetPriorityPermissionAsync(
                (long) context.GetGuild().Id, context.Executable.ToString(), idList.ToArray());
        }

        /// <inheritdoc/>
        public async ValueTask<IReadOnlyList<Permission>> ListPermissionsAsync(long guildId)
        {
            var enumerable = await repository.ListAsync();
            if(enumerable is IQueryable<Permission>)
            {
                return await enumerable.AsQueryable()
                    .Where(x => x.GuildId == guildId).ToListAsync()
                    .ConfigureAwait(false);
            }
            return enumerable.Where(x => x.GuildId == guildId).ToList();
        }

        /// <inheritdoc/>
        public async ValueTask<IReadOnlyList<Permission>> ListPermissionsAsync(
            long guildId,
            string commandName,
            params long[] entityFilter)
        {
            var enumerable = await repository.ListAsync();
            if(enumerable is IQueryable<Permission> queryable)
            {
                return await queryable.Where(x => x.GuildId == guildId)
                    .Where(x => entityFilter.Contains(x.EntityId))
                    .Where(x => x.CommandName == commandName)
                    .ToListAsync()
                    .ConfigureAwait(false);
            }

            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public async ValueTask<List<Permission>> ListPermissionsAsync(
            long guildId,
            params long[] entityFilter)
        {
            var enumerable = await repository.ListAsync();
            if(enumerable is IQueryable<Permission> queryable)
            {
                return await queryable.Where(x => x.GuildId == guildId)
                    .Where(x => entityFilter.Contains(x.EntityId))
                    .ToListAsync()
                    .ConfigureAwait(false);
            }

            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public async ValueTask SetPermissionAsync(Permission permission)
        {
            if(await ExistsAsync(permission))
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
