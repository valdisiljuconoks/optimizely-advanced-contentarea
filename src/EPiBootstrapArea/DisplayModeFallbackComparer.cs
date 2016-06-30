using System;
using System.Collections.Generic;

namespace EPiBootstrapArea
{
    public class DisplayModeFallbackComparer : IEqualityComparer<DisplayModeFallback>
    {
        public bool Equals(DisplayModeFallback x, DisplayModeFallback y)
        {
            return x.LargeScreenWidth == y.LargeScreenWidth &&
                   x.MediumScreenWidth == y.MediumScreenWidth &&
                   x.SmallScreenWidth == y.SmallScreenWidth &&
                   x.ExtraSmallScreenWidth == y.ExtraSmallScreenWidth &&
                   x.Tag == y.Tag;
        }

        public int GetHashCode(DisplayModeFallback obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            return obj.GetHashCode();
        }
    }
}
