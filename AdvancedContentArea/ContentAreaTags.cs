// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

namespace TechFellow.Optimizely.AdvancedContentArea;

/// <summary>
/// Well-known CSS tag names used by display modes in content areas.
/// </summary>
public static class ContentAreaTags
{
    /// <summary>Display mode tag for full-width (12-column) layout.</summary>
    public const string FullWidth = "displaymode-full";

    /// <summary>Display mode tag for half-width (6-column) layout.</summary>
    public const string HalfWidth = "displaymode-half";

    /// <summary>Display mode tag indicating the item should not be rendered.</summary>
    public const string NoRenderer = "displaymode-norenderer";

    /// <summary>Display mode tag for items with no display mode applied.</summary>
    public const string None = "displaymode-none";

    /// <summary>Display mode tag for one-quarter width (3-column) layout.</summary>
    public const string OneQuarterWidth = "displaymode-one-quarter";

    /// <summary>Display mode tag for one-third width (4-column) layout.</summary>
    public const string OneThirdWidth = "displaymode-one-third";

    /// <summary>Display mode tag for three-quarters width (9-column) layout.</summary>
    public const string ThreeQuartersWidth = "displaymode-three-quarters";

    /// <summary>Display mode tag for two-thirds width (8-column) layout.</summary>
    public const string TwoThirdsWidth = "displaymode-two-thirds";
}
