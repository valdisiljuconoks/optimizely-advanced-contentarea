using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Web;
using TechFellow.Optimizely.AdvancedContentArea;

namespace AlloySampleSite.Models.Blocks;

[SiteContentType(GUID = "0E572837-D5B2-45D0-ACF1-171DF7D2A00A")]
[SiteImageUrl] // Use site's default thumbnail
public class IndexAwareBlock : SiteBlockData
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
}
