namespace Miki.Framework.Tests.Arguments
{
    using System;
    using Miki.Framework.Arguments;
    using Miki.Framework.Arguments.Parsers;
    using Xunit;

    public class ArgumentParserTests
    {
        private enum TestValues
        {
            True,
            False
        }

        [Fact]
        public void CanParseUlongTest()
        {
            ArgumentPack pack = new ArgumentPack("22903433382093");

            var parser = new UlongArgumentParser();
            Assert.True(parser.CanParse(pack, typeof(ulong)));

            var result = (ulong)parser.Parse(pack, typeof(ulong));
            Assert.Equal(22903433382093UL, result);
        }

        [Fact]
        public void CanParseGuidTest()
        {
            var guid = Guid.NewGuid();
            ArgumentPack pack = new ArgumentPack(guid.ToString());

            var parser = new GuidArgumentParser();
            Assert.True(parser.CanParse(pack, typeof(Guid)));

            var result = (Guid)parser.Parse(pack, typeof(Guid));
            Assert.Equal(guid, result);
        }

        [Fact]
        public void CanParseBasicStringTest()
        {
            ArgumentPack pack = new ArgumentPack("test", "string");

            var parser = new StringArgumentParser();
            Assert.True(parser.CanParse(pack, typeof(string)));

            var result = (string)parser.Parse(pack, typeof(string));
            Assert.Equal("test", result);
        }

        [Fact]
        public void CanParseMultiwordStringTest()
        {
            ArgumentPack pack = new ArgumentPack("\"test", "string\"");

            var parser = new StringArgumentParser();
            Assert.True(parser.CanParse(pack, typeof(string)));

            var result = (string)parser.Parse(pack, typeof(string));
            Assert.Equal("test string", result);
        }

        [Fact]
        public void CanParseBoolTest()
        {
            ArgumentPack pack = new ArgumentPack("true");

            var parser = new BooleanArgumentParser();
            Assert.True(parser.CanParse(pack, typeof(bool)));

            var result = (bool)parser.Parse(pack, typeof(bool));
            Assert.True(result);
        }

        [Fact]
        public void CanParseIntTest()
        {
            ArgumentPack pack = new ArgumentPack("1000");

            var parser = new IntArgumentParser();
            Assert.True(parser.CanParse(pack, typeof(int)));

            var result = (int)parser.Parse(pack, typeof(int));
            Assert.Equal(1000, result);
        }

        [Fact]
        public void CanParseSuffixedIntTest()
        {
            ArgumentPack pack = new ArgumentPack("1k");

            var parser = new SuffixedIntArgumentParser();
            Assert.True(parser.CanParse(pack, typeof(int)));

            var result = (int)parser.Parse(pack, typeof(int));
            Assert.Equal(1000, result);
        }

        [Fact]
        public void ParseCustomEnumTest()
        {
            ArgumentPack pack = new ArgumentPack("true");

            var parser = new EnumArgumentParser();
            Assert.True(parser.CanParse(pack, typeof(TestValues)));

            var result = (TestValues)parser.Parse(pack, typeof(TestValues));
            Assert.Equal(TestValues.True, result);
        }
    }
}
