using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc.Html;
using HtmlAgilityPack;

namespace EPiBootstrapArea
{
    public class BootstrapAwareContentAreaRenderer : ContentAreaRenderer
    {
        private static bool _fallbackCached;
        private static IEnumerable<DisplayModeFallback> _fallbacks;

        protected override string GetContentAreaItemCssClass(HtmlHelper htmlHelper, ContentAreaItem contentAreaItem)
        {
            var tag = GetContentAreaItemTemplateTag(htmlHelper, contentAreaItem);
            var baseClasses = base.GetContentAreaItemCssClass(htmlHelper, contentAreaItem);

            return string.Format("block {0} {1} {2} {3}",
                                 GetTypeSpecificCssClasses(contentAreaItem, ContentRepository),
                                 GetCssClassesForTag(tag),
                                 tag,
                                 baseClasses);
        }

        protected override void RenderContentAreaItem(
            HtmlHelper htmlHelper,
            ContentAreaItem contentAreaItem,
            string templateTag,
            string htmlTag,
            string cssClass)
        {
            var originalWriter = htmlHelper.ViewContext.Writer;
            var tempWriter = new StringWriter();

            htmlHelper.ViewContext.Writer = tempWriter;
            var content = contentAreaItem.GetContent(ContentRepository);

            try
            {
                base.RenderContentAreaItem(htmlHelper, contentAreaItem, templateTag, htmlTag, cssClass);

                var contentItemContent = tempWriter.ToString();
                var shouldRender = IsInEditMode(htmlHelper);

                if (!shouldRender)
                {
                    var doc = new HtmlDocument();
                    doc.Load(new StringReader(contentItemContent));
                    var blockContentNode = doc.DocumentNode.ChildNodes.FirstOrDefault();

                    if (blockContentNode != null)
                    {
                        shouldRender = !string.IsNullOrEmpty(blockContentNode.InnerHtml);
                        if (!shouldRender)
                        {
                            // ReSharper disable once SuspiciousTypeConversion.Global
                            var visibilityControlledContent = content as IControlVisibility;
                            shouldRender = (visibilityControlledContent == null) || (!visibilityControlledContent.HideIfEmpty);
                        }
                    }
                }

                if (shouldRender)
                {
                    originalWriter.Write(contentItemContent);
                }
            }
            finally
            {
                // restore original writer to proceed further with rendering pipeline
                htmlHelper.ViewContext.Writer = originalWriter;
            }
        }

        private static string GetCssClassesForTag(string tagName)
        {
            ReadRegisteredDisplayModes();

            if (string.IsNullOrWhiteSpace(tagName))
            {
                tagName = ContentAreaTags.FullWidth;
            }

            var fallback = _fallbacks.FirstOrDefault(f => f.Tag == tagName);
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

            // ReSharper disable once SuspiciousTypeConversion.Global
            var customClassContent = content as ICustomCssInContentArea;
            if (customClassContent != null && !string.IsNullOrWhiteSpace(customClassContent.ContentAreaCssClass))
            {
                cssClass += string.Format(" {0}", customClassContent.ContentAreaCssClass);
            }

            return cssClass;
        }

        private static void ReadRegisteredDisplayModes()
        {
            if (_fallbackCached)
            {
                return;
            }

            var displayModeFallbackProvider = ServiceLocator.Current.GetInstance<IDisplayModeFallbackProvider>();
            _fallbacks = displayModeFallbackProvider.GetAll();
            _fallbackCached = true;
        }
    }
}
