using System.Web.Mvc;
using EPiBootstrapArea.SampleWeb.Business.Rendering;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using InitializationModule = EPiServer.Web.InitializationModule;

namespace EPiBootstrapArea.SampleWeb.Business.Initialization
{
    /// <summary>
    ///     Module for customizing templates and rendering.
    /// </summary>
    [ModuleDependency(typeof(InitializationModule))]
    public class CustomizedRenderingInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            //Add custom view engine allowing partials to be placed in additional locations
            //Note that we add it first in the list to optimize view resolving when using DisplayFor/PropertyFor
            ViewEngines.Engines.Insert(0, new SiteViewEngine());

            context.Locate.TemplateResolver()
                .TemplateResolved += TemplateCoordinator.OnTemplateResolved;

            ConfigurationContext.Setup(ctx =>
            {
                ctx.RowSupportEnabled = true;
                ctx.AutoAddRow = false;

                ctx.DisableBuiltinDisplayOptions = false;
                ctx.CustomDisplayOptions
                    .Add<DisplayModeFallback.None>()
                    .Add<One12thDisplayOption>()
                    .Add<One6thDisplayOption>()
                    .Add(new DisplayModeFallback
                    {
                        Name = "Full width (1/1)",
                        Tag = ContentAreaTags.FullWidth,
                        LargeScreenWidth = 12,
                        MediumScreenWidth = 12,
                        SmallScreenWidth = 12,
                        ExtraSmallScreenWidth = 12,
                        Icon = "epi-icon__layout--full"
                    });
            });
        }

        public void Uninitialize(InitializationEngine context)
        {
            ServiceLocator
                .Current
                .GetInstance<TemplateResolver>()
                .TemplateResolved -= TemplateCoordinator.OnTemplateResolved;
        }
    }

    public class One12thDisplayOption : DisplayModeFallback
    {
        public One12thDisplayOption()
        {
            Name = "One 12th (1/12)";
            Tag = "displaymode-one-twelfth";
            LargeScreenWidth = 1;
            MediumScreenWidth = 1;
            SmallScreenWidth = 1;
            ExtraSmallScreenWidth = 1;
        }
    }

    public class One6thDisplayOption : DisplayModeFallback
    {
        public One6thDisplayOption()
        {
            Name = "One 6th (1/6)";
            Tag = "displaymode-one-sixth";
            LargeScreenWidth = 2;
            MediumScreenWidth = 2;
            SmallScreenWidth = 2;
            ExtraSmallScreenWidth = 2;
        }
    }
}
