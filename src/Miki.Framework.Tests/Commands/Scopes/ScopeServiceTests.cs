namespace Miki.Framework.Tests.Commands.Scopes
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Framework.Commands.Scopes;
    using Framework.Commands.Scopes.Models;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class ScopeServiceTests : BaseEntityTest<Scope>
    {
        private const string ValidScope = "scope.testing";
        private const long ValidId = 0L;

        public ScopeServiceTests()
        {
            using var context = NewDbContext();
            context.Set<Scope>()
                .Add(new Scope
                {
                    ScopeId = ValidScope,
                    UserId = ValidId
                });
            context.SaveChanges();
        }

        [Fact]
        public async Task AddScopeTest()
        {
            const long newId = 1L;
            const string newScope = "scope.new";

            await using (var context = NewContext())
            {
                var service = new ScopeService(context);
                await service.AddScopeAsync(new Scope
                {
                    UserId = newId,
                    ScopeId = newScope
                });
                await context.CommitAsync();
            }

            await using (var context = NewContext())
            {
                var service = new ScopeService(context);
                var result = await service.HasScopeAsync(newId, newScope);
                Assert.True(result);
            }
        }

        [Theory]
        [InlineData(ValidId, ValidScope, true)]
        [InlineData(0L, "invalid.scope", false)]
        public async Task HasScopeTest(long id, string scope, bool expected)
        {
            await using var context = NewContext();
            var service = new ScopeService(context);

            bool hasScope = await service.HasScopeAsync(id, scope);

            Assert.Equal(expected, hasScope);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Entity<Scope>()
                .HasKey(x => new {x.UserId, x.ScopeId});
        }
    }
}
