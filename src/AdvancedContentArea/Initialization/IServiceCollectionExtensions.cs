using System;
using System.Collections.Generic;
using EPiServer.Web.Mvc.Html;
using Microsoft.Extensions.DependencyInjection;

namespace TechFellow.Optimizely.AdvancedContentArea.Initialization
{
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
}
