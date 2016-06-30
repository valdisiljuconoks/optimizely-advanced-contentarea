using System.Collections.Generic;

namespace EPiBootstrapArea.Providers
{
    public class DisplayModeFallbackDefaultProvider : IDisplayModeFallbackProvider
    {
        public void Initialize()
        {
        }

        public virtual List<DisplayModeFallback> GetAll()
        {
            var initialData = new List<DisplayModeFallback>
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
                },
                new DisplayModeFallback
                {
                    Name = "Half width (1/2)",
                    Tag = ContentAreaTags.HalfWidth,
                    LargeScreenWidth = 6,
                    MediumScreenWidth = 6,
                    SmallScreenWidth = 12,
                    ExtraSmallScreenWidth = 12,
                    Icon = "epi-icon__layout--half"
                },
                new DisplayModeFallback
                {
                    Name = "One third width (1/3)",
                    Tag = ContentAreaTags.OneThirdWidth,
                    LargeScreenWidth = 4,
                    MediumScreenWidth = 6,
                    SmallScreenWidth = 12,
                    ExtraSmallScreenWidth = 12,
                    Icon = "epi-icon__layout--one-third"
                },
                new DisplayModeFallback
                {
                    Name = "Two thirds width (2/3)",
                    Tag = ContentAreaTags.TwoThirdsWidth,
                    LargeScreenWidth = 8,
                    MediumScreenWidth = 6,
                    SmallScreenWidth = 12,
                    ExtraSmallScreenWidth = 12,
                    Icon = "epi-icon__layout--two-thirds"
                },
                new DisplayModeFallback
                {
                    Name = "One quarter width (1/4)",
                    Tag = ContentAreaTags.OneQuarterWidth,
                    LargeScreenWidth = 3,
                    MediumScreenWidth = 6,
                    SmallScreenWidth = 12,
                    ExtraSmallScreenWidth = 12,
                    Icon = "epi-icon__layout--one-quarter"
                },
                new DisplayModeFallback
                {
                    Name = "Three quarters width (3/4)",
                    Tag = ContentAreaTags.ThreeQuartersWidth,
                    LargeScreenWidth = 9,
                    MediumScreenWidth = 6,
                    SmallScreenWidth = 12,
                    ExtraSmallScreenWidth = 12,
                    Icon = "epi-icon__layout--three-quarters"
                },
            };

            return initialData;
        }
    }
}
