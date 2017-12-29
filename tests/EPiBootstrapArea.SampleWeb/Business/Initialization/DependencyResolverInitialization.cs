using System.Web.Mvc;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiBootstrapArea.SampleWeb.Business.Rendering;
using EPiBootstrapArea.SampleWeb.Helpers;
using EPiServer.Web.Mvc;
using EPiServer.Web.Mvc.Html;
using StructureMap;

namespace EPiBootstrapArea.SampleWeb.Business.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class DependencyResolverInitialization : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.StructureMap().Configure(ConfigureContainer);

            DependencyResolver.SetResolver(new StructureMapDependencyResolver(context.StructureMap()));
        }

        private static void ConfigureContainer(ConfigurationExpression container)
        {
            //Swap out the default ContentRenderer for our custom
            container.For<IContentRenderer>().Use<ErrorHandlingContentRenderer>();

            //Implementations for custom interfaces can be registered here.
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
