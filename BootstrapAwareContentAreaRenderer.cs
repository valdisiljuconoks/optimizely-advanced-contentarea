using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.Data.Dynamic;
using EPiServer.Web;
using EPiServer.Web.Mvc;
using EPiServer.Web.Mvc.Html;

namespace EPiBootstrapArea
{
    public class BootstrapAwareContentAreaRenderer : ContentAreaRenderer
    {
        private static bool fallbackCached;
        private static IEnumerable<DisplayModeFallback> fallbacks;

        public BootstrapAwareContentAreaRenderer(
                IContentRenderer contentRenderer,
                TemplateResolver templateResolver,
                ContentFragmentAttributeAssembler attributeAssembler) : base(contentRenderer, templateResolver, attributeAssembler)
        {
        }

        protected override string GetContentAreaItemCssClass(HtmlHelper htmlHelper, ContentAreaItem contentAreaItem)
        {
            var tag = GetContentAreaItemTemplateTag(htmlHelper, contentAreaItem);
            return string.Format("block {0} {1} {2}", GetTypeSpecificCssClasses(contentAreaItem, ContentRepository), GetCssClassesForTag(tag), tag);
        }

        private static string GetCssClassesForTag(string tagName)
        {
            ReadRegisteredDisplayModes();

            if (string.IsNullOrWhiteSpace(tagName))
            {
                tagName = ContentAreaTags.FullWidth;
            }

            var fallback = fallbacks.FirstOrDefault(f => f.Tag == tagName);
            if (fallback == null)
            {
                return string.Empty;
            }

            return string.Format("col-lg-{0} col-md-{1} col-sm-{2} col-xs-{3}",
                    fallback.LargeScreenWidth,
                    fallback.MediumScreenWidth,
                    fallback.SmallScreenWidth,
                    fallback.ExtraSmallScreenWidth);
        }

        private static string GetTypeSpecificCssClasses(ContentAreaItem contentAreaItem, IContentRepository contentRepository)
        {
            var content = contentAreaItem.GetContent(contentRepository);
            var cssClass = content == null ? String.Empty : content.GetOriginalType().Name.ToLowerInvariant();

            var customClassContent = content as ICustomCssInContentArea;
            if (customClassContent != null && !string.IsNullOrWhiteSpace(customClassContent.ContentAreaCssClass))
            {
                cssClass += string.Format(" {0}", customClassContent.ContentAreaCssClass);
            }

            return cssClass;
        }

        private static void ReadRegisteredDisplayModes()
        {
            if (fallbackCached)
            {
                return;
            }

            var store = typeof(DisplayModeFallback).GetStore();
            fallbacks = store.LoadAll<DisplayModeFallback>();
            fallbackCached = true;
        }
    }
}
