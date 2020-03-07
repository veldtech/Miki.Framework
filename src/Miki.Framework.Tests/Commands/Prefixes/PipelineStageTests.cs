namespace Miki.Framework.Tests.Commands.Prefixes
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Miki.Framework.Commands.Prefixes;
    using Moq;
    using Xunit;

    public class PipelineStageTests
    {

        [Theory]
        [InlineData(">")]
        [InlineData("testlong. withspaces.")]
        [InlineData("<@90285094092840980495> ")]
        public async Task TestWithValidContextAsync(string prefix)
        {
            var funcMock = new Mock<Func<ValueTask>>();
            var serviceMock = new Mock<IPrefixService>();
            serviceMock.Setup(x => x.MatchAsync(It.IsAny<IContext>()))
                .Returns(new ValueTask<string>(prefix));

            var stage = new PipelineStageTrigger(serviceMock.Object);

            var context = new TestContextObject();
            context.SetQuery(prefix + "ping");

            await stage.CheckAsync(null, context, funcMock.Object);

            funcMock.Verify(x => x.Invoke(), Times.Once, "Didn't invoke next()");

            Assert.Equal("ping", context.GetQuery());
            Mock.Verify(funcMock);
        }
    }
}
