using AlloySampleSite.Models.Blocks;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.SpecializedProperties;
using System.ComponentModel.DataAnnotations;
using TechFellow.Optimizely.AdvancedContentArea;

namespace AlloySampleSite.Models.Pages
{
    /// <summary>
    /// Used for the site's start page and also acts as a container for site settings
    /// </summary>
    [ContentType(
        GUID = "19671657-B684-4D95-A61F-8DD4FE60D559",
        GroupName = Global.GroupNames.Specialized)]
    [SiteImageUrl]
    [AvailableContentTypes(
        Availability.Specific,
        Include = new[] { typeof(ContainerPage), typeof(ProductPage), typeof(StandardPage), typeof(ISearchPage), typeof(LandingPage), typeof(ContentFolder) }, // Pages we can create under the start page...
        ExcludeOn = new[] { typeof(ContainerPage), typeof(ProductPage), typeof(StandardPage), typeof(ISearchPage), typeof(LandingPage) })] // ...and underneath those we can't create additional start pages
    public class StartPage : SitePageData
    {
        [Display(
            GroupName = SystemTabNames.Content,
            Order = 320)]
        [CultureSpecific]
        public virtual ContentArea MainContentArea { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 330)]
        [CultureSpecific]
        public virtual ContentArea OneHalfContentArea { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 340)]
        [CultureSpecific]
        public virtual ContentArea RowContentArea { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 350)]
        [CultureSpecific]
        [BootstrapRowValidation]
        public virtual ContentArea SingleRowValidationArea { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 360)]
        [CultureSpecific]
        public virtual ContentArea BlockWithDefaultDisplayOptionArea { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 370)]
        [CultureSpecific]
        [DefaultDisplayOption(ContentAreaTags.HalfWidth)]
        public virtual ContentArea HalfDefaultDisplayOptionArea { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 380)]
        [CultureSpecific]
        public virtual ContentArea IndexedContentArea { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 390)]
        [CultureSpecific]
        public virtual ContentArea NoneDisplayOptionContentArea { get; set; }

        [Display(GroupName = Global.GroupNames.SiteSettings, Order = 300)]
        public virtual LinkItemCollection ProductPageLinks { get; set; }

        [Display(GroupName = Global.GroupNames.SiteSettings, Order = 350)]
        public virtual LinkItemCollection CompanyInformationPageLinks { get; set; }

        [Display(GroupName = Global.GroupNames.SiteSettings, Order = 400)]
        public virtual LinkItemCollection NewsPageLinks { get; set; }

        [Display(GroupName = Global.GroupNames.SiteSettings, Order = 450)]
        public virtual LinkItemCollection CustomerZonePageLinks { get; set; }

        [Display(GroupName = Global.GroupNames.SiteSettings)]
        public virtual PageReference GlobalNewsPageLink { get; set; }

        [Display(GroupName = Global.GroupNames.SiteSettings)]
        public virtual PageReference ContactsPageLink { get; set; }

        [Display(GroupName = Global.GroupNames.SiteSettings)]
        public virtual PageReference SearchPageLink { get; set; }

        [Display(GroupName = Global.GroupNames.SiteSettings)]
        public virtual SiteLogotypeBlock SiteLogotype { get; set; }

        public virtual SomeValuesEnum SomeValue { get; set; }

        public virtual string SomeMultipleValue { get; set; }

    }

    public enum SomeValuesEnum
    {
        [Display(Name = "NOONE!")]
        None = 0,
        [Display(Name = "1st value")]
        FirstValue = 1,
        [Display(Name = "This is second")]
        SecondValue = 2,
        [Display(Name = "And here comes last (3rd)")]
        ThirdOne = 3
    }
}
