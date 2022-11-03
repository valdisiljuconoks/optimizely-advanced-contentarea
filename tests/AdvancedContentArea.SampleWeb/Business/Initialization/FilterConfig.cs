using System.Web.Mvc;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using InitializationModule = EPiServer.Web.InitializationModule;

namespace EPiBootstrapArea.SampleWeb.Business.Initialization
{
    /// <summary>
    ///     Module for registering filters which will be applied to controller actions.
    /// </summary>
    [ModuleDependency(typeof(InitializationModule))]
    public class FilterConfig : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            GlobalFilters.Filters.Add(ServiceLocator.Current.GetInstance<PageContextActionFilter>());
        }

        public void Uninitialize(InitializationEngine context) { }

        public void Preload(string[] parameters) { }
    }
}
