// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using EPiServer.Core;
using EPiServer.Web;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TechFellow.Optimizely.AdvancedContentArea;

/// <summary>
/// View helpers for reading display option context during content area rendering.
/// </summary>
public static class IHtmlHelperExtensions
{
    internal static bool? GetFlagValueFromViewData(this IHtmlHelper htmlHelper, string key)
    {
        return htmlHelper.ViewContext.ViewData.GetValueFromDictionary(key);
    }

    internal static string GetValueFromViewData(this IHtmlHelper htmlHelper, string key)
    {
        return htmlHelper.ViewContext.ViewData.GetValueFromDictionary<string>(key);
    }

    /// <summary>
    /// Gets the display option currently stored in view data for the block being rendered.
    /// </summary>
    /// <param name="htmlHelper">The HTML helper.</param>
    /// <param name="block">The block being rendered.</param>
    /// <returns>The active <see cref="DisplayOption"/>, or <see langword="null"/> if none is set.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="htmlHelper"/> or <paramref name="block"/> is null.</exception>
    public static DisplayOption GetDisplayOption(this IHtmlHelper htmlHelper, BlockData block)
    {
        if (htmlHelper == null)
        {
            throw new ArgumentNullException(nameof(htmlHelper));
        }

        if (block == null)
        {
            throw new ArgumentNullException(nameof(block));
        }

        return htmlHelper.ViewContext.ViewData.ContainsKey(Constants.CurrentDisplayOptionKey)
            ? htmlHelper.ViewContext.ViewData[Constants.CurrentDisplayOptionKey] as DisplayOption
            : null;
    }

    /// <summary>
    /// Gets the zero-based index of the current block within the content area render pass.
    /// </summary>
    /// <param name="htmlHelper">The HTML helper.</param>
    /// <returns>The block index, or -1 when not set in view data.</returns>
    public static int BlockIndex(this IHtmlHelper htmlHelper)
    {
        return (int?)htmlHelper.ViewData[Constants.BlockIndexViewDataKey] ?? -1;
    }
}
