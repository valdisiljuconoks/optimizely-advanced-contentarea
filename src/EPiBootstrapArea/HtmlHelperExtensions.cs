using System.Web.Mvc;

namespace EPiBootstrapArea
{
    internal static class HtmlHelperExtensions
    {
        internal static bool? GetFlagValueFromViewData(this HtmlHelper htmlHelper, string key)
        {
            return htmlHelper.ViewContext.ViewData.GetValueFromDictionary(key);
        }
    }
}
