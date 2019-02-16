using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EPiBootstrapArea.Providers;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Mvc.Html;
using InitializationModule = EPiServer.Web.InitializationModule;

namespace EPiBootstrapArea.Initialization
{
    [ModuleDependency(typeof(InitializationModule))]
    [ModuleDependency(typeof(ServiceContainerInitialization))]
    [InitializableModule]
    public class SetupBootstrapRenderer : IConfigurableModule
    {
        private ServiceConfigurationContext _context;

        void IConfigurableModule.ConfigureContainer(ServiceConfigurationContext context)
        {
            _context = context;

            context.Services.AddSingleton<IDisplayModeFallbackProvider, DisplayModeFallbackDefaultProvider>();
            context.Services.AddSingleton<PropertyRenderer, CustomPropertyRenderer>();
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
            var allDisplayOptions = GetAllDisplayOptions();
            RegisterDisplayOptions(allDisplayOptions);

            // setup proper renderer with all registered fallback (+custom ones as well)
            _context.Services.AddTransient<ContentAreaRenderer>(_ => new BootstrapAwareContentAreaRenderer(allDisplayOptions));

            // setup model metadata provider - to supply proper index inside content area item (while rendering)
            if (_context.Services.Contains(typeof(ModelMetadataProvider)))
            {
                var currentProvider = ServiceLocator.Current.GetInstance<ModelMetadataProvider>();
                _context.Services.AddSingleton<ModelMetadataProvider>(new ModelMetadataProviderDecorator<DefaultDisplayOptionMetadataProvider>(currentProvider));
            }
            else
                _context.Services.AddSingleton<ModelMetadataProvider, DefaultDisplayOptionMetadataProvider>();
        }

        private List<DisplayModeFallback> GetAllDisplayOptions()
        {
            var builtInOptions = new List<DisplayModeFallback>();

            var provider = ServiceLocator.Current.GetInstance<IDisplayModeFallbackProvider>();
            if(provider != null)
            {
                provider.Initialize();
                builtInOptions = provider.GetAll();
            }

            var customModes = ConfigurationContext.Current.CustomDisplayOptions;

            return ConfigurationContext.Current.DisableBuiltinDisplayOptions
                ? customModes
                : builtInOptions.Union(customModes, new DisplayModeFallbackComparer()).ToList();
        }

        private void RegisterDisplayOptions(List<DisplayModeFallback> listOfFallback)
        {
            listOfFallback.ForEach(AddDisplayOption);
        }

        private static void AddDisplayOption(DisplayModeFallback mode)
        {
            var options = ServiceLocator.Current.GetInstance<DisplayOptions>();
            var localizationService = ServiceLocator.Current.GetInstance<LocalizationService>();
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
