namespace Miki.Framework.Tests.Commands.Prefixes
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Discord.Common;
    using Framework.Commands.Permissions;
    using Framework.Commands.Prefixes;
    using Framework.Commands.Prefixes.Models;
    using Framework.Commands.Prefixes.Triggers;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Xunit;

    // TODO(velddev): rework prefixes to have a better system to better seperate 

    public class PrefixServiceTests : BaseEntityTest<Prefix>
    {
        private const long ValidGuildId = 0;
        private const long ValidDefaultId = 1;
        private const string ValidDefaultValue = ">";
        private const string ValidValue = "!";

        public PrefixServiceTests()
        {
            using var context = NewDbContext();
            context.Set<Prefix>().AddRange(
                new Prefix
                {
                    GuildId = ValidGuildId,
                    DefaultValue = ValidDefaultValue,
                    Value = ValidValue
                },
                new Prefix
                {
                    GuildId = ValidDefaultId,
                    DefaultValue = ValidDefaultValue,
                    Value = ValidDefaultValue
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
