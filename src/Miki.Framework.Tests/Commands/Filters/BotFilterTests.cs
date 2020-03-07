namespace Miki.Framework.Tests.Commands.Filters
{
    using System.Threading.Tasks;
    using Miki.Framework.Commands.Filters;
    using Moq;
    using Miki.Discord.Common;
    using Xunit;

    public class BotFilterTests
    {
        private readonly BotFilter filter = new BotFilter();

        [Fact]
        public async Task IsBotTestAsync()
        {
            var context = NewContext(true);

            var b = await filter.CheckAsync(context);

            Assert.False(b);
        }

        [Fact]
        public async Task IsNotBotTestAsync()
        {
            var context = NewContext(false);

            var b = await filter.CheckAsync(context);

            Assert.True(b);
        }

        private IContext NewContext(bool val)
        {
            var author = new Mock<IDiscordUser>();
            author.Setup(x => x.IsBot)
                .Returns(val);

            var message = new Mock<IDiscordMessage>();
            message.Setup(x => x.Author)
                .Returns(author.Object);

            var context = new Mock<IContext>();
            context.Setup(x => x.GetContext("framework-message"))
                .Returns(message.Object);

            return context.Object;
        }

    }
}
