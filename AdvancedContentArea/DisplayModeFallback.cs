// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.ComponentModel.DataAnnotations;

namespace TechFellow.Optimizely.AdvancedContentArea;

/// <summary>
/// Describes a Bootstrap-based display mode with responsive column widths and optional CSS class patterns.
/// </summary>
public class DisplayModeFallback
{
    /// <summary>Unique identifier for the display mode.</summary>
    public string Id { get; set; }

    /// <summary>Human-readable name shown in the CMS UI.</summary>
    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; }

    /// <summary>Tag value stored on content area items for this display mode.</summary>
    [Required(AllowEmptyStrings = false)]
    public string Tag { get; set; }

    /// <summary>Bootstrap column width (1–12) for extra-extra-large breakpoints.</summary>
    [Required]
    [Range(1, 12)]
    public int ExtraExtraLargeScreenWidth { get; set; }

    /// <summary>Optional CSS class pattern for extra-extra-large breakpoints.</summary>
    public string ExtraExtraLargeScreenCssClassPattern { get; set; }

    /// <summary>Bootstrap column width (1–12) for extra-large breakpoints.</summary>
    [Required]
    [Range(1, 12)]
    public int ExtraLargeScreenWidth { get; set; }

    /// <summary>Optional CSS class pattern for extra-large breakpoints.</summary>
    public string ExtraLargeScreenCssClassPattern { get; set; }

    /// <summary>Bootstrap column width (1–12) for large breakpoints.</summary>
    [Required]
    [Range(1, 12)]
    public int LargeScreenWidth { get; set; }

    /// <summary>Optional CSS class pattern for large breakpoints.</summary>
    public string LargeScreenCssClassPattern { get; set; }

    /// <summary>Bootstrap column width (1–12) for medium breakpoints.</summary>
    [Required]
    [Range(1, 12)]
    public int MediumScreenWidth { get; set; }

    /// <summary>Optional CSS class pattern for medium breakpoints.</summary>
    public string MediumScreenCssClassPattern { get; set; }

    /// <summary>Bootstrap column width (1–12) for small breakpoints.</summary>
    [Required]
    [Range(1, 12)]
    public int SmallScreenWidth { get; set; }

    /// <summary>Optional CSS class pattern for small breakpoints.</summary>
    public string SmallScreenCssClassPattern { get; set; }

    /// <summary>Bootstrap column width (1–12) for extra-small breakpoints.</summary>
    [Required]
    [Range(1, 12)]
    public int ExtraSmallScreenWidth { get; set; }

    /// <summary>Optional CSS class pattern for extra-small breakpoints.</summary>
    public string ExtraSmallScreenCssClassPattern { get; set; }

    /// <summary>CMS icon CSS class for this display mode.</summary>
    public string Icon { get; set; }

    /// <summary>
    /// A display mode that applies no layout (zero column width at all breakpoints).
    /// </summary>
    public static DisplayModeFallback None =>
        new()
        {
            Id = "none",
            Name = "None",
            Tag = ContentAreaTags.None,
            ExtraExtraLargeScreenWidth = 0,
            ExtraLargeScreenWidth = 0,
            LargeScreenWidth = 0,
            MediumScreenWidth = 0,
            SmallScreenWidth = 0,
            ExtraSmallScreenWidth = 0,
        };
}
