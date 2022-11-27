using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Web;
using TechFellow.Optimizely.AdvancedContentArea;

namespace AlloySampleSite.Models.Blocks;

[SiteContentType(GUID = "6AD9A1B9-FDFA-4037-A790-206DE2D1F798")]
[SiteImageUrl] // Use site's default thumbnail
[DefaultDisplayOption(ContentAreaTags.HalfWidth)]
public class DefaultToOneHalfTeaserBlock : SiteBlockData, ICustomCssInContentArea
{
    [CultureSpecific]
    [Required(AllowEmptyStrings = false)]
    [Display(
        GroupName = SystemTabNames.Content,
        Order = 1)]
    public virtual string Heading { get; set; }

    [CultureSpecific]
    [Required(AllowEmptyStrings = false)]
    [Display(
        GroupName = SystemTabNames.Content,
        Order = 2)]
    [UIHint(UIHint.Textarea)]
    public virtual string Text { get; set; }

    [CultureSpecific]
    [Required(AllowEmptyStrings = false)]
    [UIHint(UIHint.Image)]
    [Display(
        GroupName = SystemTabNames.Content,
        Order = 3)]
    public virtual ContentReference Image { get; set; }

    [Display(
        GroupName = SystemTabNames.Content,
        Order = 4)]
    public virtual PageReference Link { get; set; }

    public string ContentAreaCssClass => "add-this-custom-css-class";
}
