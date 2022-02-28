using System;
using System.Reflection;
using EPiServer.Core;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EPiBootstrapArea.Providers
{
    public class DefaultDisplayOptionMetadataProvider //: DataAnnotationsModelMetadataProvider
    {
        //    public override ModelMetadata GetMetadataForProperty(Func<object> modelAccessor, Type containerType, string propertyName)
        //    {
        //        var metadata = base.GetMetadataForProperty(modelAccessor, containerType, propertyName);

        //        var pi = containerType.GetProperty(propertyName);
        //        if (pi == null)
        //            return metadata;

        //        if (pi.PropertyType != typeof(ContentArea))
        //            return metadata;

        //        var attr = pi.GetCustomAttribute<DefaultDisplayOptionAttribute>();
        //        if(attr != null)
        //            metadata.AdditionalValues.Add($"{nameof(DefaultDisplayOptionMetadataProvider)}__DefaultDisplayOption", attr.DisplayOption);

        //        return metadata;
        //    }
    }
}
