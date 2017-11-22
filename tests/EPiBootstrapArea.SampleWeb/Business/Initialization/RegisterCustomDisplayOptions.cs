using EPiBootstrapArea.Initialization;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace EPiBootstrapArea.SampleWeb.Business.Initialization
{
    [ModuleDependency(typeof(SetupBootstrapRenderer))]
    public class RegisterCustomDisplayOptions : IConfigurableModule
    {
        public void Initialize(InitializationEngine context)
        {
            //            ConfigurationContext.Setup(ctx => { ctx.DisableBuiltinDisplayOptions = false; });
        }

        public void Uninitialize(InitializationEngine context) { }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.StructureMap().Configure(x => x.For<IDisplayModeFallbackProvider>()
                                              .Use<CustomDisplayModeFallbackDefaultProvider>());
        }
    }
}
