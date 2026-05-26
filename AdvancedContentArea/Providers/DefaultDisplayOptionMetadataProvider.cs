using System.Linq;
using System.Reflection;
using EPiServer.Core;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace TechFellow.Optimizely.AdvancedContentArea.Providers;

/// <summary>
/// Copies <see cref="DefaultDisplayOptionAttribute"/> values into model metadata for <see cref="ContentArea"/> properties.
/// </summary>
public class DefaultDisplayOptionMetadataProvider : IDisplayMetadataProvider
{
    /// <inheritdoc />
    public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
    {
        var modelMetadata = context.DisplayMetadata;
        var pi = context.Key.PropertyInfo;

        if (pi == null)
        {
            return;
        }

        if (pi.PropertyType != typeof(ContentArea))
        {
            return;
        }

        var attr = pi.GetCustomAttribute<DefaultDisplayOptionAttribute>();
        if (attr != null)
        {
            modelMetadata.AdditionalValues.Add($"{nameof(DefaultDisplayOptionMetadataProvider)}__DefaultDisplayOption",
                                               attr.DisplayOption);
        }
    }
}
