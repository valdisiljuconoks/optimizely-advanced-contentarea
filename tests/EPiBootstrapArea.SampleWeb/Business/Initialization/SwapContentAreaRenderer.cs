using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc.Html;
using InitializationModule = EPiServer.Web.InitializationModule;

namespace EPiBootstrapArea.SampleWeb.Business.Initialization
{
    [ModuleDependency(typeof(InitializationModule))]
    public class SwapContentAreaRenderer : IConfigurableModule
    {
        public void Initialize(InitializationEngine context) { }

        public void Uninitialize(InitializationEngine context) { }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Services.Intercept<ContentAreaRenderer>((_, __) => new ContentAreaRenderer());
        }
    }
}
