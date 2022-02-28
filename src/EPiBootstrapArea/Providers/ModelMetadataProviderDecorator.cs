using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EPiBootstrapArea.Providers
{
    /// <summary>
    /// if developer has specified default display option for content area
    /// this class will make sure that this data is injected into model's metadata additional data field for ContentArea models
    /// so then - when rendering content area - renderer will have access to default option set by attribute
    /// </summary>
    /// <typeparam name="TProvider"></typeparam>
    internal class ModelMetadataProviderDecorator<TProvider> : ModelMetadataProvider where TProvider : ModelMetadataProvider, new()
    {
        private readonly ModelMetadataProvider _innerProvider;
        private readonly TProvider _wrappedProvider;

        public ModelMetadataProviderDecorator(ModelMetadataProvider innerProvider)
        {
            _innerProvider = innerProvider ?? throw new ArgumentNullException(nameof(innerProvider));
            _wrappedProvider = new TProvider();
        }

        public override IEnumerable<ModelMetadata> GetMetadataForProperties(Type modelType)
        {
            return _innerProvider.GetMetadataForProperties(modelType);
        }

        public override ModelMetadata GetMetadataForType(Type modelType)
        {
            return _innerProvider.GetMetadataForType(modelType);
        }

        public override ModelMetadata GetMetadataForParameter(ParameterInfo parameter)
        {
            var metadata = _innerProvider.GetMetadataForParameter(parameter);
            var additionalMetadata = _wrappedProvider.GetMetadataForParameter(parameter);

            MergeAdditionalValues(metadata.AdditionalValues, additionalMetadata.AdditionalValues);

            return metadata;
        }

        private void MergeAdditionalValues(IReadOnlyDictionary<object, object> target, IReadOnlyDictionary<object, object> source)
        {
            //foreach (var key in source.Keys)
            //{
            //    if(!target.ContainsKey(key))
            //    {
            //        target.Add(key, source[key]);
            //    }
            //}
        }
    }
}
