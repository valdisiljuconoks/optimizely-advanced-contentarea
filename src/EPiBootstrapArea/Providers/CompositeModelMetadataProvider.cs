using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EPiBootstrapArea.Providers
{
    // if developer has specified default display option for content area
    // this class will make sure that this data is injected into model's metadata additionaldata field for ContentArea models
    // so then - when rendering content area - renderer will have access to default option set by attribute
    public class CompositeModelMetadataProvider<TProvider> : ModelMetadataProvider where TProvider : ModelMetadataProvider, new()
    {
        private readonly ModelMetadataProvider _innerProvider;
        private readonly TProvider _wrappedProvider;

        public CompositeModelMetadataProvider(ModelMetadataProvider innerProvider)
        {
            if(innerProvider == null)
                throw new ArgumentNullException(nameof(innerProvider));

            _innerProvider = innerProvider;
            _wrappedProvider = new TProvider();
        }

        public override IEnumerable<ModelMetadata> GetMetadataForProperties(object container, Type containerType)
        {
            return _innerProvider.GetMetadataForProperties(container, containerType);
        }

        public override ModelMetadata GetMetadataForProperty(Func<object> modelAccessor, Type containerType, string propertyName)
        {
            var metadata = _innerProvider.GetMetadataForProperty(modelAccessor, containerType, propertyName);

            var additionalMetadata = _wrappedProvider.GetMetadataForProperty(modelAccessor, containerType, propertyName);
            MergeAdditionalValues(metadata.AdditionalValues, additionalMetadata.AdditionalValues);

            return metadata;
        }

        private void MergeAdditionalValues(IDictionary<string, object> target, Dictionary<string, object> source)
        {
            foreach (var key in source.Keys)
            {
                if(!target.ContainsKey(key))
                {
                    target.Add(key, source[key]);
                }
            }
        }

        public override ModelMetadata GetMetadataForType(Func<object> modelAccessor, Type modelType)
        {
            return _innerProvider.GetMetadataForType(modelAccessor, modelType);
        }
    }
}
