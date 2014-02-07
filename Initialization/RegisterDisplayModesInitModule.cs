using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using InitializationModule = EPiServer.Web.InitializationModule;

namespace EPiBootstrapArea.Initialization
{
    [ModuleDependency(typeof(InitializationModule))]
    public class RegisterDisplayModesInitModule : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            var options = ServiceLocator.Current.GetInstance<DisplayOptions>();
            options
                    .Add(ContentAreaTags.FullWidth, "/displayoptions/full", ContentAreaTags.FullWidth, "", "epi-icon__layout--full")
                    .Add(ContentAreaTags.HalfWidth, "/displayoptions/half", ContentAreaTags.HalfWidth, "", "epi-icon__layout--half")
                    .Add(ContentAreaTags.OneThirdWidth, "/displayoptions/one-third", ContentAreaTags.OneThirdWidth, "", "epi-icon__layout--one-third")
                    .Add(ContentAreaTags.TwoThirdsWidth, "/displayoptions/two-thirds", ContentAreaTags.TwoThirdsWidth, "", "epi-icon__layout--two-thirds")
                    .Add(ContentAreaTags.OneQuarterWidth, "/displayoptions/one-quarter", ContentAreaTags.OneQuarterWidth, "", "epi-icon__layout--one-quarter")
                    .Add(ContentAreaTags.ThreeQuartersWidth, "/displayoptions/three-quarters", ContentAreaTags.ThreeQuartersWidth, "", "epi-icon__layout--three-quarters");
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void Preload(string[] parameters)
        {
        }
    }
}
