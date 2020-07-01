namespace Miki.Framework.Tests.Arguments
{
    using System;
    using System.Reflection;
    using Miki.Framework.Arguments;
    using Xunit;

    public class TypedArgumentPackTests
    {
        private TypedArgumentPack GetPack(params string[] arguments)
        {
            var argumentPack = new ArgumentPack(arguments);
            var provider = new ArgumentParseProvider();
            provider.SeedAssembly(Assembly.GetExecutingAssembly());
            return new TypedArgumentPack(argumentPack, provider);
        }

        [Fact]
        public void ParseCustomEnumTest()
        {
            var pack = GetPack("true");
            Assert.True(pack.Take<ArgumentParserTests.TestValues>(out var result));
            Assert.Equal(ArgumentParserTests.TestValues.True, result);
        }

        [Fact]
        public void ParseStringTest()
        {
            var pack = GetPack("test");
            Assert.True(pack.Take<string>(out var result));
            Assert.Equal("test", result);
        }

        [Fact]
        public void ParseIntTest()
        {
            var pack = GetPack("12");
            Assert.True(pack.Take<int>(out var result));
            Assert.Equal(12, result);
        }

        [Fact]
        public void ParseGuidTest()
        {
            var guid = Guid.NewGuid();
            var pack = GetPack(guid.ToString());
            Assert.True(pack.Take<Guid>(out var result));
            Assert.Equal(guid, result);
        }
    }
}
