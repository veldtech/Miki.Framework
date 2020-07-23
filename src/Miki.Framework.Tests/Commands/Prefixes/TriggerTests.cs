using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Miki.Cache;
using Miki.Cache.InMemory;
using Miki.Discord.Common;
using Miki.Framework.Commands;
using Miki.Framework.Commands.Prefixes.Models;
using Miki.Framework.Commands.Prefixes.Triggers;
using Miki.Serialization.Protobuf;
using Moq;
using Xunit;

namespace Miki.Framework.Tests.Commands.Prefixes
{
    public interface ITestDiscordMessage : IDiscordMessage
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
        public async Task PrefixTriggerDoesTriggerAsync()
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
        public async Task MentionTriggerDoesTriggerAsync()
        {
            var mention = "<@12065984510520>";
            var trigger = new MentionTrigger();
            var mock = new TestContextObject();

            mock.SetContext(CorePipelineStage.QueryContextKey, mention + " test");

            var selfUserMock = new Mock<IDiscordSelfUser>();
            selfUserMock.Setup(x => x.Id).Returns(12065984510520);

            var clientMock = new Mock<IDiscordClient>();
            clientMock.Setup(x => x.GetSelfAsync())
                .Returns(Task.FromResult(selfUserMock.Object));
            mock.SetService(typeof(IDiscordClient), clientMock.Object);

            var result = await trigger.CheckTriggerAsync(mock)
                .ConfigureAwait(false);

            Assert.Equal(mention, result);
        }


        [Fact]
        public async Task AcceptDefaultCommandFromDynamicPrefixAsync()
        {
            var trigger = new DynamicPrefixTrigger("test.");
            var mockContext = new TestContextObject();

            mockContext.SetService(typeof(DbContext), NewDbContext());
            mockContext.SetService(
                typeof(ICacheClient), new InMemoryCacheClient(new ProtobufSerializer()));

            var mockMessage = new Mock<ITestDiscordMessage>();
            mockMessage.Setup(x => x.Id).Returns(0L);
            mockContext.SetContext(CorePipelineStage.MessageContextKey, mockMessage.Object);
            mockContext.SetContext(CorePipelineStage.QueryContextKey, "test.command");

            var result = await trigger.CheckTriggerAsync(mockContext)
                .ConfigureAwait(false);

            Assert.Equal("test.", result);
        }


        [Fact]
        public async Task AcceptModifiedCommandFromDynamicPrefixAsync()
        {
            var trigger = new DynamicPrefixTrigger("test.");
            var mockContext = new Mock<IContext>();

            mockContext.Setup(x => x.GetService(
                    It.Is<Type>(y => y.IsAssignableFrom(typeof(DbContext)))))
                .Returns(NewDbContext());
            mockContext.Setup(x => x.GetService(
                    It.Is<Type>(y => y.IsAssignableFrom(typeof(ICacheClient)))))
                .Returns(new InMemoryCacheClient(new ProtobufSerializer()));

            var mockMessage = new Mock<ITestDiscordMessage>();
            mockMessage.Setup(x => x.Id).Returns(1L);
            mockContext.Setup(x => x.GetContext(
                    It.Is<string>(y => y == CorePipelineStage.MessageContextKey)))
                .Returns(mockMessage.Object);
            mockContext.Setup(x => x.GetContext(
                    It.Is<string>(y => y == CorePipelineStage.QueryContextKey)))
                .Returns("test.command");

            var result = await trigger.CheckTriggerAsync(mockContext.Object)
                .ConfigureAwait(false);

            Assert.Equal("test.", result);
        }

        protected override void OnModelCreating([NotNull] ModelBuilder builder)
        {
            if(builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            base.OnModelCreating(builder);

            builder.Entity<Prefix>()
                .HasKey(x => new {x.GuildId, x.DefaultValue});

        }
    }
}
