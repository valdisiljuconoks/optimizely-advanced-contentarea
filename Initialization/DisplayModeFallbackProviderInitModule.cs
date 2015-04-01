using EPiBootstrapArea.Providers;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace EPiBootstrapArea.Initialization
{
    [ModuleDependency(typeof(ServiceContainerInitialization))]
    [InitializableModule]
    public class DisplayModeFallbackProviderInitModule : IConfigurableModule
    {
        void IConfigurableModule.ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Container.Configure(x => x.For<IDisplayModeFallbackProvider>().Use<DisplayModeFallbackDefaultProvider>());
        }

        public void Initialize(InitializationEngine context)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void Preload(string[] parameters)
        {
        }
    }
}
