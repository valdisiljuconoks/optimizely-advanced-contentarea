using System;
using System.Web.Mvc;
using EPiBootstrapArea.Providers;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using StructureMap;
using InitializationModule = EPiServer.Web.InitializationModule;

namespace EPiBootstrapArea.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(InitializationModule))]
    public class InjectContentAreaModelMetadataProviderModule : IConfigurableModule
    {
        private IContainer _container;

        public void Initialize(InitializationEngine context)
        {
            context.InitComplete += ContextOnInitComplete;
        }

        public void Uninitialize(InitializationEngine context) { }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            _container = context.Container;
        }

        private void ContextOnInitComplete(object sender, EventArgs eventArgs)
        {
            var currentProvider = _container.TryGetInstance<ModelMetadataProvider>();

            if(currentProvider == null)
            {
                _container.Configure(ctx => ctx.For<ModelMetadataProvider>()
                                               .Use<DefaultDisplayOptionMetadataProvider>());
            }
            else
            {
                // decorate existing provider
                _container.Configure(ctx => ctx.For<ModelMetadataProvider>()
                                               .DecorateAllWith<CompositeModelMetadataProvider<DefaultDisplayOptionMetadataProvider>>());
            }
        }
    }
}
