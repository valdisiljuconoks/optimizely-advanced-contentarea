using Microsoft.AspNetCore.Routing;

namespace TechFellow.Optimizely.AdvancedContentArea
{
    internal static class RouteDictionaryExtensions
    {
        internal static bool? GetFlagValue(this RouteValueDictionary additionalValues, string key)
        {
            return additionalValues.GetValueFromDictionary(key);
        }
    }
}
