// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using EPiServer;
using EPiServer.Core;

namespace TechFellow.Optimizely.AdvancedContentArea;

/// <summary>
/// Extension methods for <see cref="IContentData"/> used during content area rendering.
/// </summary>
public static class ContentExtensions
{
    /// <summary>
    /// Builds a stable bookmark name for in-page navigation anchors.
    /// </summary>
    /// <param name="content">The content instance.</param>
    /// <returns>
    /// A lowercase type name, optionally suffixed with the content link identifier when the instance implements <see cref="IContent"/>.
    /// </returns>
    public static string GetContentBookmarkName(this IContentData content)
    {
        if (content is IContent iContent)
        {
            return iContent.GetOriginalType().Name.ToLowerInvariant()
                + "_"
                + iContent.ContentLink;

        }
        return content.GetOriginalType().Name.ToLowerInvariant();
    }
}
