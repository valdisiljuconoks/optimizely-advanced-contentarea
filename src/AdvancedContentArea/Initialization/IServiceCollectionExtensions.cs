// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Linq;
using EPiServer.Web;
using EPiServer.Web.Mvc.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace TechFellow.Optimizely.AdvancedContentArea.Initialization;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddAdvancedContentArea(
        this IServiceCollection services,
        Action<AdvancedContentAreaRendererOptions> configure = null)
    {
        var options = new AdvancedContentAreaRendererOptions();

        if (configure != null)
        {
            configure(options);
        }

        services.AddTransient<ContentAreaRenderer, AdvancedContentAreaRenderer>();
        services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, ConfigureModelMetadataDetailsProviders>());
        //context.Services.AddSingleton<PropertyRenderer, CustomPropertyRenderer>();

        if (options.DisplayOptions?.Any() ?? false)
        {
            services.Configure<DisplayOptions>(displayOption =>
            {
                foreach (var option in options.DisplayOptions)
                {
                    displayOption.Add(option.Id, option.Name, option.Tag, "", option.Icon);
                }
            });

            services.AddSingleton(_ => options.DisplayOptions);
        }

        services.AddSingleton(options);

        return services;
    }
}
