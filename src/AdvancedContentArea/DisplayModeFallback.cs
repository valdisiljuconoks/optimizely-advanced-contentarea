// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.ComponentModel.DataAnnotations;

namespace TechFellow.Optimizely.AdvancedContentArea;

public class DisplayModeFallback
{
    public string Id { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string Tag { get; set; }

    [Required]
    [Range(1, 12)]
    public int ExtraExtraLargeScreenWidth { get; set; }

    public string ExtraExtraLargeScreenCssClassPattern { get; set; }

    [Required]
    [Range(1, 12)]
    public int ExtraLargeScreenWidth { get; set; }

    public string ExtraLargeScreenCssClassPattern { get; set; }

    [Required]
    [Range(1, 12)]
    public int LargeScreenWidth { get; set; }

    public string LargeScreenCssClassPattern { get; set; }

    [Required]
    [Range(1, 12)]
    public int MediumScreenWidth { get; set; }

    public string MediumScreenCssClassPattern { get; set; }

    [Required]
    [Range(1, 12)]
    public int SmallScreenWidth { get; set; }

    public string SmallScreenCssClassPattern { get; set; }

    [Required]
    [Range(1, 12)]
    public int ExtraSmallScreenWidth { get; set; }

    public string ExtraSmallScreenCssClassPattern { get; set; }

    public string Icon { get; set; }

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
