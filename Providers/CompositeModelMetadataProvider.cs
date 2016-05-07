using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using EPiServer.Core;

namespace EPiBootstrapArea.Providers
{
    // if developer has specified default display option for content area
    // this class will make sure that this data is injected into model's metadata additionaldata field for ContentArea models
    public class CompositeModelMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        private readonly ModelMetadataProvider _inner;

        public CompositeModelMetadataProvider(ModelMetadataProvider inner)
        {
            _inner = inner;
        }

        public override IEnumerable<ModelMetadata> GetMetadataForProperties(object container, Type containerType)
        {
            return _inner == null
                       ? base.GetMetadataForProperties(container, containerType)
                       : _inner.GetMetadataForProperties(container, containerType);
        }

        public override ModelMetadata GetMetadataForProperty(Func<object> modelAccessor, Type containerType, string propertyName)
        {
            // if bootstrap model metadata provider was initialized with null reference _inner provider
            // then we just call data annotation based model to create metadata object
            // and then inject additional values into that metadata model
            var metadata = _inner == null
                               ? base.GetMetadataForProperty(modelAccessor, containerType, propertyName)
                               : _inner.GetMetadataForProperty(modelAccessor, containerType, propertyName);

            var pi = containerType.GetProperty(propertyName);
            if(pi.PropertyType != typeof(ContentArea))
            {
                return metadata;
            }

            var attr = pi.GetCustomAttribute<DefaultDisplayOptionAttribute>();
            if(attr != null)
            {
                metadata.AdditionalValues.Add($"{nameof(CompositeModelMetadataProvider)}__DefaultDisplayOption", attr.DisplayOption);
            }

            return metadata;
        }

        public override ModelMetadata GetMetadataForType(Func<object> modelAccessor, Type modelType)
        {
            return _inner == null
                       ? base.GetMetadataForType(modelAccessor, modelType)
                       : _inner.GetMetadataForType(modelAccessor, modelType);
        }
    }
}
