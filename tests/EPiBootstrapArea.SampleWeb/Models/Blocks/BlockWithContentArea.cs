using EPiServer.Core;
using EPiServer.DataAnnotations;

namespace EPiBootstrapArea.SampleWeb.Models.Blocks
{
    [ContentType(DisplayName = "BlockWithContentArea", GUID = "3da6431f-6fd8-4547-8be3-723fb340044e", Description = "")]
    public class BlockWithContentArea : BlockData
    {
        public virtual ContentArea Area { get; set; }
    }
}
