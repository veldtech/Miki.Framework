using System;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Miki.Framework.Commands.Permissions;
using Miki.Framework.Commands.Permissions.Models;
using Xunit;

namespace Miki.Framework.Tests.Commands.Permissions
{
    public class PermissionServiceTests : BaseEntityTest<Permission>
    {
        private const long ValidUserEntity = 0L;
        private const long ValidGuildEntity = 1L;
        private const long ValidRoleEntity = 2L;
        private const long ValidChannelEntity = 3L;

        public PermissionServiceTests()
        {
            using var ctx = NewDbContext();
            ctx.Column.AddRange(
                new Permission
                {
                    CommandName = "test",
                    EntityId = ValidUserEntity,
                    GuildId = ValidGuildEntity,
                    Status = PermissionStatus.Allow,
                    Type = EntityType.User
                },
                new Permission
                {
                    CommandName = "test",
                    EntityId = ValidChannelEntity,
                    GuildId = ValidGuildEntity,
                    Status = PermissionStatus.Default,
                    Type = EntityType.Channel
                },
                new Permission
                {
                    CommandName = "test",
                    EntityId = ValidGuildEntity,
                    GuildId = ValidGuildEntity,
                    Status = PermissionStatus.Allow,
                    Type = EntityType.Guild
                },
                new Permission
                {
                    CommandName = "test",
                    EntityId = ValidRoleEntity,
                    GuildId = ValidGuildEntity,
                    Status = PermissionStatus.Deny,
                    Type = EntityType.Role
                });
            ctx.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {   
            builder?.Entity<Permission>()
                .HasKey(x => new {x.EntityId, x.CommandName, x.GuildId});
        }

        [Fact]
        public async Task ListPermissionsTestAsync()
        {
            await using var unit = NewContext();
            var service = new PermissionService(unit);

            var permissions = await service.ListPermissionsAsync(ValidGuildEntity);
            Assert.Equal(4, permissions.Count);
        }

        [Fact]
        public async Task GetPriorityPermissionTestAsync()
        {
            await using var unit = NewContext();
            var service = new PermissionService(unit);

            var bestPermission = await service.GetPriorityPermissionAsync(
                    ValidGuildEntity, 
                    "test", 
                    new[] { ValidUserEntity, ValidGuildEntity, ValidRoleEntity })
                .ConfigureAwait(false);

            Assert.Equal(PermissionStatus.Allow, bestPermission.Status);
            Assert.Equal(EntityType.User, bestPermission.Type);
            Assert.Equal("test", bestPermission.CommandName);
            Assert.Equal(ValidUserEntity, bestPermission.EntityId);
            Assert.Equal(ValidGuildEntity, bestPermission.GuildId);

            bestPermission = await service.GetPriorityPermissionAsync(
                    ValidGuildEntity,
                    "test",
                    new[] { ValidGuildEntity, ValidRoleEntity })
                .ConfigureAwait(false);

            Assert.Equal(PermissionStatus.Deny, bestPermission.Status);
            Assert.Equal(EntityType.Role, bestPermission.Type);
            Assert.Equal("test", bestPermission.CommandName);
            Assert.Equal(ValidRoleEntity, bestPermission.EntityId);
            Assert.Equal(ValidGuildEntity, bestPermission.GuildId);

            bestPermission = await service.GetPriorityPermissionAsync(
                    ValidGuildEntity,
                    "test",
                    new[] { ValidGuildEntity })
                .ConfigureAwait(false);

            Assert.Equal(PermissionStatus.Allow, bestPermission.Status);
            Assert.Equal(EntityType.Guild, bestPermission.Type);
            Assert.Equal("test", bestPermission.CommandName);
            Assert.Equal(ValidGuildEntity, bestPermission.EntityId);
            Assert.Equal(ValidGuildEntity, bestPermission.GuildId);
        }

        [Fact]
        public async Task SetPermissionTestAsync()
        {
            long entityId = new Random().Next(int.MaxValue);
            var expectedPermission = new Permission
            {
                CommandName = "test",
                EntityId = entityId,
                GuildId = ValidGuildEntity,
                Status = PermissionStatus.Allow,
                Type = EntityType.User
            };

            await using(var unit = NewContext())
            {
                var service = new PermissionService(unit);
                var permission = await service.GetPermissionAsync(
                    entityId, expectedPermission.CommandName, ValidGuildEntity);
                Assert.Null(permission);

                await service.SetPermissionAsync(expectedPermission)
                    .ConfigureAwait(false);
                await unit.CommitAsync()
                    .ConfigureAwait(false);
            }

            await using(var unit = NewContext())
            {
                var service = new PermissionService(unit);
                var permission = await service.GetPermissionAsync(
                    entityId, expectedPermission.CommandName, ValidGuildEntity);
                Assert.Equal(expectedPermission, permission);
            }
        }

        [Fact]
        public async Task UpdatePermissionTestAsync()
        {
            var expectedPermission = new Permission
            {
                CommandName = "test",
                EntityId = ValidUserEntity,
                GuildId = ValidGuildEntity,
                Status = PermissionStatus.Allow,
                Type = EntityType.User
            };

            await using(var unit = NewContext())
            {
                var service = new PermissionService(unit);
                var permission = await service.GetPermissionAsync(
                    ValidUserEntity, expectedPermission.CommandName, ValidGuildEntity);
                Assert.NotNull(permission);

                expectedPermission.Status = PermissionStatus.Deny;
                await service.SetPermissionAsync(expectedPermission)
                    .ConfigureAwait(false);
                await unit.CommitAsync()
                    .ConfigureAwait(false);
            }

            await using(var unit = NewContext())
            {
                var service = new PermissionService(unit);
                var permission = await service.GetPermissionAsync(
                    ValidUserEntity, expectedPermission.CommandName, ValidGuildEntity);
                Assert.Equal(expectedPermission, permission);
            }
        }

        [Fact]
        public async Task DeletePermissionTestAsync()
        {
            await using(var context = NewContext())
            {
                var service = new PermissionService(context);

                var deletedPermission = new Permission
                {
                    CommandName = "test",
                    EntityId = ValidRoleEntity,
                    GuildId = ValidGuildEntity,
                    Status = PermissionStatus.Deny,
                    Type = EntityType.Role
                };
                await service.DeleteAsync(deletedPermission);
                await context.CommitAsync()
                    .ConfigureAwait(false);
            }

            await using(var context = NewContext())
            {
                var service = new PermissionService(context);

                var permissions = await service.ListPermissionsAsync(ValidGuildEntity);
                Assert.Equal(3, permissions.Count);
            }
        }
    }
}
