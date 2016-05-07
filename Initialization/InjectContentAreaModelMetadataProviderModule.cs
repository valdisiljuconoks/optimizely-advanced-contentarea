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
            var currentModel = _container.GetInstance<ModelMetadataProvider>();
            _container.Configure(c => c.For<ModelMetadataProvider>()
                                       .Use<CompositeModelMetadataProvider>()
                                       .Ctor<ModelMetadataProvider>()
                                       .Is(currentModel));
        }
    }
}
