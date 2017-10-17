using System.Collections.Generic;
using System.Linq;
using EPiBootstrapArea.Providers;

namespace EPiBootstrapArea.SampleWeb.Business.Initialization
{
    public class CustomDisplayModeFallbackDefaultProvider : DisplayModeFallbackDefaultProvider
    {
        public override List<DisplayModeFallback> GetAll()
        {
            return base.GetAll().Union(new[]
                                       {
                                           new DisplayModeFallback
                                           {
                                               Name = "One 12th (1/12)",
                                               Tag = "displaymode-one-twelfth",
                                               LargeScreenWidth = 1,
                                               MediumScreenWidth = 1,
                                               SmallScreenWidth = 1,
                                               ExtraSmallScreenWidth = 1
                                           }
                                       }).ToList();
        }
    }
}
