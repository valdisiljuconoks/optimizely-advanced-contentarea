// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;

namespace TechFellow.Optimizely.AdvancedContentArea;

/// <summary>
/// Sets the default display option for a block type when rendered in a content area with a specific tag.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DefaultDisplayOptionForTagAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultDisplayOptionForTagAttribute"/> class.
    /// </summary>
    /// <param name="tag">The content area tag to match.</param>
    /// <param name="displayOption">The display option identifier to apply.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="tag"/> or <paramref name="displayOption"/> is null or whitespace.</exception>
    public DefaultDisplayOptionForTagAttribute(string tag, string displayOption)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            throw new ArgumentNullException(nameof(tag));
        }

        if (string.IsNullOrWhiteSpace(displayOption))
        {
            throw new ArgumentNullException(nameof(displayOption));
        }

        Tag = tag;
        DisplayOption = displayOption;
    }

    /// <summary>The content area tag this default applies to.</summary>
    public string Tag { get; }

    /// <summary>The default display option identifier for the given tag.</summary>
    public string DisplayOption { get; }
}
