// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;

namespace TechFellow.Optimizely.AdvancedContentArea;

public class DefaultDisplayOptionAttribute : Attribute
{
    public DefaultDisplayOptionAttribute(string displayOption)
    {
        if (string.IsNullOrWhiteSpace(displayOption))
        {
            throw new ArgumentNullException(nameof(displayOption));
        }

        DisplayOption = displayOption;
    }

    public string DisplayOption { get; }
}
