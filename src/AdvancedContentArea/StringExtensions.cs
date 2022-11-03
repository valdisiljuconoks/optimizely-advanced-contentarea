using System;

namespace TechFellow.Optimizely.AdvancedContentArea
{
    internal static class StringExtensions
    {
        internal static string TryFormat(this string target, params object[] args)
        {
            try
            {
                return string.Format(target, args);
            }
            catch (FormatException)
            {
                return null;
            }
        }
    }
}
