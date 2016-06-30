using System.Collections.Generic;

namespace EPiBootstrapArea
{
    public interface IDisplayModeFallbackProvider
    {
        void Initialize();

        List<DisplayModeFallback> GetAll();
    }
}
