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
        private readonly IAsyncRepository<Permission> repository;

        public PermissionService(IAsyncRepository<Permission> repository)
        {
            this.repository = repository;
        }

        public async Task<bool> ExistsAsync(Permission permission)
        {
            var enumerable = await repository.ListAsync();
            if(enumerable is IQueryable<Permission> queryable)
            {
                return await queryable.AnyAsync(
                    x => x.EntityId == permission.EntityId
                      && x.CommandName == permission.CommandName
                      && x.GuildId == permission.GuildId);
            }
            throw new NotSupportedException();
        }

        public ValueTask<Permission> GetPermissionAsync(
            long entityId, string commandName, long guildId)
        {
            return repository.GetAsync(entityId, commandName, guildId);
        }
        public async Task<Permission> GetPriorityPermissionAsync(
            long guildId, string commandName, long[] entityIds)
        {
            var enumerable = await repository.ListAsync();
            if(enumerable is IQueryable<Permission> queryable)
            {
                return await queryable
                    .Where(x => x.CommandName == commandName && x.GuildId == guildId)
                    .Where(x => entityIds.Contains(x.EntityId))
                    .OrderBy(x => x.Type)
                    .FirstOrDefaultAsync(x => x.Status != PermissionStatus.Default);
            }
            throw new NotSupportedException();
        }
        public async Task<Permission> GetPriorityPermissionAsync(
            IContext context)
        {
            if(context.GetMessage().Author is IDiscordGuildUser gu)
            {
                if(await gu.HasPermissionsAsync(GuildPermission.Administrator))
                {
                    return new Permission()
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
            if(enumerable is IQueryable<Permission> queryable)
            {
                return await queryable.Where(x => x.GuildId == guildId)
                    .ToListAsync();
            }
            throw new NotSupportedException();
        }
        public async Task<List<Permission>> ListPermissionsAsync(
            long guildId, string commandName, params long[] entityFilter)
        {
            var enumerable = await repository.ListAsync();
            if(enumerable is IQueryable<Permission> queryable)
            {
                return await queryable.Where(x => x.GuildId == guildId)
                    .Where(x => entityFilter.Contains(x.EntityId))
                    .Where(x => x.CommandName == commandName)
                    .ToListAsync();
            }
            throw new NotSupportedException();
        }
        public async Task<List<Permission>> ListPermissionsAsync(
    long guildId, params long[] entityFilter)
        {
            var enumerable = await repository.ListAsync();
            if(enumerable is IQueryable<Permission> queryable)
            {
                return await queryable.Where(x => x.GuildId == guildId)
                    .Where(x => entityFilter.Contains(x.EntityId))
                    .ToListAsync();
            }
            throw new NotSupportedException();
        }

        public async Task SetPermissionAsync(Permission permission)
        {        
            if(await ExistsAsync(permission))
            {
                await repository.EditAsync(permission);
            }
            else
            {
                await repository.AddAsync(permission);
            }
        }
    }
}
