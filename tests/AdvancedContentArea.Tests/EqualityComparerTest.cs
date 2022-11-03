using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Xunit.Assert;

namespace TechFellow.Optimizely.AdvancedContentArea.Tests;

public class EqualityComparerTest
{
    [Fact]
    public void CompareTwoList_SameContent_OnlyUniqueItems()
    {
        var l1 = new List<DisplayModeFallback>
        {
            new()
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
            new()
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

        Single(result);
    }
}
