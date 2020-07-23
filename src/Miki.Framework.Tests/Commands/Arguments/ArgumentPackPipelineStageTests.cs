using System.Threading.Tasks;
using Miki.Framework.Commands.Pipelines;
using Xunit;

namespace Miki.Framework.Tests.Commands.Arguments
{
    public class ArgumentPackPipelineStageTests
    {
        [Fact]
        public async Task DoesRemoveEmptySpacesAsync()
        {
            ArgumentPackBuilder builder = new ArgumentPackBuilder();
            TestContextObject context = new TestContextObject();
            context.SetQuery("test  string");

            await builder.CheckAsync(null, context, () => default);
            Assert.Equal(2, context.GetArgumentPack().Pack.Length);
            Assert.Equal("test", context.GetArgumentPack().Pack.Take());
            Assert.Equal("string", context.GetArgumentPack().Pack.Take());
        }
    }
}
