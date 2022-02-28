using Microsoft.AspNetCore.Routing;

namespace EPiBootstrapArea
{
    internal static class RouteDictionaryExtensions
    {
        internal static bool? GetFlagValue(this RouteValueDictionary additionalValues, string key)
        {
            return additionalValues.GetValueFromDictionary(key);
        }
    }
}
