namespace Miki.Framework.Tests.Commands.Prefixes
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Framework.Commands.Prefixes.Models;
    using Microsoft.EntityFrameworkCore;
    using Miki.Framework.Commands.Prefixes;
    using Miki.Framework.Commands.Prefixes.Triggers;
    using Moq;
    using Xunit;

    // TODO(velddev): rework prefixes to have a better system to better seperate 

    public class PrefixServiceTests
    {
        [Theory]
        [InlineData(".")]
        [InlineData(null)]
        public async Task MatchesAsync(string prefix)
        {
            var mockTrigger = new Mock<ITrigger>();
            mockTrigger.Setup(x => x.CheckTriggerAsync(It.IsAny<IContext>()))
                .Returns(Task.FromResult(prefix));

            var service = new PrefixService(
                new PrefixCollectionBuilder()
                    .AddAsDefault(mockTrigger.Object)
                    .Build());

            var mock = new Mock<IContext>();
            var result = await service.MatchAsync(mock.Object);
            Assert.Equal(prefix, result);
        }

        [Theory]
        [InlineData(".", ".")]
        [InlineData(null, "backup")]
        public async Task MatchesMultipleAsync(string prefix, string expected)
        {
            var mockTrigger = new Mock<ITrigger>();
            mockTrigger.Setup(x => x.CheckTriggerAsync(It.IsAny<IContext>()))
                .Returns(Task.FromResult(prefix));

            var mockTriggerBackup = new Mock<ITrigger>();
            mockTriggerBackup.Setup(x => x.CheckTriggerAsync(It.IsAny<IContext>()))
                .Returns(Task.FromResult("backup"));

            var service = new PrefixService(
                new PrefixCollectionBuilder()
                    .AddAsDefault(mockTrigger.Object)
                    .Add(mockTriggerBackup.Object)
                    .Build());

            var mock = new Mock<IContext>();
            var result = await service.MatchAsync(mock.Object);
            Assert.Equal(expected, result);
        }
    }
}
