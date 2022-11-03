using System;
using EPiServer.Core;
using EPiServer.Web;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EPiBootstrapArea
{
    public static class IHtmlHelperExtensions
    {
        internal static bool? GetFlagValueFromViewData(this IHtmlHelper htmlHelper, string key)
        {
            return htmlHelper.ViewContext.ViewData.GetValueFromDictionary(key);
        }

        internal static string GetValueFromViewData(this IHtmlHelper htmlHelper, string key)
        {
            return htmlHelper.ViewContext.ViewData.GetValueFromDictionary<string>(key);
        }

        public static DisplayOption GetDisplayOption(this IHtmlHelper htmlHelper, BlockData block)
        {
            if(htmlHelper == null)
                throw new ArgumentNullException(nameof(htmlHelper));

            if(block == null)
                throw new ArgumentNullException(nameof(block));

            return htmlHelper.ViewContext.ViewData.ContainsKey(Constants.CurrentDisplayOptionKey)
                       ? htmlHelper.ViewContext.ViewData[Constants.CurrentDisplayOptionKey] as DisplayOption
                       : null;
        }

        public static int BlockIndex(this IHtmlHelper htmlHelper)
        {
            return (int?)htmlHelper.ViewData[Constants.BlockIndexViewDataKey] ?? -1;
        }
    }
}
