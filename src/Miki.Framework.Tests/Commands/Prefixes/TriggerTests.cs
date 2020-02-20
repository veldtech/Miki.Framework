namespace Miki.Framework.Tests.Commands.Prefixes
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Miki.Cache;
    using Miki.Cache.InMemory;
    using Miki.Discord.Common;
    using Miki.Framework.Commands;
    using Miki.Framework.Commands.Prefixes.Models;
    using Miki.Framework.Commands.Prefixes.Triggers;
    using Miki.Framework.Commands.Stages;
    using Miki.Serialization.Protobuf;
    using Moq;
    using Xunit;

    public interface TestDiscordMessage : IDiscordTextChannel, IDiscordGuildChannel
    {

    }

    public class TriggerTests : BaseEntityTest<Prefix>
    {
        public TriggerTests()
        {
            using var context = NewDbContext();
            context.Set<Prefix>().AddRange(
                new Prefix
                {
                    GuildId = 0L,
                    DefaultValue = "test.",
                    Value = "test."
                },
                new Prefix
                {
                    GuildId = 1L,
                    DefaultValue = "test.",
                    Value = "new."
                });
            context.SaveChanges();
        }

        [Fact]
        public async Task PrefixTriggerDoesTrigger()
        {
            var trigger = new PrefixTrigger("test.");
            var mock = new Mock<IContext>();
            mock.Setup(x => x.GetContext(It.IsAny<string>()))
                .Returns("test.command");
            
            var result = await trigger.CheckTriggerAsync(mock.Object)
                .ConfigureAwait(false);

            Assert.Equal("test.", result);
        }

        [Fact]
        public async Task AcceptDefaultCommandFromDynamicPrefix()
        {
            var trigger = new DynamicPrefixTrigger("test.");
            var mockContext = new Mock<IContext>();

            mockContext.Setup(x => x.GetService(
                    It.Is<Type>(y => y.IsAssignableFrom(typeof(DbContext)))))
                .Returns(NewDbContext());
            mockContext.Setup(x => x.GetService(
                    It.Is<Type>(y => y.IsAssignableFrom(typeof(ICacheClient)))))
                .Returns(new InMemoryCacheClient(new ProtobufSerializer()));

            var mockChannel = new Mock<TestDiscordMessage>();
            mockChannel.Setup(x => x.Id).Returns(0L);
            mockContext.Setup(x => x.GetContext(
                    It.Is<string>(y => y == CorePipelineStage.QueryContextKey)))
                .Returns("test.command");
            mockContext.Setup(x => x.GetContext(
                    It.Is<string>(y => y == FetchDataStage.ChannelArgumentKey)))
                .Returns(mockChannel.Object);

            var result = await trigger.CheckTriggerAsync(mockContext.Object)
                .ConfigureAwait(false);

            Assert.Equal("test.", result);
        }


        [Fact]
        public async Task AcceptModifiedCommandFromDynamicPrefix()
        {
            var trigger = new DynamicPrefixTrigger("test.");
            var mockContext = new Mock<IContext>();

            mockContext.Setup(x => x.GetService(
                    It.Is<Type>(y => y.IsAssignableFrom(typeof(DbContext)))))
                .Returns(NewDbContext());
            mockContext.Setup(x => x.GetService(
                    It.Is<Type>(y => y.IsAssignableFrom(typeof(ICacheClient)))))
                .Returns(new InMemoryCacheClient(new ProtobufSerializer()));

            var mockChannel = new Mock<TestDiscordMessage>();
            mockChannel.Setup(x => x.Id).Returns(1L);
            mockContext.Setup(x => x.GetContext(
                    It.Is<string>(y => y == CorePipelineStage.QueryContextKey)))
                .Returns("test.command");
            mockContext.Setup(x => x.GetContext(
                    It.Is<string>(y => y == FetchDataStage.ChannelArgumentKey)))
                .Returns(mockChannel.Object);

            var result = await trigger.CheckTriggerAsync(mockContext.Object)
                .ConfigureAwait(false);

            Assert.Equal("test.", result);
        }

        protected override void OnModelCreating([NotNull] ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Prefix>()
                .HasKey(x => new {x.GuildId, x.DefaultValue});

        }
    }
}
