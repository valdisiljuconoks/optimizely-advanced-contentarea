using EPiServer.Core;
using EPiServer.DataAnnotations;

namespace EPiBootstrapArea.SampleWeb.Models.Pages
{
    [ContentType(DisplayName = "EPiFormPage", GUID = "b3875f0f-13ca-4926-9da3-918bd7480865", Description = "")]
    public class EPiFormPage : SitePageData
    {
        public virtual ContentArea TheForm { get; set; }
    }
}
