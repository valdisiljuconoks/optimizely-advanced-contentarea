using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Extensions;

namespace EPiBootstrapArea.Tests
{
    public class EqualityTests
    {
        [Theory]
        [InlineData("name", 12, 12, 12, 12, "tag", "name", 12, 12, 12, 12, "tag", true)]
        [InlineData("name", 12, 12, 12, 12, "tag", "name", 3, 12, 12, 12, "tag", false)]
        public void TestFewInstances(
            string xname,
            int xlarge,
            int xmedium,
            int xsmall,
            int xxsmall,
            string xtag,
            string yname,
            int ylarge,
            int ymedium,
            int ysmall,
            int yxsmall,
            string ytag,
            bool validationResult)
        {
            var modeX = new DisplayModeFallback
            {
                Name = xname,
                LargeScreenWidth = xlarge,
                MediumScreenWidth = xmedium,
                SmallScreenWidth = xsmall,
                ExtraSmallScreenWidth = xxsmall,
                Tag = xtag,
            };

            var modeY = new DisplayModeFallback
            {
                Name = yname,
                LargeScreenWidth = ylarge,
                MediumScreenWidth = ymedium,
                SmallScreenWidth = ysmall,
                ExtraSmallScreenWidth = yxsmall,
                Tag = ytag,
            };

            var comparer = new DisplayModeFallbackComparer();
            Assert.Equal(validationResult, comparer.Equals(modeX, modeY));
        }
    }
}
