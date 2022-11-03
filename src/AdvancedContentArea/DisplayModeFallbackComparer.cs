// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;

namespace TechFellow.Optimizely.AdvancedContentArea;

public class DisplayModeFallbackComparer : IEqualityComparer<DisplayModeFallback>
{
    public bool Equals(DisplayModeFallback x, DisplayModeFallback y)
    {
        return x.LargeScreenWidth == y.LargeScreenWidth
               && x.MediumScreenWidth == y.MediumScreenWidth
               && x.SmallScreenWidth == y.SmallScreenWidth
               && x.ExtraSmallScreenWidth == y.ExtraSmallScreenWidth
               && x.Tag == y.Tag;
    }

    public int GetHashCode(DisplayModeFallback obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        return obj.LargeScreenWidth.GetHashCode()
               ^ obj.MediumScreenWidth.GetHashCode()
               ^ obj.SmallScreenWidth.GetHashCode()
               ^ obj.ExtraSmallScreenWidth.GetHashCode()
               ^ obj.Tag.GetHashCode();
    }
}
