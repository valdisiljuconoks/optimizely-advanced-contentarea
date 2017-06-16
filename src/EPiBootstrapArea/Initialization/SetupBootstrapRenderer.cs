using System;
using System.Web.Mvc;
using EPiBootstrapArea.Providers;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc.Html;
using StructureMap;
using StructureMap.Pipeline;
using InitializationModule = EPiServer.Web.InitializationModule;

namespace EPiBootstrapArea.Initialization
{
    [ModuleDependency(typeof(InitializationModule))]
    [ModuleDependency(typeof(ServiceContainerInitialization))]
    [InitializableModule]
    public class SetupBootstrapRenderer : IConfigurableModule
    {
        private IContainer _container;

        void IConfigurableModule.ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Container.Configure(container =>
                                        {
                                            container.For<IDisplayModeFallbackProvider>().Use<DisplayModeFallbackDefaultProvider>();
                                            container.For<ContentAreaRenderer>().Use<BootstrapAwareContentAreaRenderer>();
                                            container.For<PropertyRenderer>().Use<CustomPropertyRenderer>();
                                        });

            _container = context.Container;
        }

        public void Initialize(InitializationEngine context)
        {
            context.InitComplete += ContextOnInitComplete;
        }

        public void Uninitialize(InitializationEngine context)
        {
            context.InitComplete -= ContextOnInitComplete;
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
                _container.Configure(ctx => ctx.For<ModelMetadataProvider>(Lifecycles.Singleton)
                                               .Use(() => new ModelMetadataProviderDecorator<DefaultDisplayOptionMetadataProvider>(currentProvider)));
            }
        }
    }
}
