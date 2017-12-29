using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace EPiBootstrapArea.Tests
{
    public class EqualityComparerTest
    {
        [Fact]
        public void CompareTwoList_SameContent_OnlyUniqueItems()
        {
            var l1 = new List<DisplayModeFallback>
            {
                new DisplayModeFallback
                {
                    Name = "Full width (1/1)",
                    Tag = ContentAreaTags.FullWidth,
                    LargeScreenWidth = 12,
                    MediumScreenWidth = 12,
                    SmallScreenWidth = 12,
                    ExtraSmallScreenWidth = 12,
                    Icon = "epi-icon__layout--full"
                }
            };

            var l2 = new List<DisplayModeFallback>
            {
                new DisplayModeFallback
                {
                    Name = "Full width (1/1)",
                    Tag = ContentAreaTags.FullWidth,
                    LargeScreenWidth = 12,
                    MediumScreenWidth = 12,
                    SmallScreenWidth = 12,
                    ExtraSmallScreenWidth = 12,
                    Icon = "epi-icon__layout--full"
                }
            };

            var result = l1.Union(l2, new DisplayModeFallbackComparer()).ToList();

            Assert.Equal(1, result.Count);
        }
    }
}
