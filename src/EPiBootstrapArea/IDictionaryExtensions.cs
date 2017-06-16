using System.Collections.Generic;

namespace EPiBootstrapArea
{
    internal static class IDictionaryExtensions
    {
        internal static bool? GetValueFromDictionary(this IDictionary<string, object> source, string key)
        {
            var actualValue = source[key];
            bool? result = null;

            if(actualValue is bool)
            {
                result = (bool) actualValue;
            }

            return result;
        }

        internal static T GetValueFromDictionary<T>(this IDictionary<string, object> source, string key)
        {
            var actualValue = source[key];
            return (T)actualValue;
        }
    }
}
