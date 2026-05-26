// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;

namespace TechFellow.Optimizely.AdvancedContentArea;

/// <summary>
/// Sets the default display option for a block type when placed in a content area.
/// </summary>
public class DefaultDisplayOptionAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultDisplayOptionAttribute"/> class.
    /// </summary>
    /// <param name="displayOption">The display option identifier.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="displayOption"/> is null or whitespace.</exception>
    public DefaultDisplayOptionAttribute(string displayOption)
    {
        if (string.IsNullOrWhiteSpace(displayOption))
        {
            throw new ArgumentNullException(nameof(displayOption));
        }

        DisplayOption = displayOption;
    }

    /// <summary>The default display option identifier.</summary>
    public string DisplayOption { get; }
}
