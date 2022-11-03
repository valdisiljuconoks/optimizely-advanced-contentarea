// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;

namespace TechFellow.Optimizely.AdvancedContentArea;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DefaultDisplayOptionForTagAttribute : Attribute
{
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

    public string Tag { get; }

    public string DisplayOption { get; }
}
