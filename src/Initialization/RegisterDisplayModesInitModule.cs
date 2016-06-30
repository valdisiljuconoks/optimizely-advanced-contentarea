using EPiServer.Data;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;
using EPiServer.Web;

namespace EPiBootstrapArea.Initialization
{
    [ModuleDependency(typeof(DataInitialization))]
    [ModuleDependency(typeof(DisplayModeFallbackProviderInitModule))]
    [ModuleDependency(typeof(ProviderBasedLocalizationService))]
    public class RegisterDisplayModesInitModule : IInitializableModule
    {
        private IDisplayModeFallbackProvider _provider;

        public void Initialize(InitializationEngine context)
        {
            _provider = ServiceLocator.Current.GetInstance<IDisplayModeFallbackProvider>();
            _provider?.Initialize();

            RegisterDisplayOptions();
        }

        public void Uninitialize(InitializationEngine context) { }

        private void RegisterDisplayOptions()
        {
            var options = ServiceLocator.Current.GetInstance<DisplayOptions>();
            var localizationService = ServiceLocator.Current.GetInstance<LocalizationService>();
            var modes = _provider.GetAll();

            foreach (var mode in modes)
            {
                var name = "/displayoptions/" + mode.Tag;
                string translatedName;

                try
                {
                    translatedName = !localizationService.TryGetString(name, out translatedName) ? mode.Name : name;
                }
                catch
                {
                    translatedName = mode.Name;
                }

                options.Add(new DisplayOption
                            {
                                Id = mode.Tag,
                                Name = translatedName,
                                Tag = mode.Tag,
                                IconClass = mode.Icon
                            });
            }
        }
    }
}
