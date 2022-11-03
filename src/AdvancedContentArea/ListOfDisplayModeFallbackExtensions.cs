// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;

namespace TechFellow.Optimizely.AdvancedContentArea;

public static class ListOfDisplayModeFallbackExtensions
{
    public static List<DisplayModeFallback> Add<T>(this List<DisplayModeFallback> target) where T : DisplayModeFallback, new()
    {
        target.Add(new T());

        return target;
    }
}
