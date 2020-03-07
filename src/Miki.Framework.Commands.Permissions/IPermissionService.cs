namespace Miki.Framework.Commands.Permissions
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Miki.Framework.Commands.Permissions.Models;

    public interface IPermissionService
    {
        /// <summary>
        /// Removes a specified permission from the permission list.
        /// </summary>
        ValueTask DeleteAsync(Permission permission);

        /// <summary>
        /// Checks if a permission with key types <see cref="Permission.EntityId"/>,
        /// <see cref="Permission.CommandName"/>, and <see cref="Permission.GuildId"/> already exists
        /// in the database.
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        ValueTask<bool> ExistsAsync(Permission permission);

        /// <summary>
        /// Gets the specified permission if exists.
        /// </summary>
        /// <param name="entityId">ID of the entity, these can be users, roles, channels, etc.</param>
        /// <param name="commandName">Name of the IExecutable you want to apply this permission for.
        /// </param>
        /// <param name="guildId">Id of the server to apply this permission to.</param>
        ValueTask<Permission> GetPermissionAsync(long entityId, string commandName, long guildId);

        /// <summary>
        /// Gets the top priority permission, can only be of type <see cref="PermissionStatus.Allow"/>
        /// and <see cref="PermissionStatus.Deny"/>. People with a special Administrator flag get to go
        /// through always.
        /// </summary>
        /// <param name="entityIds">All entity Ids whitelisted for this permission check.</param>
        /// <param name="commandName">Name of the IExecutable you want to apply this permission for.
        /// </param>
        /// <param name="guildId">Id of the server to apply this permission to.</param>
        ValueTask<Permission> GetPriorityPermissionAsync(
            long guildId, string commandName, long[] entityIds);

        /// <summary>
        /// Gets the top priority permission, can only be of type <see cref="PermissionStatus.Allow"/>
        /// and <see cref="PermissionStatus.Deny"/>. People with a special Administrator flag get to go
        /// through always.
        /// </summary>
        /// <param name="context">The current command pipeline context.</param>
        ValueTask<Permission> GetPriorityPermissionAsync(IContext context);

        /// <summary>
        /// Lists all permissions set for a single guild.
        /// </summary>
        /// <param name="guildId">Guild that you want to see all permissions of.</param>
        ValueTask<IReadOnlyList<Permission>> ListPermissionsAsync(long guildId);

        /// <summary>
        /// A guild-based permission list with additional specification filter to it. 
        /// </summary>
        /// <param name="guildId">Guild that you want to see all permissions of.</param>
        /// <param name="entityFilter">All entity Ids whitelisted for this permission check.</param>
        public ValueTask<List<Permission>> ListPermissionsAsync(
            long guildId, params long[] entityFilter);

        /// <summary>
        /// A more specific version to list all permissions. Can be used to specify to which entities
        /// you would like to see permissions of.
        /// </summary>
        /// <param name="guildId">Guild that you want to see all permissions of.</param>
        /// <param name="commandName">Name of the IExecutable you want to apply this permission for.
        /// </param>
        /// <param name="entityFilter">All entity Ids whitelisted for this permission check.</param>
        ValueTask<IReadOnlyList<Permission>> ListPermissionsAsync(
            long guildId, string commandName, params long[] entityFilter);

        /// <summary>
        /// Sets or Creates a new permission.
        /// </summary>
        ValueTask SetPermissionAsync(Permission permission);
    }
}
