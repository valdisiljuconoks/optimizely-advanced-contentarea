// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;

namespace TechFellow.Optimizely.AdvancedContentArea;

/// <summary>
/// Compares <see cref="DisplayModeFallback"/> instances by responsive column widths and tag.
/// </summary>
public class DisplayModeFallbackComparer : IEqualityComparer<DisplayModeFallback>
{
    /// <inheritdoc />
    public bool Equals(DisplayModeFallback x, DisplayModeFallback y)
    {
        return x.LargeScreenWidth == y.LargeScreenWidth
               && x.MediumScreenWidth == y.MediumScreenWidth
               && x.SmallScreenWidth == y.SmallScreenWidth
               && x.ExtraSmallScreenWidth == y.ExtraSmallScreenWidth
               && x.Tag == y.Tag;
    }

    /// <inheritdoc />
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
