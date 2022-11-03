using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace EPiBootstrapArea.SampleWeb.Models.Blocks
{
    [ContentType(DisplayName = "SampleLiteralBlock", GUID = "68adfd44-c72f-4201-b8c1-0f0b22bc87de", Description = "")]
    public class SampleLiteralBlock : BlockData
    {
        [CultureSpecific]
        [Display(
            Name = "Name",
            Description = "Name field's description",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual string Name { get; set; }

        [CultureSpecific]
        [Display(
            Name = "Content",
            Description = "Name field's description",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual XhtmlString Content { get; set; }

    }
}
