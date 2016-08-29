using EPiServer.Core;

namespace EPiBootstrapArea.SampleWeb.Models.Pages
{
    public interface IHasRelatedContent
    {
        ContentArea RelatedContentArea { get; }
    }
}
