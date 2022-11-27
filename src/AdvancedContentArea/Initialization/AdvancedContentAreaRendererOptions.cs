// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using EPiServer.Core;
using HtmlAgilityPack;

namespace TechFellow.Optimizely.AdvancedContentArea.Initialization;

public class AdvancedContentAreaRendererOptions
{
    public IReadOnlyCollection<DisplayModeFallback> DisplayOptions { get; set; }

    public bool RowSupportEnabled { get; set; }

    public bool AutoAddRow { get; set; }

    public Action<HtmlNode, ContentAreaItem, IContent> ItemStartRenderCallback { get; set; }
}
