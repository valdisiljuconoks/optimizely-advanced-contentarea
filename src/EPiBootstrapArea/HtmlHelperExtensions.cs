using System;
using System.Web.Mvc;
using EPiServer.Core;
using EPiServer.Web;

namespace EPiBootstrapArea
{
    public static class HtmlHelperExtensions
    {
        internal static bool? GetFlagValueFromViewData(this HtmlHelper htmlHelper, string key)
        {
            return htmlHelper.ViewContext.ViewData.GetValueFromDictionary(key);
        }

        public static DisplayOption GetDisplayOption(this HtmlHelper htmlHelper, BlockData block)
        {
            if(htmlHelper == null)
                throw new ArgumentNullException(nameof(htmlHelper));

            if(block == null)
                throw new ArgumentNullException(nameof(block));

            return htmlHelper.ViewContext.ViewData.ContainsKey(Constants.CurrentDisplayOptionKey)
                       ? htmlHelper.ViewContext.ViewData[Constants.CurrentDisplayOptionKey] as DisplayOption
                       : null;
        }
    }
}
