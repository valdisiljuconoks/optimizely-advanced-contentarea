// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;

namespace TechFellow.Optimizely.AdvancedContentArea;

internal static class StringExtensions
{
    internal static string TryFormat(this string target, params object[] args)
    {
        try
        {
            return string.Format(target, args);
        }
        catch (FormatException)
        {
            return null;
        }
    }
}
