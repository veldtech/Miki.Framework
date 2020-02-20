namespace Miki.Framework.Tests.Commands.Prefixes
{
    using System.Diagnostics.CodeAnalysis;
    using Framework.Commands.Prefixes.Models;
    using Microsoft.EntityFrameworkCore;

    // TODO(velddev): rework prefixes to have a better system to better seperate 

    public class PrefixServiceTests : BaseEntityTest<Prefix>
    {
        private const long GuildId = 0;
        private const long DefaultId = 1;
        private const string DefaultValue = ">";
        private const string Value = "!";

        public PrefixServiceTests()
        {
            using var context = NewDbContext();
            context.Set<Prefix>().AddRange(
                new Prefix
                {
                    GuildId = GuildId,
                    DefaultValue = DefaultValue,
                    Value = Value
                },
                new Prefix
                {
                    GuildId = DefaultId,
                    DefaultValue = DefaultValue,
                    Value = DefaultValue
                });
            context.SaveChanges();
        }
        
        protected override void OnModelCreating([NotNull] ModelBuilder builder)
        {
            builder?.Entity<Prefix>()
                .HasKey(x => new {x.GuildId, x.DefaultValue});
        }
    }
}
