using System.Collections.Generic;

namespace TechFellow.Optimizely.AdvancedContentArea
{
    public interface IDisplayModeFallbackProvider
    {
        void Initialize();

        List<DisplayModeFallback> GetAll();
    }
}
