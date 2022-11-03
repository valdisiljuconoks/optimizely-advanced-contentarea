using System;
using System.Collections.Generic;
using EPiBootstrapArea;
using EPiServer.Web.Mvc.Html;

namespace Microsoft.Extensions.DependencyInjection
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
