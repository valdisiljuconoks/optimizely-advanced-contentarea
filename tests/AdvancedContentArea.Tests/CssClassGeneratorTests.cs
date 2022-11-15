using Xunit;

namespace TechFellow.Optimizely.AdvancedContentArea.Tests;

public class CssClassGeneratorTests
{
    [Theory]
    [InlineData(4, null, 6, null, 8, null, 12, null, "col-lg-4 col-md-6 col-sm-8 col-xs-12")]
    [InlineData(4, "custom-large-{0}", 6, "medium-{0}", 8, "{0}-small", 12, "xs{0}", "custom-large-4 medium-6 8-small xs12")]
    [InlineData(4, "custom-large-{0}", 6, "medium-{0}", 8, "{0}-small", 12, null, "custom-large-4 medium-6 8-small col-xs-12")]
    [InlineData(4, "custom-large-{0}", 6, "medium-{0}", 8, "{0}-small", 12, "test", "custom-large-4 medium-6 8-small test")]
    [InlineData(4, "custom-large-{0}", 6, "medium-{0}", 8, "{0}-small", 12, "exceptional-{5}", "custom-large-4 medium-6 8-small")]
    public void TestCssClassGenerationForItem_DisplayOptionsWithPatterns(
        int lgSize,
        string lgPattern,
        int mdSize,
        string mdPattern,
        int smSize,
        string smPattern,
        int xsSize,
        string xsPattern,
        string expected)
    {
        var displayOption = new DisplayModeFallback
        {
            LargeScreenWidth = lgSize,
            LargeScreenCssClassPattern = lgPattern,
            MediumScreenWidth = mdSize,
            MediumScreenCssClassPattern = mdPattern,
            SmallScreenWidth = smSize,
            SmallScreenCssClassPattern = smPattern,
            ExtraSmallScreenWidth = xsSize,
            ExtraSmallScreenCssClassPattern = xsPattern
        };

        var result = AdvancedContentAreaRenderer.GetCssClassesForItem(displayOption);

        Assert.Equal(expected, result);
    }
}
