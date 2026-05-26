// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using EPiServer.Core;
using HtmlAgilityPack;

namespace TechFellow.Optimizely.AdvancedContentArea.Initialization;

/// <summary>
/// Configuration options for <see cref="AdvancedContentAreaRenderer"/>.
/// </summary>
public class AdvancedContentAreaRendererOptions
{
    /// <summary>Display mode fallbacks available to the renderer.</summary>
    public IReadOnlyCollection<DisplayModeFallback> DisplayOptions { get; set; }

    /// <summary>When <see langword="true"/>, items are wrapped in Bootstrap rows based on column width.</summary>
    public bool RowSupportEnabled { get; set; }

    /// <summary>When <see langword="true"/>, row markup is added automatically during rendering.</summary>
    public bool AutoAddRow { get; set; }

    /// <summary>Optional callback invoked when the opening tag for a content area item is rendered.</summary>
    public Action<HtmlNode, ContentAreaItem, IContentData> ItemStartRenderCallback { get; set; }
}
