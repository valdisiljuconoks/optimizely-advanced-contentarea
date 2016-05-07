using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc.Html;

namespace EPiBootstrapArea.Initialization
{
    [ModuleDependency(typeof(ServiceContainerInitialization))]
    [InitializableModule]
    public class SwapRendererInitModule : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Container.Configure(container => container.For<ContentAreaRenderer>().Use<BootstrapAwareContentAreaRenderer>());
        }

        public void Initialize(InitializationEngine context) { }

        public void Uninitialize(InitializationEngine context) { }
    }
}
