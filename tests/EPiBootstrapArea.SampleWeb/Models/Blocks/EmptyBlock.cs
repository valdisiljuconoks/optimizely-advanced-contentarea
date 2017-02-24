using EPiServer.Core;
using EPiServer.DataAnnotations;

namespace EPiBootstrapArea.SampleWeb.Models.Blocks
{
    [ContentType(DisplayName = "EmptyBlock", GUID = "0e641414-823c-486d-92c3-46db04c10e4a", Description = "")]
    public class EmptyBlock : BlockData, IControlVisibility
    {
        public bool HideIfEmpty => true;
    }
}
