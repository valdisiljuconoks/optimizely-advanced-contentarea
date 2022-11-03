// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer;
using EPiServer.Core;

namespace TechFellow.Optimizely.AdvancedContentArea;

public static class ContentExtensions
{
    public static string GetContentBookmarkName(this IContent content)
    {
        return content.GetOriginalType().Name.ToLowerInvariant()
               + "_"
               + content.ContentLink;
    }
}
