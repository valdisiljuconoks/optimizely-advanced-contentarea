// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TechFellow.Optimizely.AdvancedContentArea.Providers;

namespace TechFellow.Optimizely.AdvancedContentArea.Initialization;

public class ConfigureModelMetadataDetailsProviders : IConfigureOptions<MvcOptions>
{
    public void Configure(MvcOptions options)
    {
        options.ModelMetadataDetailsProviders.Add(new DefaultDisplayOptionMetadataProvider());
    }
}
