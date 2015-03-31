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
            context.Container.Configure(x => x.For<IDisplayModeFallbackProvider>().Use<DisplayModeFallbackProvider>());
        }

        void IInitializableModule.Initialize(InitializationEngine context)
        {
        }

        public void Preload(string[] parameters)
        {
        }

        void IInitializableModule.Uninitialize(InitializationEngine context)
        {
        }
    }
}
