// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;

namespace TechFellow.Optimizely.AdvancedContentArea;

public class ConfigurationContext
{
    public bool RowSupportEnabled { get; set; }

    public bool AutoAddRow { get; set; }

    public bool DisableBuiltinDisplayOptions { get; set; }

    public List<DisplayModeFallback> CustomDisplayOptions { get; } = new();

    public static ConfigurationContext Current { get; } = new();

    public static void Setup(Action<ConfigurationContext> configCallback)
    {
        configCallback?.Invoke(Current);
    }
}
