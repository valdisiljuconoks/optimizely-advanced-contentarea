// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using Microsoft.AspNetCore.Routing;

namespace TechFellow.Optimizely.AdvancedContentArea.Extensions;

internal static class RouteDictionaryExtensions
{
    internal static bool? GetFlagValue(this RouteValueDictionary additionalValues, string key)
    {
        return additionalValues.GetValueFromDictionary(key);
    }
}
