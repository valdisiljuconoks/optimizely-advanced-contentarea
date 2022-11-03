// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using EPiServer.Web.Mvc.Html;
using Microsoft.Extensions.DependencyInjection;

namespace TechFellow.Optimizely.AdvancedContentArea.Initialization;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddBootstrapAreaRenderer(
        this IServiceCollection services,
        Func<List<DisplayModeFallback>> displayModes)
    {
        services.AddTransient<ContentAreaRenderer>(_ => new BootstrapAwareContentAreaRenderer(displayModes()));

        return services;
    }
}
