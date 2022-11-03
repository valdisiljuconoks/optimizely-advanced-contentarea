using System.Collections.Generic;

namespace TechFellow.Optimizely.AdvancedContentArea
{
    public static class ListOfDisplayModeFallbackExtensions
    {
        public static List<DisplayModeFallback> Add<T>(this List<DisplayModeFallback> target) where T : DisplayModeFallback, new()
        {
            target.Add(new T());

            return target;
        }
    }
}
