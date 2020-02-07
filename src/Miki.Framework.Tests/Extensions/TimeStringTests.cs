using System;

namespace Miki.Framework.Tests.Extensions
{
    using Xunit;

    public class TimeStringTests
    {
        [Theory]
        [InlineData("+1s", 1)]
        [InlineData("1s +1m", 61)]
        [InlineData("1s 1m 1h", 3661)]
        [InlineData("1d", 86400)]
        [InlineData("-1s", -1)]
        [InlineData("1s +1s -1s 1M +1m -1m 1h +1h -1h 1d +1d -1d 1w +1w -1w 1y +1y -1y", 32230861)]
        [InlineData("328492837498273498273498s", 0)]
        [InlineData("", 0)]
        public void TimeStringTest(string query, long totalSeconds)
        {
            Assert.Equal(totalSeconds, query.GetTimeFromString().TotalSeconds);
        }

        [Theory]
        [InlineData("50000y", typeof(OverflowException))]
        public void InvalidTimeStringTest(string query, Type t)
        {
            Assert.Throws(t, () => query.GetTimeFromString());
        }
    }
}
