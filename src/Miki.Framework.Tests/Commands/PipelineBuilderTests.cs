namespace Miki.Framework.Tests.Commands
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Discord.Common;
    using Framework.Commands;
    using Framework.Commands.Pipelines;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using Xunit;

    public class CommandPipelineTests
    {
        private readonly IServiceProvider provider;

        private const ulong ValidId = 0L;

        public CommandPipelineTests()
        {
            Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();

            Mock<IServiceScope> serviceScope = new Mock<IServiceScope>();
            serviceScope.Setup(x => x.ServiceProvider)
                .Returns(serviceProviderMock.Object);

            Mock<IServiceScopeFactory> serviceScopeProviderMock = new Mock<IServiceScopeFactory>();
            serviceScopeProviderMock.Setup(x => x.CreateScope())
                .Returns(serviceScope.Object);

            serviceProviderMock
                .Setup(x => x.GetService(It.Is<Type>(y => y == typeof(IServiceScopeFactory))))
                .Returns(serviceScopeProviderMock.Object);

            provider = serviceProviderMock.Object;
        }

        [Fact]
        public void AddPipelineTest()
        {
            var pipelineStage = Mock.Of<IPipelineStage>();

            CommandPipelineBuilder builder = new CommandPipelineBuilder(provider);
            builder.UseStage(pipelineStage);
            var pipeline = builder.Build();

            Assert.Equal(1, pipeline.PipelineStages.Count);
            Assert.Contains(pipelineStage, pipeline.PipelineStages);
        }

        [Theory]
        [InlineData(ValidId, true)]
        [InlineData(1L, false)]
        public async Task RunPipelineTestAsync(ulong id, bool success)
        {
            using var @lock = new Semaphore(0, 1);

            var pipelineStage = new Mock<IPipelineStage>();
            pipelineStage.Setup(x => x.CheckAsync(
                    It.IsAny<IDiscordMessage>(), 
                    It.IsAny<IMutableContext>(), 
                    It.IsAny<Func<ValueTask>>()))
                .Returns<IDiscordMessage, IMutableContext, Func<ValueTask>>((x, y, z) =>
                {
                    if (x.Id != ValidId)
                    {
                        y.SetExecutable(
                            new ExecutableAdapter(_ => throw new InvalidOperationException()));
                        return z();
                    }
                    else
                    {
                        y.SetExecutable(
                            new ExecutableAdapter(_ => default));
                    }
                    return z();
                });

            var messageMock = new Mock<IDiscordMessage>();
            messageMock.Setup(x => x.Id)
                .Returns(id);

            var pipeline = new CommandPipelineBuilder(provider)
                .UseStage(pipelineStage.Object)
                .Build();
            
            pipeline.OnExecuted += (x) =>
            {
                Assert.Equal(success, x.Success);
                @lock.Release();
                return default;
            };

            await pipeline.ExecuteAsync(messageMock.Object);

            @lock.WaitOne();
        }
    }
}
